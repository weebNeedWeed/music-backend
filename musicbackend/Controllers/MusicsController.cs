using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using musicbackend.Data;
using musicbackend.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace musicbackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MusicsController : ControllerBase
    {
        private readonly MusicContext _context;
        private readonly IConfiguration _configuration;
        private readonly HttpClient client = new HttpClient();

        public MusicsController(MusicContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Music>> AddMusic([FromBody] Music musicData)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            int userId = Int32.Parse(User.Claims.First(x => x.Type == "Id").Value); ;

            Music music = new Music
            {
                Name = musicData.Name,
                UploaderName = musicData.UploaderName,
                ImageUrl = musicData.ImageUrl,
                YoutubeUrl = musicData.YoutubeUrl,
                UserId = userId
            };

            try
            {
                await _context.Musics.AddAsync(music);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                return BadRequest();
            }

            return music;
        }

        [HttpGet]
        [Route("{Id:int}")]
        [Authorize]
        public async Task<ActionResult<Music>> GetMusicById([FromRoute] int Id)
        {
            int userId = Int32.Parse(User.Claims.First(x => x.Type == "Id").Value);

            Music music = await _context.Musics.FirstOrDefaultAsync(x => x.MusicId == Id && x.UserId == userId);

            if (music == null)
            {
                return NotFound();
            }

            return music;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Music>>> GetAllMusics([FromQuery] string search)
        {
            int userId = Int32.Parse(User.Claims.First(x => x.Type == "Id").Value);

            var query = _context.Musics.Where(x => x.UserId == userId);

            if (search != null)
            {
                query = query.Where(x => x.Name.Contains(search));
            }

            List<Music> musics = await query.ToListAsync();

            return musics;
        }

        [HttpGet]
        [Route("youtube")]
        public async Task<ActionResult<IEnumerable<Music>>> SearchYoutube([FromQuery] string search, [FromQuery] int limit = 10)
        {
            if (search == null)
            {
                return BadRequest();
            }

            string apiKey = _configuration["ApiKey"];
            string apiUrl = $"https://www.googleapis.com/youtube/v3/search?part=snippet&maxResults={limit}&q={search}&type=video&key={apiKey}";

            JObject response = JObject.Parse(await client.GetStringAsync(apiUrl));

            List<Music> musics = new List<Music>();

            foreach (var item in response["items"])
            {
                string name = (string) item["snippet"]["title"];
                string url = (string) item["snippet"]["thumbnails"]["medium"]["url"];
                string uploader = (string) item["snippet"]["channelTitle"];
                string id = (string) item["id"]["videoId"];

                Music music = new Music
                {
                    Name = name,
                    YoutubeUrl = $"https://www.youtube.com/watch?v={id}",
                    UploaderName = uploader,
                    ImageUrl = url
                };

                musics.Add(music);
            }

            return musics;
        }

        [HttpDelete]
        [Route("{Id:int}")]
        [Authorize]
        public async Task<IActionResult> DeleteMusicById([FromRoute]int Id)
        {
            int userId = Int32.Parse(User.Claims.First(x => x.Type == "Id").Value);
            Music music = await _context.Musics.Where(x => x.MusicId == Id && x.UserId == userId).FirstOrDefaultAsync();
            
            if(music == null)
            {
                return NotFound();
            }

            try
            {
                _context.Musics.Remove(music);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
            catch (Exception)
            {
                throw;
            }

            return Ok();
        }
    }
}
