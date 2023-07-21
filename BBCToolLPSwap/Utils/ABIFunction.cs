using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace BBCToolLPSwap.Utils
{
    public static class ABIFunction
    {
        [Function("createPair", "address")]
        public class CreatePair : FunctionMessage
        {
            [Parameter("address", "tokenA", 1)]
            public string TokenA { get; set; }
            [Parameter("address", "tokenB", 2)]
            public string TokenB { get; set; }
        }
        [Function("addLiquidity", typeof(AddLiquidityOutputDTOBase))]
        public class AddLiquidity : FunctionMessage
        {
            [Parameter("address", "tokenA", 1)]
            public string TokenA { get; set; }
            [Parameter("address", "tokenB", 2)]
            public string TokenB { get; set; }
            [Parameter("uint", "amountADesired", 3)]
            public BigInteger AmountADesired { get; set; }
            [Parameter("uint", "amountBDesired", 4)]
            public BigInteger AmountBDesired { get; set; }
            [Parameter("uint", "amountAMin", 5)]
            public BigInteger AmountAMin { get; set; }//amountETHMin
            [Parameter("uint", "amountBMin", 6)]
            public BigInteger AmountBMin { get; set; }
            [Parameter("address", "to", 7)]
            public string To { get; set; }
            [Parameter("uint", "deadline", 8)]
            public BigInteger Deadline { get; set; }
        }
        [Function("removeLiquidity", typeof(RemoveLiquidityETHOutputDTOBase))]
        public class RemoveLiquidity : FunctionMessage
        {
            [Parameter("address", "tokenA", 1)]
            public string TokenA { get; set; }
            [Parameter("address", "tokenB", 2)]
            public string TokenB { get; set; }
            [Parameter("uint", "liquidity", 3)]
            public BigInteger Liquidity { get; set; }
            [Parameter("uint", "amountAMin", 4)]
            public BigInteger AmountAMin { get; set; }//amountETHMin
            [Parameter("uint", "amountBMin", 5)]
            public BigInteger AmountBMin { get; set; }
            [Parameter("address", "to", 6)]
            public string To { get; set; }
            [Parameter("uint", "deadline", 7)]
            public BigInteger Deadline { get; set; }
        }
        [FunctionOutput]
        public class AddLiquidityOutputDTOBase : IFunctionOutputDTO//uint amountToken, uint amountETH, uint liquidity
        {
            [Parameter("uint", "amountA", 1)]
            public virtual BigInteger AmountToken { get; set; }
            [Parameter("uint", "amountB", 2)]
            public virtual BigInteger AmountETH { get; set; }
            [Parameter("uint", "liquidity", 3)]
            public virtual BigInteger Liquidity { get; set; }

        }
        [FunctionOutput]
        public class RemoveLiquidityETHOutputDTOBase : IFunctionOutputDTO//uint amountToken, uint amountETH, uint liquidity
        {
            [Parameter("uint", "amountToken", 1)]
            public virtual BigInteger AmountToken { get; set; }
            [Parameter("uint", "amountETH", 2)]
            public virtual BigInteger AmountETH { get; set; }
        }
    }
}
