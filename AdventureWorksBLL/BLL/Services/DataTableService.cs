using AdventureWorks.BLL.DataTables;
using AutoMapper;
using System;
using System.Reflection;
using static AdventureWorks.BLL.DataTables.DataTableServerSideRequest;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
using AdventureWorks.BLL.Utility;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using AdventureWorks.DAL;

namespace AdventureWorks.BLL.Services
{
    public class DataTableService : IDataTableService
    {
        private readonly IPpeService _ppeService;
        private readonly IMapper _mapper;
        private readonly ILogger<DataTableService> _logger;

        public DataTableService(IPpeService ppeService, IMapper mapper, ILogger<DataTableService> logger)
        {
            _ppeService = ppeService;
            _mapper = mapper;
            _logger = logger;
        }


        /*
         * Filter (Search) Definition: https://datatables.net/reference/api/search
         * 
         * TODO:
         *   -Smart Search Feature: Match words out of order.
         *   -Smart Search Feature: Preserved text.
         *   -Privileged Feature: Regex
         * 
         * COMPLETED:
         *   -Smart Search Feature: Partial word matching.
         *   -Optimization
         *      --Where queries are not running on db (in some cases)
         *      --OrderBy queries are not running on db (in some cases)
         *   
         * IN PROGRESS:
         */
        public IQueryable<T> FilterData<T>(DataTableServerSideRequest request, IQueryable<T> data)
        {
            ParameterExpression pe = Expression.Parameter(typeof(T));  //i.e. t =>

            //init constant expression value for searching on all columns
            var globalSearchValue = Expression.Constant(request.Search.Value);

            bool searchOnGlobal = !String.IsNullOrWhiteSpace(request.Search.Value);

            Expression globalFilterExpression = null;
            foreach (DataTableColumn column in request.Columns)
            {
                if (!column.Searchable)
                    continue;

                if (column.Data == null) //not a data column
                    continue;

                var prop = typeof(T).GetProperty(column.Data,
                    BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public); //Ignore Case for JSON

                var targetValue = (prop != null) ?
                    ExpressionUtility.CallToString(Expression.Property(pe, prop)) : //i.e. customer.AccountNumber
                    _ppeService.GetExpression<T>(pe, column.Data);

                if (!String.IsNullOrWhiteSpace(column.Search.Value))
                {
                    var columnContains = ExpressionUtility.CallContains(targetValue, Expression.Constant(column.Search.Value));
                    data = data.Where(Expression.Lambda<Func<T, bool>>(columnContains, pe));
                }

                if (searchOnGlobal)
                    globalFilterExpression = ExpressionUtility.OrElseIgnoreNull(
                        globalFilterExpression,
                        ExpressionUtility.CallContains(targetValue, globalSearchValue));
            }
            
            if (searchOnGlobal)
                data = data.Where(Expression.Lambda<Func<T, bool>>(globalFilterExpression, pe));

            return data;
        }


        public IQueryable<Customer> FilterData2(DataTableServerSideRequest request, IQueryable<Customer> data)
        {
            return data.Where(c =>
                (c.Person != null ?
                    string.Concat(string.Concat(c.Person.LastName, ", "), c.Person.FirstName):
                    c.Store != null ?
                        c.Store.Name :
                        "Unknown").Contains("A Bik")
            );
        }

        public IQueryable<T> OrderData<T>(DataTableServerSideRequest request, IQueryable<T> data)
        {
            bool first = true; //denotes the first order expression uses OrderBy and subsequent order expressions use ThenBy
            foreach (DataTableOrder order in request.Order)
            {
                ParameterExpression pe = Expression.Parameter(typeof(T));

                var prop = typeof(T).GetProperty(
                    request.Columns[order.Column].Data,
                    BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public); //Ignore Case for JSON

                var exp = (prop != null) ?
                    Expression.Property(pe, prop) :
                    _ppeService.GetExpression<T>(pe, request.Columns[order.Column].Data);

                var funcType = typeof(Func<,>).MakeGenericType(typeof(T), exp.Type);

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
                    .MakeGenericMethod(typeof(T), exp.Type);

                first = false;
                
                var lambda = lambdaMethod.Invoke(null, new object[] { exp, new[] { pe } });
                data = (IQueryable<T>)orderMethod.Invoke(null, new[] { data, lambda });
            }
            return data;
        }

        public DataTableServerSideResponse<TMapTo> GetDataTableData<T, TMapTo>(DataTableServerSideRequest request, IQueryable<T> data)
        {
            DataTableServerSideResponse<TMapTo> response = new DataTableServerSideResponse<TMapTo>
            {
                Draw = request.Draw, //used by DataTables api to prevent xss
                RecordsTotal = data.Count() //total number of records before filtering
            };

            data = FilterData(request, data);
            data = OrderData(request, data);

            response.RecordsFiltered = data.Count(); //total number of records after filtering

            data = data.Skip(request.Start).Take(request.Length);

            _logger.LogInformation(data.Expression.ToString());

            response.Data = _mapper.Map<List<TMapTo>>(data.ToList());

            return response;
        }
    }
}