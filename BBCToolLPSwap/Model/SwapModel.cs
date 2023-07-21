using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBCToolLPSwap.Model
{
    public class SwapModel
    {
        private double slippage;//滑点设置
        private string[] path = new string[2];

        public double Slippage { get => slippage; set => slippage = value; }
        public string[] Path { get => path; set => path = value; }
    }
}
