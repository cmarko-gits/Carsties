using AutoMapper;
using Contracts;
using MassTransit;
using MongoDB.Entities;
using SearchService.Model;

namespace SearchService.Consumers
{
    public class AuctionDeleteddConsumer : IConsumer<AuctionDeleted>
    {

        private readonly IMapper _mapper;

        public AuctionDeleteddConsumer(IMapper mapper)
        {
            _mapper = mapper;
        }

        public async Task Consume(ConsumeContext<AuctionDeleted> context)
        {
            Console.WriteLine("----> Consuming AuctionDeleted : ", context.Message.Id);

            var result = await DB.DeleteAsync<Item>(context.Message.Id);

            if (!result.IsAcknowledged)
            {
                throw new MessageException(typeof(AuctionDeleted) , "Problem deleting auction");
            }
        }

    }
}