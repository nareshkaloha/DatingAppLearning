using DatingApp.webapi.Model;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.webapi.Data
{
    public class DataContext : DbContext
    {     
       public DataContext(DbContextOptions<DataContext> options) :base(options)
       {
           
       }

       public DbSet<Value> Values {get; set;}
        
    }
}