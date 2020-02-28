namespace DatingApp.webapi.Helpers
{
    public class PaginationHeaders
    {
        public int CurrentPage { get; set; }
        public int ItemsPerPage { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
        public PaginationHeaders(int pageNumber, int pageSize, int totalCount, int totalPages)
        {
            CurrentPage = pageNumber;
            ItemsPerPage = pageSize;
            TotalItems = totalCount;
            TotalPages = totalPages;            
        }
    }
}