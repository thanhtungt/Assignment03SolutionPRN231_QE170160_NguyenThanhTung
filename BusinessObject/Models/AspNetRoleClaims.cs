/*using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Models
{
    public class AspNetRoleClaims : IdentityRoleClaim<string>
    {
        public int Id { get; set; }
        public string RoleId { get; set; }
        public string ClaimType { get; set; }
        public string ClaimValue { get; set; }

        public virtual AspNetRoles Role { get; set; }
    }
}
*/
using Microsoft.AspNetCore.Identity;

namespace BusinessObject.Models
{
    public class AspNetRoleClaims : IdentityRoleClaim<string> // Kế thừa từ IdentityRoleClaim<string>
    {
        // Không cần định nghĩa lại Id, RoleId, ClaimType, ClaimValue
        public virtual AspNetRoles Role { get; set; }
    }
}