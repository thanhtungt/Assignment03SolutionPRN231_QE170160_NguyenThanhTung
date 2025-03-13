using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BusinessObject.Models
{
    public class Category 
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }

        [JsonIgnore]
        public virtual ICollection<Product> Products { get; set; }
    }
}
