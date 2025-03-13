using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace BusinessObject.Models
{
    public class AspNetRoles : IdentityRole<string> // Kế thừa từ IdentityRole<string>
    {
        // Không cần định nghĩa lại Id, Name, NormalizedName, ConcurrencyStamp
        public virtual ICollection<AspNetUserRoles> AspNetUserRoles { get; set; }
        public virtual ICollection<AspNetRoleClaims> AspNetRoleClaims { get; set; }
    }
}