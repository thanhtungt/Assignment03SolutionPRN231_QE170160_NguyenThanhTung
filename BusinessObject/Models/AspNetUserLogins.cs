/*using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Models
{
    public class AspNetUserLogins : IdentityUserLogin<string>
    {
        public string LoginProvider { get; set; }
        public string ProviderKey { get; set; }
        public string ProviderDisplayName { get; set; }
        public string UserId { get; set; }

        public virtual AspNetUsers User { get; set; }
    }
}
*/
using Microsoft.AspNetCore.Identity;

namespace BusinessObject.Models
{
    public class AspNetUserLogins : IdentityUserLogin<string> // Kế thừa từ IdentityUserLogin<string>
    {
        // Không cần định nghĩa lại LoginProvider, ProviderKey, ProviderDisplayName, UserId
        public virtual AspNetUsers User { get; set; }
    }
}