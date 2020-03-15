using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.webapi.Data;
using DatingApp.webapi.Dto;
using DatingApp.webapi.Helpers;
using DatingApp.webapi.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.webapi.Controllers
{
    [ServiceFilter(typeof(LogUserLastActivity))]
    [Route("/api/users/{userId}/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly IDatingRepository _repo;
        private readonly IMapper _mapper;
        public MessagesController(IDatingRepository repo, IMapper mapper)
        {
            this._mapper = mapper;
            this._repo = repo;
        }

        [HttpGet("{id}", Name ="GetMessage")]
        public async Task<IActionResult> GetMessage(int userId, int id)
        {
            if(userId != int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }

            var messageFromRepo = await _repo.GetMessage(id);

            if(messageFromRepo == null)
            {
                return NotFound("message not found in system");
            }

            var messageToReturn = _mapper.Map<MessageToReturnDto>(messageFromRepo);
            return Ok(messageToReturn);
        }

        [HttpGet("thread/{recipientId}")]
        public async Task<IActionResult> GetMessageThread(int userId, int recipientId)
        {
            if(userId != int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }

            var messagesFromrepo = await _repo.GetMessageThread(userId, recipientId);
            var messagesToReturn = _mapper.Map<IEnumerable<MessageToReturnDto>>(messagesFromrepo);

            return Ok(messagesToReturn);
        }

        [HttpGet]
        public async Task<IActionResult> GetMessages(int userId, [FromQuery]MessageParamsDto messageParams)
        {
            if(userId != int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }

            messageParams.UserId = userId;

            var messagesFromRepo = await _repo.GetMessagesForuser(messageParams);

            var messages = _mapper.Map<IEnumerable<MessageToReturnDto>>(messagesFromRepo);

            Response.AddPaginationHeader(new PaginationHeaders(messagesFromRepo.PageNumber, messagesFromRepo.PageSize, 
                                         messagesFromRepo.TotalCount, messagesFromRepo.TotalPages));

            return Ok(messages);
        }

        [HttpPost]
        public async Task<IActionResult> CreateMessage(int userId, MessageForCreationDto messageForCreationDto)
        {
            if(userId != int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }

            messageForCreationDto.SenderId = userId;

            var recipientFromRepo = await _repo.GetUser(messageForCreationDto.RecipientId);

            if(recipientFromRepo == null)
            {
                return NotFound("recipient not found in system");
            }

            var messageForRepo = _mapper.Map<Message>(messageForCreationDto);
            _repo.Add(messageForRepo);

            if(await _repo.SaveAll())
            {
                var messageForReturn = _mapper.Map<MessageForCreationDto>(messageForRepo);

                Response.AddCreatedAtLocation();
                return CreatedAtRoute("GetMessage", new {userId = userId, id= messageForRepo.Id}, messageForReturn);
            }

            return BadRequest("could not create message");
        }

        [HttpPost("{id}")]
        public async Task<IActionResult> DeleteMessage(int id, int userId)
        {
            if(userId != int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }

            var messageFromRepo = await _repo.GetMessage(id); //need a lighter version without navigation properties

            if(messageFromRepo == null)
            {
                return NotFound();
            }

            if(messageFromRepo.RecipientId == userId)
            {
                messageFromRepo.ReceipientDeleted = true;
            }

            if(messageFromRepo.SenderId == userId)
            {
                messageFromRepo.SenderDeleted = true;
            }

            if(messageFromRepo.SenderDeleted && messageFromRepo.ReceipientDeleted)
            {
                _repo.Delete(messageFromRepo);
            }

            if(await _repo.SaveAll())
            {
                return NoContent();
            }

            throw new Exception("could not delete message");
        }

        [HttpPost("{id}/read")]
        public async Task<IActionResult> MarkMessageAsRead(int userId, int id)
        {
            if(userId != int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }

            var messageFromRepo = await _repo.GetMessage(id);

            if(messageFromRepo == null)
                return NotFound();

            if(messageFromRepo.RecipientId != userId)
                return Unauthorized();

            messageFromRepo.IsRead = true;
            messageFromRepo.DateRead = DateTime.Now;

            await _repo.SaveAll();
            return NoContent();
        }
    }
}