using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Vega.Controllers.Resources;
using Vega.Core;
using Vega.Core.Models;
using Vega.Persistence;
using IHostingEnvironment = Microsoft.Extensions.Hosting.IHostingEnvironment;

namespace Vega.Controllers
{
    [Route("/api/vehicles/{vehicleId}/photos")]
    public class PhotosController : Controller
    {

        private readonly IWebHostEnvironment _host;
        private readonly IVehicleRepository _repository;
        private readonly IMapper _mapper;
        private readonly IPhotoRepository _photoRepository;
        private readonly IPhotoService _photoService;
        private readonly PhotoSettings photoSettings;

        public PhotosController(IWebHostEnvironment host, IVehicleRepository repository, IMapper mapper, IOptionsSnapshot<PhotoSettings> options, IPhotoRepository photoRepository, IPhotoService photoService)
        {
            this.photoSettings = options.Value;
            _host = host;
            _repository = repository;
            _mapper = mapper;
            _photoRepository = photoRepository;
            _photoService = photoService;
        }

        [HttpGet]
        public async Task<IEnumerable<PhotoResource>> GetPhotos(int vehicleId)
        {
            var photo = await _photoRepository.GetPhotos(vehicleId);

            return _mapper.Map<IEnumerable<Photo>, IEnumerable<PhotoResource>>(photo);
        }

        [HttpPost]
        public async Task<IActionResult> Upload(int vehicleId, IFormFile file)
        {
            var vehicle = await _repository.GetVehicle(vehicleId, includeRelated: false);
            if (vehicle == null)
                return NotFound();

            if (file == null)
                return BadRequest("null file");
            if (file.Length == 0)
                return BadRequest("Empty file");
            if (file.Length > photoSettings.MaxBytes)
                return BadRequest("Maximum file size exceeded");
            if (!photoSettings.IsSupported(file.FileName))
                return BadRequest("Invalid file type");

            var uploadsFolderPath = Path.Combine(_host.WebRootPath, "uploads");

            var photo = await _photoService.UploadPhoto(vehicle, file, uploadsFolderPath);

            return Ok(_mapper.Map<Photo, PhotoResource>(photo));
        }
    }
}
