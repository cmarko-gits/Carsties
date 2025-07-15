using AuctionService.Data;
using AuctionService.DTOs;
using AuctionService.Entities;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Contracts;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace AuctionService.Controllers
{
    [ApiController]
    [Route("api/auctions")]
    public class AuctionsController : ControllerBase
    {
        private readonly AuctionDbContext _context;
        private readonly IMapper _mapper;
        private readonly IPublishEndpoint _publishEndpoint;
        public AuctionsController(AuctionDbContext _context, IMapper mapper, IPublishEndpoint publishEndpoint)
        {
            this._context = _context;
            this._mapper = mapper;
            this._publishEndpoint = publishEndpoint;
        }

        [HttpGet]
        public async Task<ActionResult<List<AuctionDto>>> GetAllAuctions(string? date)
        {

            var query = _context.Auctions.OrderBy(x=>x.Item.Make).AsQueryable();

            if (!string.IsNullOrWhiteSpace(date))
            {
                query = query.Where(x=>x.UpdatedAt.CompareTo(DateTime.Parse(date).ToUniversalTime()) > 0);
            }
            
            return await query.ProjectTo<AuctionDto>(_mapper.ConfigurationProvider).ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AuctionDto>> GetAuction(Guid id)
        {
            var auction = await _context.Auctions.Include(x => x.Item).FirstOrDefaultAsync(i => i.Id == id);

            if (auction == null) return NotFound(id);

            var auctionDto = _mapper.Map<AuctionDto>(auction);

            return Ok(auctionDto);
        }

        [HttpPost]
        public async Task<ActionResult<AuctionDto>> CreateAuction(CreateAuctionDto auctionDto)
        {
            var auction = _mapper.Map<Auction>(auctionDto);

            auction.Seller = "test";

            _context.Auctions.Add(auction);

            var result = await _context.SaveChangesAsync() > 0;

            if (!result) 
                return BadRequest("Could not save changes to the db");

            // Sada je auction.Id sigurno postavljen
            var newAuction = _mapper.Map<AuctionDto>(auction);

            // Pošalji događaj nakon što su podaci sačuvani
            await _publishEndpoint.Publish(_mapper.Map<AuctionCreated>(newAuction));

            return CreatedAtAction(nameof(GetAuction), new { auction.Id }, newAuction);

        }
[HttpPut("{id}")]
public async Task<ActionResult<AuctionDto>> UpdateAuction(Guid id, UpdateAuctionDto updateAuctionDto)
{
    var auction = await _context.Auctions.Include(i => i.Item).FirstOrDefaultAsync(i => i.Id == id);

    if (auction == null) return NotFound();

    // Ažuriraj polja
    auction.Item.Make = updateAuctionDto.Make ?? auction.Item.Make;
    auction.Item.Model = updateAuctionDto.Model ?? auction.Item.Model;
    auction.Item.Color = updateAuctionDto.Color ?? auction.Item.Color;
    auction.Item.Mileage = updateAuctionDto.Mileage ?? auction.Item.Mileage;
    auction.Item.Year = updateAuctionDto.Year ?? auction.Item.Year;

    // 1️⃣ Prvo sačuvaj promene u bazu
    var result = await _context.SaveChangesAsync() > 0;

    if (!result) return BadRequest("Auction couldn't be updated");

    // 2️⃣ Tek onda pošalji događaj
    await _publishEndpoint.Publish(_mapper.Map<AuctionUpdated>(auction));

    // Vrati izmenjeni DTO
    var auctionDto = _mapper.Map<AuctionDto>(auction);
    return Ok(auctionDto);
}


        [HttpDelete("{id}")]
        public async Task<ActionResult> ActionResultAsync(Guid id)
        {
            var auction = await _context.Auctions.FindAsync(id);

            if (auction == null) return NotFound();

            _context.Auctions.Remove(auction);

            await _publishEndpoint.Publish<AuctionDeleted>(new { Id = auction.Id.ToString() });
            

            var result = await _context.SaveChangesAsync() > 0;

            if (!result) return BadRequest("Could not remove ");

            return Ok(result);  
        }

    }
}