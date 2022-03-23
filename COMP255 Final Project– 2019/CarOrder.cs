using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COMP255_Final_Project__2019
{
    public class CarOrder
    {
        public int OrderNumber { get; set; }
        public DateTime OrderDate { get; set; }


        public string CustomerName { get; set; }

        public string CustomerEmail { get; set; }

        public string CustomerAddress { get; set; }
        public bool InProduction { get; set; }
        public  DateTime DeliveryDate { get; set; }


        public override string ToString() => $"{OrderNumber,5} {OrderDate,-25} {CustomerName,-15} {CustomerEmail,-20} {CustomerAddress,-25} {InProduction, -5} {DeliveryDate , -25}";


    }
}
