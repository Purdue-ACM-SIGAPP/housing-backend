using Microsoft.AspNetCore.Mvc;
using SimpleWebAppReact.Entities;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using SimpleWebAppReact.Services;

namespace SimpleWebAppReact.Controllers
{
    /// <summary>
    /// Defines endpoints for operations relating the VideoTour table
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class VideoTourController : ControllerBase
    {
        private readonly ILogger<VideoTourController> _logger;
        private readonly IMongoCollection<VideoTour>? _videoTours;
        private readonly IGridFSBucket _videosBucket;

        public VideoTourController(ILogger<VideoTourController> logger, MongoDbService mongoDbService)
        {
            _logger = logger;
            _videoTours = mongoDbService.Database?.GetCollection<VideoTour>("videoTour");
            _videosBucket = new GridFSBucket(mongoDbService.Database);
        }

        /// <summary>
        /// gets videoTours
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<VideoTour>> Get()
        {
            // Fetch the videoTours from the database
            var videoTours = await _videoTours.Find(_ => true).ToListAsync();

            return Ok(videoTours);
        }

        /// <summary>
        /// gets specific videoTour with id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<VideoTour?>> GetById(string id)
        {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            
            var filter = Builders<VideoTour>.Filter.Eq(x => x.Id, id);
            var videoTour = _videoTours.Find(filter).FirstOrDefault();
            
            return videoTour is not null ? Ok(videoTour) : NotFound();
        }

        [HttpGet("{id}/file")]
        public async Task<ActionResult> GetFileById(string id)
        {
            var filter = Builders<VideoTour>.Filter.Eq(x => x.Id, id);
            var videoTour = (await _videoTours.FindAsync(filter)).FirstOrDefault();

            if (videoTour is null)
            {
                return NotFound();
            }

            var videoId = ObjectId.Parse(videoTour.FileId);
            var stream = new MemoryStream();
            await _videosBucket.DownloadToStreamAsync(videoId, stream);
            stream.Seek(0, SeekOrigin.Begin);
            var fileInfo = await _videosBucket
                .Find(Builders<GridFSFileInfo>.Filter.Eq("_id", videoId))
                .FirstOrDefaultAsync();
            if (fileInfo == null)
                return NotFound("File not found.");
            return File(stream, "video/mp4", fileInfo.Filename);
        }

        /// <summary>
        /// adds videoTour entry to table
        /// </summary>
        /// <param name="videoTour"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> Post([FromQuery] VideoTour videoTour, IFormFile file)
        {
            if (file.ContentType != "video/mp4")
            {
                return BadRequest("File must be an mp4");
            }

            var fileStream = file.OpenReadStream();
            var objectId = await _videosBucket.UploadFromStreamAsync(videoTour.Title, fileStream);
            fileStream.Close();

            videoTour.FileId = objectId.ToString();
            
            await _videoTours.InsertOneAsync(videoTour);
            return CreatedAtAction(nameof(GetById), new { id = videoTour.Id }, videoTour);
        }

        /// <summary>
        /// updates a videoTour entry
        /// </summary>
        /// <param name="videoTour"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<ActionResult> Update(VideoTour videoTour)
        {
            var filter = Builders<VideoTour>.Filter.Eq(x => x.Id, videoTour.Id);
            await _videoTours.ReplaceOneAsync(filter, videoTour);
            return Ok();
        }

        /// <summary>
        /// deletes a videoTour entry
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            var filter = Builders<VideoTour>.Filter.Eq(x => x.Id, id);
            var videoTour = (await _videoTours.FindAsync(filter)).FirstOrDefault();

            if (videoTour is null)
            {
                return NotFound();
            }
            
            await _videoTours.DeleteOneAsync(filter);

            var objectId = ObjectId.Parse(videoTour.FileId);
            await _videosBucket.DeleteAsync(objectId);
            
            return Ok();
        }

        /// <summary>
        /// adds a picture to a videoTour entry
        /// </summary>
        /// <param name="id"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost("uploadImage/{id}")]
        public async Task<ActionResult> UploadImage(string id, IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest();
            }
            
            using var ms = new MemoryStream();
            await file.CopyToAsync(ms);
            string base64Image = Convert.ToBase64String(ms.ToArray());
            
            var filter = Builders<VideoTour>.Filter.Eq(v => v.Id, id);
            var update = Builders<VideoTour>.Update.Set(v => v.Image, base64Image);

            var result = await _videoTours.UpdateOneAsync(filter, update);
    
            return result.ModifiedCount > 0 ? Ok() : NotFound();
        }
    }
}

