using AuctionService.DTOs;
using AuctionService.Entities;
using AutoMapper;
using Contracts;
namespace AuctionService.RequestHelper
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<Auction, AuctionDto>().IncludeMembers(x => x.Item);
            CreateMap<Item, AuctionDto>();
            CreateMap<CreateAuctionDto, Auction>()
                .ForMember(d => d.Item, o => o.MapFrom(s => s));
            CreateMap<CreateAuctionDto, Item>();
            CreateMap<AuctionDto, AuctionCreated>();
                        CreateMap<Auction, AuctionUpdated>();
//01980ca9-393f-788e-88c0-00eacd4ec357
        }
    }
}