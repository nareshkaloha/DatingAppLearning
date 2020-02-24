using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using DatingApp.webapi.Data;
using DatingApp.webapi.Dto;
using DatingApp.webapi.Helpers;
using DatingApp.webapi.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DatingApp.webapi.Controllers
{
    [Authorize]
    [Route("/api/users/{userId}/photos")]
    [ApiController]
    public class PhotosController : ControllerBase
    {
        private readonly IDatingRepository _repo;
        private readonly IMapper _mapper;
        private readonly IOptions<CloudinarySettings> _options;
        private readonly Cloudinary _cloudinary;
        public PhotosController(
            IDatingRepository repo, 
            IMapper mapper, 
            IOptions<Helpers.CloudinarySettings> options
            )
        {
            _repo = repo;
            _mapper = mapper;
            _options = options;

            var cloudinaryAccount = new Account() 
            {    
                Cloud = _options.Value.CloudName,
                ApiKey = _options.Value.ApiKey,
                ApiSecret = _options.Value.ApiSecret
            };

            _cloudinary = new Cloudinary(cloudinaryAccount);
        }

        [HttpGet("{id}", Name="GetPhoto")]
        public async Task<IActionResult> GetPhoto(int id)
        {
            var photoFromRepo = await _repo.GetPhoto(id);
            var retPhoto = _mapper.Map<PhotoDto>(photoFromRepo);

            return Ok(retPhoto);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePhoto(int userId, [FromForm]PhotForCreateDto photoForCreate)
        {
            if(userId != int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }

            var file = photoForCreate.File;
            var clUploadResult = new ImageUploadResult();

            if(file.Length > 0)
            {
                using( var stream = file.OpenReadStream())
                {
                    var clUploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(file.Name, stream),
                        Transformation = new Transformation().Width(500).Height(500).Crop("fill").Gravity("face")
                    };     

                    clUploadResult = _cloudinary.Upload(clUploadParams);     
                };
            };

            if(clUploadResult!=null)
            {
                photoForCreate.Url = clUploadResult.Uri.ToString();
                photoForCreate.PublicId = clUploadResult.PublicId;                
            }

            var userFromRepo = await _repo.GetUser(userId);

            if(!userFromRepo.Photos.Any(ph=> ph.IsMain))
            {
                photoForCreate.IsMain = true;
            }

            var photo = _mapper.Map<Photo>(photoForCreate);

            userFromRepo.Photos.Add(photo);            

            if(await _repo.SaveAll())
            {
                var photoDto = _mapper.Map<PhotoDto>(photo);
                return CreatedAtRoute("GetPhoto", new { userId = userId, id= photoDto.Id }, photoDto);
            }

            return BadRequest("could not save photo");           
        }    

        [HttpPost]
        [Route("{id}/setMain")]
        public async Task<IActionResult> SetMainPhoto(int userId, int id) 
        {
            if(userId != int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }

            var photoFromRepo = await _repo.GetPhoto(id);

            if(photoFromRepo == null)
            {
                return BadRequest("Photo not found");
            }

            if(photoFromRepo.UserId != userId)
            {
                return Unauthorized();
            }

            var currentMainPhoto = await _repo.GetMainPhotoForUser(userId);
            currentMainPhoto.IsMain = false;
            photoFromRepo.IsMain = true;

            if( await _repo.SaveAll())
            {
                return NoContent();
            }

            return BadRequest();
        }  

        [HttpDelete]
        [Route("{id}")] 
        public async Task<IActionResult> Delete(int userId, int id)
        {
            if(userId != int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }

            var photoFromRepo = await _repo.GetPhoto(id);

            if(photoFromRepo == null)
            {
                return BadRequest("Photo not found");
            }

            if(photoFromRepo.UserId != userId)
            {
                return Unauthorized();
            }

            if(photoFromRepo.IsMain)
            {
                return BadRequest("you can not delete your main photo");
            }

            if(photoFromRepo.PublicId != null)
            {
                var result = _cloudinary.Destroy( new DeletionParams(photoFromRepo.PublicId));

                if(result.Result == "ok")
                {
                    _repo.Delete(photoFromRepo);
                }
            }

            if(photoFromRepo.PublicId == null)
            {
                _repo.Delete(photoFromRepo);
            }

            if(await _repo.SaveAll())
            {
                return Ok();
            }

            return BadRequest("Failed to delete phoo");
        }
    }
}