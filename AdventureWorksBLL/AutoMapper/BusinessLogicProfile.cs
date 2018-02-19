using AdventureWorks.BLL;
using AdventureWorks.DAL;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdventureWorks.AutoMapper
{
    public class BusinessLogicProfile : Profile
    {
        public BusinessLogicProfile()
        {
            CreateMap<Customer, CustomerBl>();
            CreateMap<Store, StoreBl>();
            CreateMap<SalesTerritory, SalesTerritoryBl>();
            CreateMap<Person, PersonBl>();
            CreateMap<SalesOrderHeader, SalesOrderHeaderBl>();
            CreateMap<BusinessEntity, BusinessEntityBl>();
            CreateMap<Employee, EmployeeBl>();
            CreateMap<Password, PasswordBl>();
        }
    }
}
