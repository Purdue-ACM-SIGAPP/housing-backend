namespace SimpleWebAppReact.Controllers;
using Microsoft.AspNetCore.Mvc;
using SimpleWebAppReact.Services;
using SimpleWebAppReact.Entities;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

[ApiController]
    [Route("api/maps")]
    public class MapsController : ControllerBase
    {
        private readonly ILogger<MapsController> _logger;
        private readonly IMongoCollection<Building>? _buildings;

        public MapsController(ILogger<MapsController> logger, MongoDbService mongoDbService)
        {
            _logger = logger;
            _buildings = mongoDbService.Database?.GetCollection<Building>("building");
        }
    }
