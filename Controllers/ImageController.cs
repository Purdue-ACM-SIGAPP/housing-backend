using Microsoft.AspNetCore.Mvc;
using SimpleWebAppReact.Entities;
using MongoDB.Driver;
using SimpleWebAppReact.Services;
using MongoDB.Bson;

namespace SimpleWebAppReact.Controllers
{
    /// <summary>
    /// Defines endpoints for operations relating the Image table
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ImageController : ControllerBase
    {
        private readonly ILogger<ImageController> _logger;
        private readonly IMongoCollection<Image>? _images;

        public ImageController(ILogger<ImageController> logger, MongoDbService mongoDbService)
        {
            _logger = logger;
            _images = mongoDbService.Database?.GetCollection<Image>("image");
        }

        /// <summary>
        /// gets Images, with optional query parameters
        /// </summary>
        /// <param name="imageData"></param>
        /// <param name="description"></param>
        /// <param name="dateTaken"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IEnumerable<Image>> Get([FromQuery] BsonBinaryData? imageData = null, [FromQuery] string? description = null, [FromQuery] DateTime? dateTaken = null)
        {
            // Build the filter using a filter builder
            var filterBuilder = Builders<Image>.Filter;
            var filter = FilterDefinition<Image>.Empty;

            // Apply the image filter if the parameter is provided
            if (imageData != null)
            {
                filter &= filterBuilder.Eq(b => b.ImageData, imageData);
            }

            // Apply the description filter if the parameter is provided
            if (!string.IsNullOrEmpty(description))
            {
                filter &= filterBuilder.Eq(b => b.Description, description);
            }

            // Apply the dateTaken filter if the parameter is provided
            if (dateTaken != null)
            {
                filter &= filterBuilder.Eq(b => b.DateTaken, dateTaken);
            }

            // Fetch the Images from the database using the filter
            return await _images.Find(filter).ToListAsync();
        }

        /// <summary>
        /// gets specific Image with id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Image?>> GetById(string id)
        {
            // Simple validation to check if the ID is not null
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("Invalid ID format.");
            }
            
            var filter = Builders<Image>.Filter.Eq(x => x.Id, id);
            var image = _images.Find(filter).FirstOrDefault();
            return image is not null ? Ok(image) : NotFound();
        }

        /// <summary>
        /// adds Image entry to table
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> Post(Image image)
        {
            await _images.InsertOneAsync(image);
            return CreatedAtAction(nameof(GetById), new { id = image.Id }, image);
            
        }

        /// <summary>
        /// updates a Image entry
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<ActionResult> Update(Image image)
        {
            var filter = Builders<Image>.Filter.Eq(x => x.Id, image.Id);
            await _images.ReplaceOneAsync(filter, image);
            return Ok();
        }

        /// <summary>
        /// deletes a Image entry
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            var filter = Builders<Image>.Filter.Eq(x => x.Id, id);
            await _images.DeleteOneAsync(filter);
            return Ok();
        }
    }
}

