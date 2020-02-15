using System;

namespace DatingApp.webapi.Dto
{
    public class UserForListDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Gender { get; set; }
        public int Age { get; set; }
        public string KnownAs { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime LastActiveDate { get; set; }        
        public string City { get; set; }
        public string Country { get; set; }         
        public string PhotoUrl { get; set; }
    }
}