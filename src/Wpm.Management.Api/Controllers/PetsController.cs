using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using Wpm.Management.Api.DataAccess;

namespace Wpm.Management.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PetsController(ManagementDbContext dbContext, ILogger<PetsController> logger) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var all = await dbContext.Pets.Include(p => p.Breed).ToListAsync();
            return Ok(all);
        }

        [HttpGet("{id}", Name =nameof(GetById))]
        public async Task<IActionResult> GetById(int id)
        {
            var oPet = await dbContext.Pets.Include(p=>p.Breed)
                .Where(p=>p.Id==id)
                .FirstOrDefaultAsync();

            return Ok(oPet);
        }
        [HttpPost]
        public async Task<IActionResult> Create(NewPet oNewPet)
        {
            try
            {
                var oPet = oNewPet.ToPet();
                await dbContext.AddRangeAsync(oPet);
                await dbContext.SaveChangesAsync();
                return CreatedAtRoute(nameof(GetById), new { id = oPet.Id }, oNewPet);
            }catch (Exception ex)
            {
                logger.LogError(ex.Message.ToString());
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

    }

    public record NewPet(string Name, int Age, int BreedId)
    {
        public Pet ToPet()
        {
            return new Pet()
            {
                Name = Name,
                Age = Age,
                BreedId = BreedId
            };
        }
    }
}
