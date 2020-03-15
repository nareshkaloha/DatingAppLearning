using Microsoft.AspNetCore.Identity;

namespace DatingApp.webapi.Model
{
    public class UserRole: IdentityUserRole<int>
    {
        public virtual User User{ get; set;}
        public virtual Role Role {get; set;}
        
    }
}