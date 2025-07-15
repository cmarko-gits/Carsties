using MongoDB.Entities;
using SearchService.Model;

namespace SearchService.Services
{
    public class AuctionSvcHttpClient
    {

        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public AuctionSvcHttpClient(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _config = config;
        }

        public async Task<List<Item>> GetItemsForSearchDb()
{
    var lastItem = await DB.Find<Item>()
        .Sort(x => x.Descending(x => x.UpdatedAt))
        .ExecuteFirstAsync();

    string url;

    if (lastItem == null)
    {
        // Ako nema nijednog item-a u bazi, ne šalji date parametar
        url = _config["AuctionServiceUrl"] + "/api/auctions";
    }
    else
    {
        var lastUpdated = lastItem.UpdatedAt.ToString("o"); // ISO format
        url = _config["AuctionServiceUrl"] + "/api/auctions?date=" + Uri.EscapeDataString(lastUpdated);
    }

    Console.WriteLine("GET -> " + url);

    return await _httpClient.GetFromJsonAsync<List<Item>>(url);
}



    }
}