using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBCToolLPSwap.Model
{
    public class ContractModel
    {
        public string Address { get; set; }
        public string ABI { get; set; }
        public int UnitDecimal { get; set; }
        public decimal AmountLiquidity { get; set;}
    }
}
