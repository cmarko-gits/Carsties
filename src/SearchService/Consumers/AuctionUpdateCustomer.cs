using AutoMapper;
using Contracts;
using MassTransit;
using MongoDB.Entities;
using SearchService.Model;

namespace SearchService.Consumers
{
public class AuctionUpdatedConsumer : IConsumer<AuctionUpdated>
    {
        private readonly IMapper _mapper;
        public AuctionUpdatedConsumer(IMapper mapper)
        {
            _mapper = mapper;
        }


        public async Task Consume(ConsumeContext<AuctionUpdated> context)
{
                Console.WriteLine("----> Consuming AuctionUpdated : ", context.Message.Id);

    var item = _mapper.Map<Item>(context.Message);
    item.ID = context.Message.Id; // <-- jako važno

    var result = await DB.Update<Item>()
        .Match(i => i.ID == context.Message.Id)
        .ModifyOnly(x => new
        {
            x.Color,
            x.Make,
            x.Model,
            x.Year,
            x.Mileage
        }, item)
        .ExecuteAsync();

    if (!result.IsAcknowledged)
    {
        throw new MessageException(typeof(AuctionUpdated), "Problem updating mongodb");
    }
}


    }
}