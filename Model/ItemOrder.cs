using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DACS_1.Model
{
    public class ItemOrder
    {
        public int Id { get; set; }
        public string name { get; set; }
        public int quantity { get; set; }
        public decimal price_at_order { get; set; }
    }
}
