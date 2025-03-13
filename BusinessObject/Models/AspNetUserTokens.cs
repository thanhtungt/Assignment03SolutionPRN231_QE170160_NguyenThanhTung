/*using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Models
{
    public class AspNetUserTokens : IdentityUserToken<string>
    {
        public string UserId { get; set; }
        public string LoginProvider { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }

        public virtual AspNetUsers User { get; set; }
    }
}
*/
using Microsoft.AspNetCore.Identity;

namespace BusinessObject.Models
{
    public class AspNetUserTokens : IdentityUserToken<string> // Kế thừa từ IdentityUserToken<string>
    {
        // Không cần định nghĩa lại UserId, LoginProvider, Name, Value
        public virtual AspNetUsers User { get; set; }
    }
}