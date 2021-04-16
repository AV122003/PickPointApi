using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

using PickPointApi.Models;
using Microsoft.Extensions.Caching.Memory;

namespace PickPointApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostomatController : ControllerBase
    {
        private IPostomatRepository postomatRepository;
        private IMemoryCache memoryCache;
        private readonly int timeSpanValue = 5;

        public PostomatController(IMemoryCache memoryCache, IPostomatRepository postomatRepository)
        {
            this.memoryCache = memoryCache;
            this.postomatRepository = postomatRepository;
        }

        [HttpGet("PostomatGet")]
        public async Task<IActionResult> PostomatGet(string postomatNumber)
        {

            if (!postomatNumber.All(char.IsDigit)) //номер постамата, формат которого некорректен
                return BadRequest();

            Postomat postomat = null;

            if (!memoryCache.TryGetValue(postomatNumber, out postomat))
            {
                postomat = postomatRepository.PostomatGet(postomatNumber);
                if (postomat != null)
                {
                    memoryCache.Set(postomat.Number, postomat, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(timeSpanValue)));
                }
            }

            if (postomat == null)
                return NotFound();

            return Ok(await Task.Run(() =>
            {
                return postomat;
            })); // 200 OK    
        }
    }
}
