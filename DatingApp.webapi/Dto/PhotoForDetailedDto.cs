using System;

namespace DatingApp.webapi.Dto
{
    public class PhotoForDetailedDto
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsMain { get; set; }
        public int UserId { get; set; }
        public bool? IsApproved { get; set; }
    }
}