using DatingApp.webapi.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.webapi.Data
{
    public class DataContext : IdentityDbContext<User, Role, int, IdentityUserClaim<int>, UserRole,
        IdentityUserLogin<int>, IdentityRoleClaim<int>, IdentityUserToken<int>>
    {     
       public DataContext(DbContextOptions<DataContext> options) :base(options)
       {
           
       }
       public DbSet<Value> Values {get; set;}
       public DbSet<Photo> Photos { get; set;}
       public DbSet<Like> Likes {get; set;}
       public DbSet<Message> Messages { get; set; }
       
       protected override void OnModelCreating(ModelBuilder builder)
       {
           base.OnModelCreating(builder);

           builder.Entity<UserRole>()
            .HasKey(ur => new { ur.UserId, ur.RoleId});

            builder.Entity<UserRole>()
                .HasOne(ur => ur.User)
                .WithMany(m => m.UserRoles)
                .HasForeignKey(f => f.UserId)
                .IsRequired();

            builder.Entity<UserRole>()
                .HasOne(ur => ur.Role)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(f => f.RoleId)
                .IsRequired();

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