/*using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Models
{

    public class AspNetUsers : IdentityUser
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string NormalizedUserName { get; set; }
        public string Email { get; set; }
        public string NormalizedEmail { get; set; }
        public bool EmailConfirmed { get; set; }
        public string PasswordHash { get; set; }
        public string SecurityStamp { get; set; }
        public string ConcurrencyStamp { get; set; }
        public string PhoneNumber { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public DateTime? LockoutEnd { get; set; }
        public bool LockoutEnabled { get; set; }
        public int AccessFailedCount { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public virtual ICollection<Order> Orders { get; set; }

        public virtual ICollection<AspNetUserClaims> AspNetUserClaims { get; set; }
        public virtual ICollection<AspNetUserLogins> AspNetUserLogins { get; set; }
        public virtual ICollection<AspNetUserTokens> AspNetUserTokens { get; set; }
        public virtual ICollection<AspNetUserRoles> AspNetUserRoles { get; set; }
    }
}
*/
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace BusinessObject.Models
{
    public class AspNetUsers : IdentityUser<string> // Kế thừa từ IdentityUser<string>
    {
        // Chỉ thêm các thuộc tính bổ sung, không định nghĩa lại các thuộc tính của IdentityUser
        //public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        [JsonIgnore]
        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<AspNetUserClaims> AspNetUserClaims { get; set; }
        public virtual ICollection<AspNetUserLogins> AspNetUserLogins { get; set; }
        public virtual ICollection<AspNetUserTokens> AspNetUserTokens { get; set; }
        public virtual ICollection<AspNetUserRoles> AspNetUserRoles { get; set; }
    }
}