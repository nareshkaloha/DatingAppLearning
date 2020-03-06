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
       public DbSet<User> Users {get; set;}
       public DbSet<Photo> Photos { get; set;}
       public DbSet<Like> Likes {get; set;}
       public DbSet<Message> Messages { get; set; }
       
       protected override void OnModelCreating(ModelBuilder builder)
       {
           builder.Entity<Like>()
            .HasKey (k => new { k.LikerId, k.LikeeId});

            builder.Entity<Like>()
                .HasOne( h => h.Liker)
                .WithMany(m => m.Likees)
                .HasForeignKey(f => f.LikerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Like>()
                .HasOne(o => o.Likee)
                .WithMany(m => m.Likers)
                .HasForeignKey(f => f.LikeeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Message>()
                .HasOne(o => o.Sender)
                .WithMany(m => m.MessagesSent)
                .HasForeignKey(f => f.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Message>()
                .HasOne(o => o.Recipient)
                .WithMany( m => m.MessagesReceived)
                .HasForeignKey(f => f.RecipientId)
                .OnDelete(DeleteBehavior.Restrict);
       }        
    }
}