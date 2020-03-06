using System;

namespace DatingApp.webapi.Model
{
    public class Photo
    {
        public int Id { get; set; }
        public string PublicId { get; set; }
        public string Url { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsMain { get; set; }
        public int UserId { get; set; }
        public virtual User User { get; set; }
    }
}