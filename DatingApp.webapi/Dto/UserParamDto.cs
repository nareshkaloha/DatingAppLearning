namespace DatingApp.webapi.Dto
{
    public class UserParamsDto
    {
        private  const int MaxPageSize = 100;        
        public int PageNumber { get; set; }  = 1;
        private int pageSize;
        public int PageSize
        {
            get { return pageSize == 0? MaxPageSize: pageSize ; }
            set { pageSize = value > MaxPageSize? MaxPageSize : value; }
        }  

        public int MinAge { get; set; } = 18;          
        public int MaxAge { get; set; } = 99;
        public int UserId { get; set; }
        public string Gender { get; set; }
        public string OrderBy { get; set; }
    }
}