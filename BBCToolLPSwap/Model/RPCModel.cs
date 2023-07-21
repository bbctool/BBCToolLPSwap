using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBCToolLPSwap.Model
{
    public class RPCModel
    {
        public string Name { get; set; }
        public string URL { get; set; }
        public int NetworkId { get; set; }
        public double GasPrice { get; set; }
        public double GasLimit { get; set;}
    }
}
