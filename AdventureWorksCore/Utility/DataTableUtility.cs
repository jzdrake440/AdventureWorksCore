using AdventureWorksCore.Models.DataTables;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using static AdventureWorksCore.Models.DataTables.DataTableServerSideRequest;

namespace AdventureWorksCore.Utility
{
    public static class DataTableUtility
    {
        public static List<T> FilterData<T>(DataTableServerSideRequest request, IEnumerable<T> data)
        {
            return data.Where(SearchPredicate<T>(request)).ToList();
        }

        public static Func<T, bool> SearchPredicate<T>(DataTableServerSideRequest request)
        {
            ParameterExpression pe = Expression.Parameter(typeof(T));
            Expression globalSearchValue = ToStringExpression(Expression.Constant(request.Search.Value));

            bool searchOnGlobal = !String.IsNullOrWhiteSpace(request.Search.Value);

            Expression columnFilterExpression = null;
            Expression globalFilterExpression = null;
            foreach (DataTableColumn column in request.Columns)
            {
                if (!column.Searchable)
                    continue;

                if (column.Data == null)
                    continue;

                var prop = typeof(T).GetProperty(column.Data,
                    BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public);

                Expression targetValue = ToStringExpression(Expression.Property(pe, prop)); //i.e. customer.AccountNumber

                if (!String.IsNullOrWhiteSpace(column.Search.Value))
                {
                    Expression columnSearchValue = ToStringExpression(Expression.Constant(column.Search.Value));
                    columnFilterExpression = AndAlsoIgnoreNull(columnFilterExpression, ContainsExpression(targetValue, columnSearchValue));
                }

                if (!searchOnGlobal)
                    continue;

                globalFilterExpression = OrElseIgnoreNull(globalFilterExpression, ContainsExpression(targetValue, globalSearchValue));
            }

            Expression masterExpression = AndAlsoIgnoreNull(columnFilterExpression, globalFilterExpression);

            if (masterExpression == null)
                return c => true;
            else
                return Expression.Lambda<Func<T, bool>>(masterExpression, pe).Compile();
        }

        private static Expression ToStringExpression(Expression e)
        {
            return Expression.Call(e, "ToString", Type.EmptyTypes);
        }

        private static Expression ContainsExpression(Expression caller, Expression arg1)
        {

            return Expression.Call(caller, "Contains", Type.EmptyTypes, arg1);
        }

        private static Expression OrElseIgnoreNull(Expression left, Expression right)
        {
            if (left == null && right == null)
                return null;

            if (left == null)
                return right;

            if (right == null)
                return left;

            return Expression.OrElse(left, right);
        }

        private static Expression AndAlsoIgnoreNull(Expression left, Expression right)
        {
            if (left == null && right == null)
                return null;

            if (left == null)
                return right;

            if (right == null)
                return left;

            return Expression.AndAlso(left, right);
        }

        public static void OrderData<T>(DataTableServerSideRequest request, List<T> data)
        {
            List<DataTableOrderDataOption> options = new List<DataTableOrderDataOption>();

            foreach (DataTableOrder order in request.Order) //simplify the in data for the compare delegate
            {
                if (!request.Columns[order.Column].Orderable)
                    continue;

                if (request.Columns[order.Column].Data == null)
                    continue;

                var prop = typeof(T).GetProperty(
                    request.Columns[order.Column].Data, 
                    BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public);

                bool isNullable = prop.PropertyType.IsGenericType && 
                    typeof(Nullable<>) == prop.PropertyType.GetGenericTypeDefinition();

                var underlyingType = isNullable ?
                    prop.PropertyType.GetGenericArguments()[0] :
                    prop.PropertyType;

                var option = new DataTableOrderDataOption
                {
                    Property = prop,
                    FromNullable = isNullable,
                    Comparable = underlyingType.GetInterface("IComparable")?.GetMethod("CompareTo"),
                    Direction = order.Dir
                };


                if (option.Comparable == null) //ensure that the dynamic field is properly comparable
                    throw new ArgumentException(
                        String.Format(
                            "Attempting to sort on uncomparable property {0}.{1}; Implement the IComparable interface.",
                            typeof(T).FullName,
                            option.Property.Name)
                        );

                options.Add(option);
            }

            data.Sort((t1, t2) =>
            {
                foreach (DataTableOrderDataOption option in options)
                {
                    var propVal1 = option.Property.GetValue(t1);
                    var propVal2 = option.Property.GetValue(t2);

                    if (propVal1 == null && propVal2 == null)
                        continue;

                    if (propVal1 == null)
                        return option.Dir(-1);

                    if (propVal2 == null)
                        return option.Dir(1);

                    int c = (int)option.Comparable.Invoke(propVal1, new object[] { propVal2 });

                    if (c != 0)
                        return option.Dir(c);
                }
                return 0;
            });
        }

        private class DataTableOrderDataOption
        {
            public const string DIRECTION_ASC = "asc";
            public const string DIRECTION_DESC = "desc";

            public PropertyInfo Property { get; set; }
            public MethodInfo Comparable { get; set; }
            public bool FromNullable { get; set; }
            public string Direction { get; set; }

            public int Dir(int c) //change direction as necessary
            {
                return Direction == DIRECTION_ASC ? c : c * -1;
            }
        }

        public static DataTableServerSideResponse<T> GetDataTableData<T>(DataTableServerSideRequest request, IMapper mapper, IEnumerable<T> data)
        {
            DataTableServerSideResponse<T> response = new DataTableServerSideResponse<T>
            {
                Draw = request.Draw, //used by DataTables api to prevent xss
                RecordsTotal = data.Count() //total number of records before filtering
            };

            //Search Filtering
            var filteredData = FilterData(request, data);
            response.RecordsFiltered = filteredData.Count(); //total number of records after filtering

            //Ordering
            OrderData(request, filteredData);

            //Pagination
            if (filteredData.Count < request.Start + request.Length) //i.e. start = 51, length = 10, count = 55; 55 < 51 + 10
                response.Data = mapper.Map<List<T>>(filteredData.GetRange(request.Start, filteredData.Count - request.Start));// i.e. cont GetRange(51, 55-51)
            else
                response.Data = mapper.Map<List<T>>(filteredData.GetRange(request.Start, request.Length));

            return response;
        }
    }
}