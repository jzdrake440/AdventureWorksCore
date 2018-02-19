using System.Linq;
using AdventureWorks.BLL.DataTables;
using AutoMapper;

namespace AdventureWorks.BLL.Services
{
    public interface IDataTableService
    {
        DataTableServerSideResponse<TMapTo> GetDataTableData<T, TMapTo>(DataTableServerSideRequest request, IQueryable<T> data);
    }
}