using AdventureWorks.BLL;
using AdventureWorks.Models.Dto;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdventureWorks.AutoMapper
{
    public class PresentationProfile : Profile
    {
        public PresentationProfile()
        {
            CreateMap<CustomerBl, CustomerDto>();
            CreateMap<CustomerBl, CustomerDetailDto>();
            CreateMap<StoreBl, StoreDto>();
            CreateMap<StoreBl, StoreDetailDto>();
            CreateMap<SalesTerritoryBl, SalesTerritoryDto>();
            CreateMap<SalesTerritoryBl, SalesTerritoryDetailDto>();
            CreateMap<PersonBl, PersonDto>();
            CreateMap<PersonBl, PersonDetailDto>();
        }
    }
}
