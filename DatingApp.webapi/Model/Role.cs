using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace DatingApp.webapi.Model
{
    public class Role: IdentityRole<int>
    {
        public virtual ICollection<UserRole> UserRoles { get; set;}
    }
}