using AdventureWorksCore.Models.DataTables;
using AutoMapper;
using System;
using System.Reflection;
using static AdventureWorksCore.Models.DataTables.DataTableServerSideRequest;
using System.ComponentModel.DataAnnotations.Schema;
using AgileObjects.ReadableExpressions;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;

namespace AdventureWorksCore.Utility
{
    public static class DataTableUtility
    {


        /*
         * Build a search (filter) predicate for DataTables.
         * Search Definition: https://datatables.net/reference/api/search
         * 
         * TODO:
         *   -Smart Search Feature: Match words out of order.
         *   -Smart Search Feature: Preserved text.
         *   -Privledged Feature: Regex
         * 
         * COMPLETED:
         *   -Smart Search Feature: Partial word matching.
         *   
         * IN PROGRESS:
         *   -Optimization
         *      --Where queries are not running on db
         *      --Non-basic OrderBy queries are not running on db
         */
        public static IQueryable<T> FilterData<T>(DataTableServerSideRequest request, IQueryable<T> data)
        {
            ParameterExpression pe = Expression.Parameter(typeof(T));  //i.e. t =>

            //init constant expression value for searching on all columns
            Expression globalSearchValue = ExpressionUtility.CallToString(Expression.Constant(request.Search.Value)); //i.e. 13.ToString()

            bool searchOnGlobal = !String.IsNullOrWhiteSpace(request.Search.Value);

            Expression columnFilterExpression = null;
            Expression globalFilterExpression = null;
            foreach (DataTableColumn column in request.Columns)
            {
                if (!column.Searchable)
                    continue;

                if (column.Data == null) //not a data column
                    continue;

                var prop = typeof(T).GetProperty(column.Data,
                    BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public); //Ignore Case for JSON

                Expression targetValue = ExpressionUtility.CallToString(ReflectionUtility.GetterExpression(prop, pe)); //i.e. customer.AccountNumber

                if (!String.IsNullOrWhiteSpace(column.Search.Value))
                {
                    Expression columnSearchValue = ExpressionUtility.CallToString(Expression.Constant(column.Search.Value));
                    columnFilterExpression = ExpressionUtility.AndAlsoIgnoreNull(
                        columnFilterExpression,
                        ExpressionUtility.CallContains(targetValue, columnSearchValue));
                }

                if (!searchOnGlobal)
                    continue;

                globalFilterExpression = ExpressionUtility.OrElseIgnoreNull(
                    globalFilterExpression,
                    ExpressionUtility.CallContains(targetValue, globalSearchValue));
            }

            Expression masterExpression = ExpressionUtility.AndAlsoIgnoreNull(columnFilterExpression, globalFilterExpression);
            Debug.WriteLine(masterExpression?.ToReadableString());

            if (masterExpression == null)
                return data;

            return data.Where(Expression.Lambda<Func<T, bool>>(masterExpression, pe));
        }

        public static IQueryable<T> OrderData<T>(DataTableServerSideRequest request, IQueryable<T> data)
        {
            bool first = true; //denotes the first order expression uses OrderBy and subsequent order expressions use ThenBy
            foreach (DataTableOrder order in request.Order)
            {
                ParameterExpression pe = Expression.Parameter(typeof(T));

                var prop = typeof(T).GetProperty(
                    request.Columns[order.Column].Data,
                    BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public); //Ignore Case for JSON



                var funcType = typeof(Func<,>).MakeGenericType(typeof(T), prop.PropertyType);

                //there are 2 methods with the same parameter signature, but one of them is a generic method
                //GetMethod wont work in this case since it cannot differentiate between the two
                var lambdaMethod = typeof(Expression)
                    .GetMethods().Single(
                    m =>
                        m.Name == nameof(Expression.Lambda) &&
                        m.IsGenericMethod &&
                        m.GetParameters().Length == 2 &&
                        m.GetParameters()[0].ParameterType == typeof(Expression) &&
                        m.GetParameters()[1].ParameterType == typeof(ParameterExpression[]))
                    .MakeGenericMethod(funcType);

                //this method is an extension method
                //GetMethod wont work with extension methods
                var orderMethod = typeof(Queryable)
                    .GetMethods().Single(
                    m =>
                        m.Name == (first ?
                                    (order.Dir == "desc" ? nameof(Queryable.OrderByDescending) : nameof(Queryable.OrderBy)) :
                                    (order.Dir == "desc" ? nameof(Queryable.ThenByDescending) : nameof(Queryable.ThenBy))
                                    ) &&
                        m.GetParameters().Length == 2)
                    .MakeGenericMethod(typeof(T), prop.PropertyType);

                first = false;

                var exp = ReflectionUtility.GetterExpression(prop, pe);
                //var lambda = Expression.Lambda<Func<T, >>(exp, pe);

                var lambda = lambdaMethod.Invoke(null, new object[] { exp, new[] { pe } });
                data = (IQueryable<T>)orderMethod.Invoke(null, new[] { data, lambda });
            }
            return data;
        }

        public static DataTableServerSideResponse<T> GetDataTableData<T>(DataTableServerSideRequest request, IMapper mapper, IQueryable<T> data)
        {
            DataTableServerSideResponse<T> response = new DataTableServerSideResponse<T>
            {
                Draw = request.Draw, //used by DataTables api to prevent xss
                RecordsTotal = data.Count() //total number of records before filtering
            };

            data = FilterData(request, data);
            data = OrderData(request, data);

            response.RecordsFiltered = data.Count(); //total number of records after filtering


            response.Data = data.Skip(request.Start).Take(request.Length).ToList();

            return response;
        }

        public static void CollectColumnMetaData<T>(DataTableServerSideRequest request)
        {
            foreach (var column in request.Columns)
            {
                var prop = typeof(T).GetProperty(column.Data,
                    BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public);

                var isMapped = prop.IsDefined(typeof(NotMappedAttribute));

            }

        }
    }
}