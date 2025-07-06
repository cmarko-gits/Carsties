namespace AuctionService.Entities
{
    public class Auction
    {
        public Guid Id { get; set; }
        public int ReservePrice { get; set; } = 0;
        public string Seller { get; set; }
        public string? Winner { get; set; } // nullable ako nema pobednika još
        public int? SoldAmount { get; set; }
        public int? CurrentHighBid { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public DateTime AuctionEnd { get; set; } = DateTime.UtcNow;
        public Status Status { get; set; }
        public Item Item { get; set; } // Po konvenciji veliko slovo
    }
}
