using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API.Context;
using API.Models;
using API.Service;
using API.Models.API.Models;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PokedexController : ControllerBase
    {
        private readonly string _accessKey;
        private readonly string _secretKey;
        private readonly AppDBContext _context;
        private readonly R2Service _r2Service;

        public PokedexController(AppDBContext context, IConfiguration configuration, AppConfiguration config)
        {
            _context = context;
            _accessKey = config.AccessKey;
            _secretKey = config.SecretKey;

            _r2Service = new R2Service(_accessKey, _secretKey);
        }

        // GET: api/Pokedex
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Pokedex>>> GetPokedexEntries()
        {
            var pokedexEntries = await _context.Pokedex
                .ToListAsync();

            return Ok(pokedexEntries);
        }

        // GET: api/Pokedex/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Pokedex>> GetPokedexEntry(int id)
        {
            var pokedexEntry = await _context.Pokedex.FindAsync(id);

            if (pokedexEntry == null)
            {
                return NotFound();
            }

            return Ok(pokedexEntry);
        }

        // POST: api/Pokedex
        [HttpPost]
        public async Task<IActionResult> PostPokedexEntry([FromForm] PokedexDTO pokedexDTO)
        {
            string imageUrl = null;
            if (pokedexDTO.ProfilePicture != null && pokedexDTO.ProfilePicture.Length > 0)
            {

                try
                {
                    using (var fileStream = pokedexDTO.ProfilePicture.OpenReadStream())
                    {
                        imageUrl = await _r2Service.UploadToR2(fileStream, pokedexDTO.Name);
                    }
                }
                catch (Exception ex)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, $"Error uploading file: {ex.Message}");
                }

            }

            var pokedexEntry = new Pokedex
            {
                Name = pokedexDTO.Name,
                Type = pokedexDTO.Type,
                Art = pokedexDTO.Art,
                Hp = pokedexDTO.Hp,
                Attack = pokedexDTO.Attack,
                Defense = pokedexDTO.Defense,
                Speed = pokedexDTO.Speed,
                Weight = pokedexDTO.Weight,
                Height = pokedexDTO.Height,
                Description = pokedexDTO.Description,
                ImageUrl = imageUrl
            };

            _context.Pokedex.Add(pokedexEntry);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (PokedexEntryExists(pokedexEntry.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction(nameof(GetPokedexEntry), new { id = pokedexEntry.Id }, pokedexEntry);
        }

        // DELETE: api/Pokedex/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePokedexEntry(int id)
        {
            var pokedexEntry = await _context.Pokedex.FindAsync(id);
            if (pokedexEntry == null)
            {
                return NotFound();
            }

            _context.Pokedex.Remove(pokedexEntry);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PokedexEntryExists(int id)
        {
            return _context.Pokedex.Any(e => e.Id == id);
        }
    }
}
