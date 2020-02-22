using System;
using Microsoft.AspNetCore.Http;

namespace DatingApp.webapi.Dto
{
    public class PhotForCreateDto
    {    
        public string PublicId { get; set; }
        public string Url { get; set; }
        public string Description { get; set; }
        public IFormFile PhotoFile { get; set; }      
        public bool IsMain { get; set; }       
        public DateTime DateAdded { get; set; }
        public PhotForCreateDto()
        {
            DateAdded = DateTime.Now;            
        }
    }
}