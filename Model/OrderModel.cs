using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DACS_1.Model
{
        public class OrderModel
        {
            public string OrderId { get; set; }
            public string UId { get; set; }

            // Nên dùng DateTime để lưu ngày giờ
            public DateTime OrderDate { get; set; }

            public string UserName { get; set; }
            public bool Status { get; set; }
        }

    
}
