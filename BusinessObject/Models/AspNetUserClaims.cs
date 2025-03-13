/*using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Models
{
    public class AspNetUserClaims : IdentityUserClaim<string>
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string ClaimType { get; set; }
        public string ClaimValue { get; set; }

        public virtual AspNetUsers User { get; set; }
    }
}
*/
using Microsoft.AspNetCore.Identity;

namespace BusinessObject.Models
{
    public class AspNetUserClaims : IdentityUserClaim<string> // Kế thừa từ IdentityUserClaim<string>
    {
        // Không cần định nghĩa lại Id, UserId, ClaimType, ClaimValue
        public virtual AspNetUsers User { get; set; }
    }
}