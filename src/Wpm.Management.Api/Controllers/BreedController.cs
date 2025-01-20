using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using Wpm.Management.Api.DataAccess;

namespace Wpm.Management.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BreedController(ManagementDbContext dbContext, ILogger<BreedController> logger) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> Gets()
        {
            var oBreeds= await dbContext.Breeds.ToListAsync();
            return oBreeds != null ? Ok(oBreeds) : NotFound();
        }

        [HttpGet("{id}", Name =nameof(GetBreedById))]
        public async Task<IActionResult> GetBreedById(int id)
        {
            var oBreed = await dbContext.Breeds.SingleOrDefaultAsync(b => b.Id == id);
            return oBreed != null ? Ok(oBreed) : NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Create(NewBreed oNewBreed)
        {
            try
            {
                var oBreed = oNewBreed.ToBreed();
                await dbContext.AddRangeAsync(oBreed);
                await dbContext.SaveChangesAsync();
                return CreatedAtRoute(nameof(GetBreedById), new { id = oBreed.Id }, oNewBreed);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.ToString());
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

    }

    public record NewBreed(string Name)
    {
        public Breed ToBreed()
        {
            return new Breed(0,Name);
        }
    }
}
