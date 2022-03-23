using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COMP255_Final_Project__2019
{
    public class CarOrderOption
    {
        public int OptionID { get; set; }
        public int OrderNumber { get; set; }


        public string OptionName { get; set; }

        public string OptionDescription { get; set; }

        public decimal OptionPrice { get; set; }
       

        public override string ToString() => $"{OptionID,5} {OptionName,25} {OptionDescription,60} {OptionPrice.ToString("N2"),20}";

    }
}
