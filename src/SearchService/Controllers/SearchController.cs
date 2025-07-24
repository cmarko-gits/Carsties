using Microsoft.AspNetCore.Mvc;
using MongoDB.Entities;
using SearchService.Model;
using SearchService.RequestHelper;

namespace SearchService.Controllers
{
    [ApiController]
    [Route("api/search")]
    public class SearchController : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult> SearchItems([FromQuery] SearchParams searchParams)
        {
            var query = DB.PagedSearch<Item, Item>();

            if (!string.IsNullOrEmpty(searchParams.SearchTerm))
            {
                query = query.Match(Search.Full, searchParams.SearchTerm)
                             .SortByTextScore();
            }

            Console.WriteLine($"FILTERBY: {searchParams.FilterBy}");

            var filter = searchParams.FilterBy?.Trim().ToLower();

            query = filter switch
            {
                "finished" => query.Match(x => x.AuctionEnd < DateTime.UtcNow),
                "endingsoon" => query.Match(x => x.AuctionEnd < DateTime.UtcNow.AddHours(6) && x.AuctionEnd > DateTime.UtcNow),
                "live" => query.Match(x => x.AuctionEnd > DateTime.UtcNow),
                _ => query, 
            };


            if (!string.IsNullOrEmpty(searchParams.Seller))
            {
                query = query.Match(x => x.Seller == searchParams.Seller);
            }

            query = searchParams.OrderBy switch
            {
                "make" => query.Sort(x => x.Ascending(x => x.Make)),
                "new" => query.Sort(x => x.Descending(x => x.CreatedAt)),
                _ => query.Sort(x => x.Ascending(x => x.AuctionEnd)),
            };

            int pageNumber = searchParams.PageNumber > 0 ? searchParams.PageNumber : 1;
            int pageSize = (searchParams.PageSize > 0 && searchParams.PageSize <= 100) ? searchParams.PageSize : 20;

            query = query.PageNumber(pageNumber).PageSize(pageSize);

            var result = await query.ExecuteAsync();

            return Ok(new
            {
                result = result.Results,
                pageCount = result.PageCount,
                totalCount = result.TotalCount
            });
        }
    }
}
