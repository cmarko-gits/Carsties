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
        public async Task<ActionResult<List<Item>>> SearchItems([FromQuery] SearchParams searchParams)
        {
            var query = DB.PagedSearch<Item,Item>();

            query.Sort(x => x.Ascending(a => a.Make));

            if (!string.IsNullOrEmpty(searchParams.SearchTerm))
            {
                query.Match(Search.Full, searchParams.SearchTerm).SortByTextScore();
            }

            query = searchParams.OrderBy switch
            {
                "make" => query.Sort(x => x.Ascending(x=>x.Make)),
                "new" => query.Sort(x => x.Descending(x=>x.CreatedAt)),
                 _ => query.Sort(x => x.Ascending(x=>x.AuctionEnd)),
            };

            query = searchParams.FilterBy switch
            {
                "finished" => query.Match(x => x.AuctionEnd < DateTime.UtcNow),
                "endingSoon" => query.Match(x => x.AuctionEnd < DateTime.UtcNow.AddHours(6) && x.AuctionEnd > DateTime.UtcNow),
                _ => query.Match(x => x.AuctionEnd > DateTime.UtcNow)
            };

            if (!string.IsNullOrEmpty(searchParams.SearchTerm))
            {
                query.Match(x => x.Seller == searchParams.Seller);
            }

            query.PageNumber(searchParams.PageNumber);
            query.PageSize(searchParams.PageSize);

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