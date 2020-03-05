namespace DatingApp.webapi.Dto
{
    public class MessageParamsDto
    {
        private  const int MaxPageSize = 100;        
        public int PageNumber { get; set; }  = 1;
        private int pageSize;
        public int PageSize
        {
            get { return pageSize == 0? MaxPageSize: pageSize ; }
            set { pageSize = value > MaxPageSize? MaxPageSize : value; }
        }  
        public int UserId { get; set; }
        public string MessageContainer { get; set; } = "Unread";
    }
}