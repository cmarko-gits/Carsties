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
            // Kreiramo query sa paginacijom
            var query = DB.PagedSearch<Item, Item>();

            // Ako postoji SearchTerm, filtriramo sa tekstualnom pretragom (full text)
            if (!string.IsNullOrEmpty(searchParams.SearchTerm))
            {
                query = query.Match(Search.Full, searchParams.SearchTerm)
                             .SortByTextScore();
            }

            /*query = searchParams.FilterBy switch
            {
                "finished" => query.Match(x => x.AuctionEnd < DateTime.UtcNow),
                "endingSoon" => query.Match(x => x.AuctionEnd < DateTime.UtcNow.AddHours(6) && x.AuctionEnd > DateTime.UtcNow),
                _ => query.Match(x => x.AuctionEnd > DateTime.UtcNow),
            };*/

            // Filtriramo po seller-u ako je prosleđen parametar
            if (!string.IsNullOrEmpty(searchParams.Seller))
            {
                query = query.Match(x => x.Seller == searchParams.Seller);
            }

            // Sortiramo rezultate prema parametru OrderBy
            query = searchParams.OrderBy switch
            {
                "make" => query.Sort(x => x.Ascending(x => x.Make)),
                "new" => query.Sort(x => x.Descending(x => x.CreatedAt)),
                _ => query.Sort(x => x.Ascending(x => x.AuctionEnd)),
            };

            // Postavljamo paginaciju sa podrazumevanim vrednostima ako nisu prosleđene
            int pageNumber = searchParams.PageNumber > 0 ? searchParams.PageNumber : 1;
            int pageSize = (searchParams.PageSize > 0 && searchParams.PageSize <= 100) ? searchParams.PageSize : 20;

            query = query.PageNumber(pageNumber).PageSize(pageSize);

            // Izvršavamo query i dobijamo rezultate
            var result = await query.ExecuteAsync();

            // Vraćamo rezultate i informacije o paginaciji kao JSON
            return Ok(new
            {
                result = result.Results,
                pageCount = result.PageCount,
                totalCount = result.TotalCount
            });
        }
    }
}
