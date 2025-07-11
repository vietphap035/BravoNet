using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DACS_1.Model
{
    public class StaffModel
    {
        public string UId { get; set; }
        public string FullName { get; set; }
        public decimal basicSalary { get; set; }
        public decimal bonus { get; set; }

        public int workingHours { get; set; }

        public decimal salary
        {
            get
            {
                return basicSalary + bonus + (workingHours * 10000); // Assuming 10,000 VND per hour
            }
        }
    }
}
