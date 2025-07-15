using AutoMapper;
using Contracts;
using SearchService.Model;
namespace SearchService.RequestHelper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {

            CreateMap<AuctionCreated, Item>();
            CreateMap<AuctionUpdated , Item>();
        }
    }
}