using AdventureWorksCore.Models.Dto;
using AdventureWorksCore.Models.Entity;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdventureWorksCore.AutoMapper
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<Customer, CustomerDto>();
            CreateMap<Customer, CustomerDetailDto>();
            CreateMap<Store, StoreDto>();
            CreateMap<Store, StoreDetailDto>();
            CreateMap<SalesTerritory, SalesTerritoryDto>();
            CreateMap<SalesTerritory, SalesTerritoryDetailDto>();
            CreateMap<Person, PersonDto>();
            CreateMap<Person, PersonDetailDto>();
        }
    }
}
