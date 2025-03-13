/*using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Models
{
    public class AspNetUserRoles : IdentityUserRole<string>
    {
        public string UserId { get; set; }
        public string RoleId { get; set; }

        public virtual AspNetUsers User { get; set; }
        public virtual AspNetRoles Role { get; set; }

        public virtual ICollection<AspNetRoles> AspNetRoles { get; set; }
        public virtual ICollection<AspNetRoleClaims> AspNetRoleClaims { get; set; }
    }
}
*/
using Microsoft.AspNetCore.Identity;

namespace BusinessObject.Models
{
    public class AspNetUserRoles : IdentityUserRole<string> // Kế thừa từ IdentityUserRole<string>
    {
        // Không cần định nghĩa lại UserId, RoleId
        public virtual AspNetUsers User { get; set; }
        public virtual AspNetRoles Role { get; set; }

        // Xóa ICollection<AspNetRoles> và ICollection<AspNetRoleClaims> vì không hợp lý
    }
}