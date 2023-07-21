using BBCToolLPSwap.Utils;
using Nethereum.Contracts;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BBCToolLPSwap.Model
{
    public class Web3Ext
    {
        // uniswap 买卖交易
        public static ResultModel getLPAddress(RPCModel rPCModel, ContractModel factoryContract, ContractModel contractA, ContractModel contractB)
        {
            ResultModel resultModel = new ResultModel();
            try
            {
                Web3 web3 = new Web3(rPCModel.URL);
                Contract myContract = web3.Eth.GetContract(factoryContract.ABI, factoryContract.Address);
                var result = myContract.GetFunction("getPair").CallAsync<String>(contractA.Address, contractB.Address);
                result.Wait();
                resultModel.Message = result.Result;
                if (result.Result == "0x0000000000000000000000000000000000000000")
                {
                    resultModel.State = 0;
                    return resultModel;
                }
                resultModel.State = 1;
                return resultModel;
            }
            catch (Exception ex)
            {
                resultModel.State = -1;
                resultModel.Message = ex.Message;
                Console.WriteLine(ex.Message);
                if (ex.InnerException != null)
                {
                    Console.WriteLine(ex.InnerException.ToString());
                } 
                return resultModel;
            }
        }
        public static ResultModel createPair(RPCModel rPCModel, AccountModel accountModel, ContractModel factoryContract, ContractModel contractA, ContractModel contractB)
        {
            ResultModel resultModel=new ResultModel();
            try
            {
                Account account = new Account(accountModel.PrivateKey, rPCModel.NetworkId);
                Web3 web3 = new Web3(account, rPCModel.URL);
                Contract myContract = web3.Eth.GetContract(factoryContract.ABI, factoryContract.Address);
                BigInteger gPrice = Web3.Convert.ToWeiFromUnit(rPCModel.GasPrice, 1000000000);
                BigInteger gasLimit = new BigInteger(rPCModel.GasLimit);
                TransactionInput transactionInput = new TransactionInput
                {
                    Data = "",
                    Gas = new HexBigInteger(gasLimit),
                    From = account.Address,
                    Value = new HexBigInteger(0),
                    GasPrice = new HexBigInteger(gPrice)
                };
                var swapResult = myContract.GetFunction("createPair").SendTransactionAsync(transactionInput, contractA.Address, contractB.Address);
                swapResult.Wait();
                int count = 0;
                resultModel.TransHash = swapResult.Result;
                while (true)
                {
                    var receipt = web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(swapResult.Result);
                    receipt.Wait();
                    if (receipt.Result != null)
                    {
                        resultModel.State = (int)receipt.Result.Status.ToUlong();
                        break;
                    }
                    if (count >= 20)
                    {
                        resultModel.State = 2;
                        break;
                    }
                    Thread.Sleep(2000);
                    count++;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                if (ex.InnerException != null)
                {
                    Console.WriteLine(ex.InnerException.ToString());
                }
                resultModel.Message = ex.Message;
            }
            return resultModel;
        }
        //获得代币余额
        public static ResultModel getTokenBalance(RPCModel rPCModel, ContractModel contractModel, string address)// string ABI, string contractAdd, int unitDecimal, string address)
        {
            ResultModel resultModel = new ResultModel();
            try
            {
                Web3 web3 = new Web3(rPCModel.URL);
                Contract voteContract = web3.Eth.GetContract(contractModel.ABI, contractModel.Address);
                var balanceResult = voteContract.GetFunction("balanceOf").CallAsync<BigInteger>(address);
                balanceResult.Wait();
                decimal balanceEther = Web3.Convert.FromWei(balanceResult.Result, contractModel.UnitDecimal);
                //balanceEther = decimal.Round(balanceEther, 18, MidpointRounding.AwayFromZero);
                resultModel.State = 1;
                resultModel.DecimalValue = balanceEther;
                return resultModel;
            }
            catch (Exception ex)
            {
                resultModel.State = -1;
                resultModel.Message = ex.Message;
                if (ex.InnerException != null)
                {
                    resultModel.Message=ex.InnerException.ToString();
                }
                return resultModel;
            }
        }

        public static ResultModel addLiquidity(RPCModel rPCModel, AccountModel accountModel, ContractModel routerContract, ContractModel contractA, ContractModel contractB)
        {
            ResultModel resultModel = new ResultModel();
            try
            {
                Account account = new Account(accountModel.PrivateKey, rPCModel.NetworkId);
                Web3 web3 = new Web3(account, rPCModel.URL);
                Contract myContract = web3.Eth.GetContract(routerContract.ABI, routerContract.Address);
                BigInteger gPrice = Web3.Convert.ToWeiFromUnit(rPCModel.GasPrice, 1000000000);
                BigInteger gasLimit = new BigInteger(rPCModel.GasLimit);
                BigInteger amountADesired = Web3.Convert.ToWei(contractA.AmountLiquidity, contractA.UnitDecimal);
                BigInteger amountBDesired = Web3.Convert.ToWei(contractB.AmountLiquidity, contractB.UnitDecimal);
                TransactionInput transactionInput = new TransactionInput
                {
                    Data = "",
                    Gas = new HexBigInteger(gasLimit),
                    From = account.Address,
                    Value = new HexBigInteger(0),
                    GasPrice = new HexBigInteger(gPrice)
                };
                var swapResult = myContract.GetFunction("addLiquidity").SendTransactionAsync(transactionInput, contractA.Address, contractB.Address, amountADesired, amountBDesired, 0, 0, account.Address,GetSeconds(DateTime.Now));
                swapResult.Wait();
                int count = 0;
                resultModel.TransHash = swapResult.Result;
                while (true)
                {
                    var receipt = web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(swapResult.Result);
                    receipt.Wait();
                    if (receipt.Result != null)
                    {
                        resultModel.State = (int)receipt.Result.Status.ToUlong();
                        break;
                    }
                    if (count >= 20)
                    {
                        resultModel.State = 2;
                        break;
                    }
                    Thread.Sleep(2000);
                    count++;
                }
            }
            catch (Exception ex)
            {
                resultModel.State = -1;
                resultModel.Message = ex.Message;
                if (ex.InnerException != null)
                {
                    resultModel.Message=ex.InnerException.Message;
                }
            }
            return resultModel;
        }
        public static ResultModel tokenApprove(RPCModel rPCModel, AccountModel accountEntity, ContractModel contractModel, decimal amount, string approveAddress)
        {
            ResultModel resultModel = new ResultModel();
            try
            {
                Account account = new Account(accountEntity.PrivateKey, rPCModel.NetworkId);
                Web3 web3 = new Web3(account, rPCModel.URL);
                Contract voteContract = web3.Eth.GetContract(contractModel.ABI, contractModel.Address);
                BigInteger gPrice = Web3.Convert.ToWeiFromUnit(rPCModel.GasPrice, 1000000000);
                BigInteger gasLimit = new BigInteger(rPCModel.GasLimit);
                gasLimit = gasLimit + gasLimit * 10 / 100;
                voteContract.Eth.TransactionManager.DefaultGas = gasLimit;
                voteContract.Eth.TransactionManager.DefaultGasPrice = gPrice;// gasPrice.Result.Value;            
                BigInteger countEth = Web3.Convert.ToWei(amount,contractModel.UnitDecimal);
                TransactionInput transactionInput = new TransactionInput
                {
                    Data = "",
                    Gas = new HexBigInteger(gasLimit),
                    From = account.Address,
                    Value = new HexBigInteger(0),
                    GasPrice = new HexBigInteger(gPrice)
                };
                var sendResult = voteContract.GetFunction("approve").SendTransactionAsync(transactionInput, approveAddress, countEth);
                sendResult.Wait();
                //请求交易回执               
                int count = 0;
                while (true)
                {
                    try
                    {
                        resultModel.TransHash = sendResult.Result;
                        var receipt = web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(sendResult.Result);
                        receipt.Wait();
                        if (receipt.Result != null)
                        {
                            resultModel.State = (int)receipt.Result.Status.ToUlong();
                            break;
                        }
                        if (count >= 20)
                        {
                            resultModel.Message = "获取授权回执超时！";
                            resultModel.State = 2;
                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                        
                    }
                    Thread.Sleep(2000);
                    count++;
                }
            }
            catch (Exception ex)
            {
                resultModel.State = -1;
                resultModel.Message = ex.Message;
                if (ex.InnerException != null)
                {
                    resultModel.Message = ex.InnerException.Message;
                }
            }
            return resultModel;

        }
        public static ResultModel getAllowance(RPCModel rPCModel, AccountModel accountModel, ContractModel contract, string spender)
        {
            ResultModel resultModel = new ResultModel();
            try
            {
                Web3 web3 = new Web3(rPCModel.URL);
                Contract voteContract = web3.Eth.GetContract(contract.ABI, contract.Address);
                object[] arguments = new object[1];
                arguments[0] = spender;
                var sendResult = voteContract.GetFunction("allowance").CallAsync<BigInteger>(accountModel.Address, spender);
                sendResult.Wait();
                decimal ethCount = Web3.Convert.FromWei(sendResult.Result, contract.UnitDecimal);
                resultModel.State = 1;
                resultModel.Message = "获取授权成功!";
                resultModel.DecimalValue = ethCount;
                return resultModel;
            }
            catch (Exception ex)
            {
                resultModel.State = -1;
                resultModel.Message = ex.Message;
                if (ex.InnerException != null)
                {
                    resultModel.Message += ex.InnerException.Message;
                }
                return resultModel;
            }
        }
        public static ResultModel removeLiquidity(RPCModel rPCModel, AccountModel accountEntity, ContractModel routerContract, ContractModel contractLP, ContractModel contractA, ContractModel contractB)
        {
            ResultModel resultModel = new ResultModel();
            try
            {
                Account account = new Account(accountEntity.PrivateKey, rPCModel.NetworkId);
                Web3 web3 = new Web3(account, rPCModel.URL);
                Contract myContract = web3.Eth.GetContract(routerContract.ABI, routerContract.Address);
                BigInteger gPrice = Web3.Convert.ToWeiFromUnit(rPCModel.GasPrice, 1000000000);
                BigInteger gasLimit = new BigInteger(rPCModel.GasLimit);
                BigInteger liquidity = Web3.Convert.ToWei(contractLP.AmountLiquidity, contractLP.UnitDecimal);
                TransactionInput transactionInput = new TransactionInput
                {
                    Data = "",
                    Gas = new HexBigInteger(gasLimit),
                    From = account.Address,
                    Value = new HexBigInteger(0),
                    GasPrice = new HexBigInteger(gPrice)
                };
                var swapResult = myContract.GetFunction("removeLiquidity").SendTransactionAsync(transactionInput, contractA.Address, contractB.Address, liquidity, 0, 0, account.Address,GetSeconds(DateTime.Now));
                swapResult.Wait();
                int count = 0;
                resultModel.TransHash = swapResult.Result;
                resultModel.Message = "LP撤出成功!";
                while (true)
                {
                    var receipt = web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(swapResult.Result);
                    receipt.Wait();
                    if (receipt.Result != null)
                    {
                        resultModel.State = (int)receipt.Result.Status.ToUlong();
                        break;
                    }
                    if (count >= 20)
                    {
                        resultModel.State = 2;
                        break;
                    }
                    Thread.Sleep(2000);
                    count++;
                }
            }
            catch (Exception ex)
            {
                resultModel.State = -1;
                resultModel.Message=ex.Message;
                if (ex.InnerException != null)
                {
                    resultModel.Message=ex.Message+ex.InnerException.ToString();
                }
            }
            return resultModel;
        }


        public static ResultModel swapExactTokensForTokens(RPCModel rPCModel, AccountModel accountEntity, ContractModel routerContract, ContractModel contractA, ContractModel contractB, SwapModel swapModel)
        {
            ResultModel resultModel = new ResultModel();
            try
            {
                Account account = new Account(accountEntity.PrivateKey, rPCModel.NetworkId);
                Web3 web3 = new Web3(account, rPCModel.URL);
                Contract myContract = web3.Eth.GetContract(routerContract.ABI, routerContract.Address);
                BigInteger amountIn = 0;
                amountIn = Web3.Convert.ToWei(contractB.AmountLiquidity, contractB.UnitDecimal);
                BigInteger gPrice = Web3.Convert.ToWeiFromUnit(rPCModel.GasPrice, 1000000000);
                var amountResult = myContract.GetFunction("getAmountsOut").CallDecodingToDefaultAsync(amountIn, swapModel.Path);
                amountResult.Wait();
                System.Collections.Generic.List<BigInteger> list = (System.Collections.Generic.List<BigInteger>)amountResult.Result[0].Result;
                decimal decOut = 0;
                BigInteger amountOutMin = 0;//
                decOut = (decimal)Web3.Convert.FromWei((BigInteger)list[1], contractA.UnitDecimal) * (decimal)(1 - swapModel.Slippage / 100);
                amountOutMin = Web3.Convert.ToWei(decOut, contractA.UnitDecimal);
                BigInteger gasLimit=new BigInteger(rPCModel.GasLimit);
                TransactionInput transactionInput = new TransactionInput
                {
                    Data = "",
                    Gas = new HexBigInteger(gasLimit),
                    From = account.Address,
                    Value = new HexBigInteger(0),
                    GasPrice = new HexBigInteger(gPrice)
                };
                var swapResult = myContract.GetFunction("swapExactTokensForTokens").SendTransactionAsync(transactionInput, amountIn, amountOutMin, swapModel.Path, account.Address, GetSeconds(DateTime.Now));
                swapResult.Wait();
                int count = 0;
                resultModel.TransHash = swapResult.Result;
                while (true)
                {
                    var receipt = web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(swapResult.Result);
                    receipt.Wait();
                    if (receipt.Result != null)
                    {
                        resultModel.State = (int)receipt.Result.Status.ToUlong();
                        if (resultModel.State == 1)
                        {
                            resultModel.Message = "Swap交易成功!";
                        }
                        else
                        {
                            resultModel.Message = "Swap交易失败!";
                        }
                        break;
                    }
                    if (count >= 20)
                    {
                        resultModel.State = 2;
                        resultModel.Message = "获取交易hash超时!";
                        break;
                    }
                    Thread.Sleep(2000);
                    count++;
                }
            }
            catch (Exception ex)
            {
                resultModel.State =-1;
                resultModel.Message= ex.Message;
                if (ex.InnerException != null)
                {
                    resultModel.Message += ex.InnerException.Message;
                }
            }
            return resultModel;
        }
        public static ResultModel swapExactTokensForTokensSupportingFeeOnTransferTokens(RPCModel rPCModel, AccountModel accountModel, ContractModel routerContract, ContractModel contractA, ContractModel contractB, SwapModel swapModel)
        {
            ResultModel resultModel = new ResultModel();
            try
            {
                Account account = new Account(accountModel.PrivateKey, rPCModel.NetworkId);
                Web3 web3 = new Web3(account, rPCModel.URL);
                Contract myContract = web3.Eth.GetContract(routerContract.ABI, routerContract.Address);
                BigInteger amountIn =  Web3.Convert.ToWei(contractA.AmountLiquidity, contractA.UnitDecimal);
                BigInteger gPrice = Web3.Convert.ToWeiFromUnit(rPCModel.GasPrice, 1000000000);
                var amountResult = myContract.GetFunction("getAmountsOut").CallDecodingToDefaultAsync(amountIn, swapModel.Path);
                amountResult.Wait();
                System.Collections.Generic.List<BigInteger> list = (System.Collections.Generic.List<BigInteger>)amountResult.Result[0].Result;
                decimal decOut = 0;
                BigInteger amountOutMin = 0;
                decOut = (decimal)Web3.Convert.FromWei((BigInteger)list[1], contractB.UnitDecimal) * (decimal)(1 - swapModel.Slippage / 100);
                amountOutMin = Web3.Convert.ToWei(decOut, contractB.UnitDecimal);
                BigInteger gasLimit = new BigInteger(rPCModel.GasLimit);
                TransactionInput transactionInput = new TransactionInput
                {
                    Data = "",
                    Gas = new HexBigInteger(gasLimit),
                    From = account.Address,
                    Value = new HexBigInteger(0),
                    GasPrice = new HexBigInteger(gPrice)
                };
                var swapResult = myContract.GetFunction("swapExactTokensForTokensSupportingFeeOnTransferTokens").SendTransactionAsync(transactionInput, amountIn, amountOutMin, swapModel.Path, account.Address, GetSeconds(DateTime.Now));
                swapResult.Wait();
                int count = 0;
                resultModel.TransHash = swapResult.Result;
                while (true)
                {
                    try
                    {
                        var receipt = web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(swapResult.Result);
                        receipt.Wait();
                        if (receipt.Result != null)
                        {
                            resultModel.State = (int)receipt.Result.Status.ToUlong();
                            if (resultModel.State == 1)
                            {
                                resultModel.Message = "Swap交易成功!";
                            }
                            else
                            {
                                resultModel.Message = "Swap交易失败!";
                            }
                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                    if (count >= 20)
                    {
                        resultModel.State = 2;
                        resultModel.Message = "获取交易hash超时!";
                        break;
                    }
                    Thread.Sleep(2000);
                    count++;
                }
                return resultModel;
            }
            catch (Exception ex)
            {
                resultModel.State = -1;
                resultModel.Message= "交易失败:" + ex.Message;
                if (ex.InnerException != null)
                {
                    resultModel.Message += ex.InnerException.Message;
                }
                return resultModel;
            }
        }
        public static ResultModel getLPReserves(RPCModel rPCModel, ContractModel lPContract, ContractModel contractA, ContractModel contractB)
        {
            ResultModel resultModel = new ResultModel();
            try
            {
                Web3 web3 = new Web3(rPCModel.URL);
                Contract myContract = web3.Eth.GetContract(lPContract.ABI, lPContract.Address);
                var reservesResult = myContract.GetFunction("getReserves").CallDecodingToDefaultAsync();// .CallAsync<BigInteger>();
                reservesResult.Wait();
                if ((BigInteger)reservesResult.Result[0].Result > 0)
                {
                    decimal reserves0;// = Web3.Convert.FromWei((BigInteger)reservesResult.Result[0].Result);
                    decimal reserves1;// = Web3.Convert.FromWei((BigInteger)reservesResult.Result[1].Result);
                    reserves0 = Web3.Convert.FromWei((BigInteger)reservesResult.Result[0].Result, contractA.UnitDecimal);
                    reserves1 = Web3.Convert.FromWei((BigInteger)reservesResult.Result[1].Result, contractB.UnitDecimal);
                    resultModel.TokenAReserves = reserves0;
                    resultModel.TokenBReserves = reserves1;
                    resultModel.State = 1;
                }
            }
            catch (Exception ex)
            {
                resultModel.State = -1;
                resultModel.Message = ex.Message;
            }
            return resultModel;
        }
        public static long GetSeconds(DateTime dt)
        {
            long currentTicks = dt.Ticks;
            DateTime dtFrom = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            long ms = dtFrom.Millisecond;
            long ticks = (currentTicks - dtFrom.Ticks);
            long currentMillis = (currentTicks - dtFrom.Ticks) / 10000 / 1000;
            return currentMillis;
        }
    }
}
