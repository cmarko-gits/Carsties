using Contracts;
using MassTransit;
using MongoDB.Entities;
using SearchService.Model;

namespace SearchService.Consumers
{
    public class AuctionFInishedConsumer : IConsumer<AuctionFinished>
    {



        public async Task Consume(ConsumeContext<AuctionFinished> context)
        {
            var auction = await DB.Find<Item>().OneAsync(context.Message.AuctionId);

            if (context.Message.ItemSold)
            {
                auction.Winner = context.Message.Winner;
                auction.SoldAmount = (int)context.Message.Amount;
            }

            auction.Status = "Finished";

            await auction.SaveAsync();
        }

    }
}