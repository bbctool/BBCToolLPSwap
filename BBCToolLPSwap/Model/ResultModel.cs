using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBCToolLPSwap.Model
{
    public class ResultModel
    {
        private string transHash;
        private string message;
        private int state;
        private decimal decimalValue;
        private decimal tokenAReserves;
        private decimal tokenBReserves;
        public string TransHash { get => transHash; set => transHash = value; }
        public string Message { get => message; set => message = value; }
        public int State { get => state; set => state = value; }
        public decimal DecimalValue { get => decimalValue; set => decimalValue = value; }
        public decimal TokenAReserves { get => tokenAReserves; set => tokenAReserves = value; }
        public decimal TokenBReserves { get => tokenBReserves; set => tokenBReserves = value; }
    }
}
