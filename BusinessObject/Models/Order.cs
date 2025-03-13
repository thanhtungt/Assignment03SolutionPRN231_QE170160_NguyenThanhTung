using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BusinessObject.Models
{
    public class Order 
    {
        public int OrderId { get; set; }
        public string MemberId { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime? RequiredDate { get; set; }
        public DateTime? ShippedDate { get; set; }
        public decimal Freight { get; set; }

        [JsonIgnore]
        public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
        [JsonIgnore]
        public virtual AspNetUsers Member { get; set; }
    }
}
