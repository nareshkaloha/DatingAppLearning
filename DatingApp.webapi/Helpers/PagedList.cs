using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatingApp.webapi.Model;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.webapi.Helpers
{
    public class PagedList<T>: List<T> where  T: class
    {
        public int PageNumber { get; }
        public int PageSize { get; }
        public int TotalCount { get; }
        public int TotalPages { get; }
        public PagedList(List<T> items, int pageNumber, int pageSize, int totalCount)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalCount = totalCount;
            TotalPages = (int)System.Math.Ceiling(totalCount/(double)pageSize);
            this.AddRange(items);            
        }    

        public static async Task<PagedList<T>> CreateAsync(IQueryable<T> source, int pageNumber, int pageSize)
        {
            int totalCount = await source.CountAsync();
            var data = await source.Skip((pageNumber -1) * pageSize ).Take(pageSize).ToListAsync();

            return new PagedList<T>(data, pageNumber, pageSize, totalCount);
        }
    }
}