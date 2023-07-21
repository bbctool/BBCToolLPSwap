using BBCToolLPSwap.Model;
using BBCToolLPSwap.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BBCToolLPSwap
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private RPCModel rPCModel = new RPCModel();
        private ContractModel routerContract = new ContractModel();
        private ContractModel factoryContract = new ContractModel();
        private ContractModel lpModel = new ContractModel();
        private ContractModel contractToken0 = new ContractModel();
        private ContractModel contractToken1 = new ContractModel();
        private AccountModel accountModel = new AccountModel();
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string curPath= System.Environment.CurrentDirectory;
            rPCModel.Name = "币安测试链";
            rPCModel.URL = this.textBoxUrl.Text;// "https://data-seed-prebsc-1-s2.binance.org:8545/";
            rPCModel.NetworkId = Convert.ToInt32(this.textBoxNetworkId.Text);
            routerContract.Address = this.textBoxRouter.Text;
            factoryContract.Address = this.textBoxFactory.Text;
            factoryContract.ABI=FileHelper.ReadFile(curPath + "//UniswapV2Factory.json");
            routerContract.ABI = FileHelper.ReadFile(curPath + "//UniswapV2Router.json");
            lpModel.ABI= FileHelper.ReadFile(curPath + "//UniswapV2LP.json");
            this.lpModel.UnitDecimal = 18;
            this.contractToken0.ABI=lpModel.ABI;
            this.contractToken1.ABI=lpModel.ABI;
        }

        private void GetPair_Click(object sender, RoutedEventArgs e)
        {
            this.factoryContract.Address = this.textBoxFactory.Text;
            this.contractToken0.Address = this.textBoxToken0.Text;
            this.contractToken1.Address = this.textBoxToken1.Text;
            ResultModel resultModel =  Web3Ext.getLPAddress(this.rPCModel, this.factoryContract, this.contractToken0, this.contractToken1);
            this.textBoxLP.Text = resultModel.Message;
        }

        private void CreatePair_Click(object sender, RoutedEventArgs e)
        {
            this.factoryContract.Address = this.textBoxFactory.Text;
            this.accountModel.PrivateKey = textBoxPrivateKey.Text;
            this.contractToken0.Address= this.textBoxToken0.Text;
            this.contractToken1.Address= this.textBoxToken1.Text;
            this.rPCModel.GasPrice = Convert.ToDouble(this.textBoxGasPrice.Text);
            this.rPCModel.GasLimit=Convert.ToDouble(this.textBoxGasLimit.Text);
            ResultModel resultModel = Web3Ext.createPair(this.rPCModel,this.accountModel, this.factoryContract, contractToken0, this.contractToken1);
            if (resultModel.State == 1)
            {
                resultModel = Web3Ext.getLPAddress(this.rPCModel, this.factoryContract, this.contractToken0, this.contractToken1);
                this.textBoxLP.Text = resultModel.Message;
            }
            else
            {
                this.textBoxLP.Text = resultModel.Message;
            }
           
        }

        private void GetLPBalance_Click(object sender, RoutedEventArgs e)
        {
            this.rPCModel.URL = this.textBoxUrl.Text;
            this.rPCModel.NetworkId=Convert.ToInt32(this.textBoxNetworkId.Text);
            this.lpModel.UnitDecimal = 18;
            if(string.IsNullOrEmpty(this.textBoxLP.Text))
            {
                MessageBox.Show("lp地址不能为空，请点击查看LP地址后操作!", "提示");
                return;
            }
            this.lpModel.Address=this.textBoxLP.Text;
             ResultModel resultModel = Web3Ext.getTokenBalance(this.rPCModel, this.lpModel, this.textBoxAccountAddress.Text);
            if(resultModel.State == 1)
            {
                textBoxLPBalance.Text = resultModel.DecimalValue.ToString();
            }
            else
            {
                textBoxLPBalance.Text = resultModel.Message;
            }
        }

        //添加流动性
        private void AddLP_Click(object sender, RoutedEventArgs e)
        {
            this.rPCModel.GasPrice = Convert.ToDouble(this.textBoxGasPrice.Text);
            this.rPCModel.GasLimit = Convert.ToDouble(this.textBoxGasLimit.Text);
            this.accountModel.PrivateKey = this.textBoxPrivateKey.Text;
            this.routerContract.Address=this.textBoxRouter.Text;
            this.contractToken0.Address = this.textBoxToken0.Text;
            this.contractToken0.UnitDecimal = Convert.ToInt32(this.textBoxToken0Decimals.Text);
            this.contractToken1.Address = this.textBoxToken1.Text;
            this.contractToken0.AmountLiquidity = Convert.ToDecimal(this.textBoxToken0AddAmount.Text);
            this.contractToken1.Address = this.textBoxToken1.Text;
            this.contractToken1.UnitDecimal = Convert.ToInt32(this.textBoxToken1Decimals.Text);
            this.contractToken1.AmountLiquidity = Convert.ToDecimal(this.textBoxToken1AddAmount.Text);
            if (string.IsNullOrEmpty(this.textBoxLP.Text))
            {
                MessageBox.Show("lp地址不能为空，请点击查看LP地址后操作!", "提示");
                return;
            }
            ResultModel resultModel = Web3Ext.getAllowance(this.rPCModel, this.accountModel, this.contractToken0, this.routerContract.Address);
            if (resultModel.State == 1)
            {
                if (resultModel.DecimalValue < this.contractToken0.AmountLiquidity)
                {
                    resultModel = Web3Ext.tokenApprove(this.rPCModel, this.accountModel, this.contractToken0, 1000000000000000, this.routerContract.Address);
                    textBoxLPResult.Text = resultModel.Message;
                    if (resultModel.State != 1)
                    {
                        MessageBox.Show("授权失败!", "提示");
                        return;
                    }
                }
            }
            else
            {
                textBoxLPResult.Text = resultModel.Message;
            }
            resultModel = Web3Ext.getAllowance(this.rPCModel, this.accountModel, this.contractToken1, this.routerContract.Address);
            if (resultModel.State == 1)
            {
                if (resultModel.DecimalValue < this.contractToken1.AmountLiquidity)
                {
                    resultModel = Web3Ext.tokenApprove(this.rPCModel, this.accountModel, this.contractToken1, 1000000000000000, this.routerContract.Address);
                    textBoxLPResult.Text = resultModel.Message;
                    if(resultModel.State != 1)
                    {
                        MessageBox.Show("授权失败!", "提示");
                        return;
                    }
                }
            }
            else
            {
                textBoxLPResult.Text = resultModel.Message;
            }
            resultModel = Web3Ext.addLiquidity(this.rPCModel, this.accountModel,this.routerContract, this.contractToken0,this.contractToken1);
            if (resultModel.State == 1)
            {
                textBoxLPResult.Text = "成功!";
                this.lpModel.Address = this.textBoxLP.Text;
                resultModel = Web3Ext.getTokenBalance(this.rPCModel, this.lpModel, this.textBoxAccountAddress.Text);
                if (resultModel.State == 1)
                {
                    textBoxLPBalance.Text = resultModel.DecimalValue.ToString();
                }
                else
                {
                    textBoxLPResult.Text = resultModel.Message;
                }
            }
            else
            {
                textBoxLPResult.Text = resultModel.Message;
            }
        }
        //冒泡排序 
                    
        



        //撤销流动性
        private void RemoveLP_Click(object sender, RoutedEventArgs e)
        {
            this.rPCModel.URL = this.textBoxUrl.Text;
            this.rPCModel.NetworkId = Convert.ToInt32(this.textBoxNetworkId.Text);
            this.rPCModel.GasPrice = Convert.ToDouble(this.textBoxGasPrice.Text);
            this.rPCModel.GasLimit = Convert.ToDouble(this.textBoxGasLimit.Text);
            this.accountModel.Address= this.textBoxAccountAddress.Text;
            this.accountModel.PrivateKey = this.textBoxPrivateKey.Text;
            this.routerContract.Address = this.textBoxRouter.Text;
            this.contractToken0.Address = this.textBoxToken0.Text;
            this.contractToken1.Address = this.textBoxToken1.Text;
            this.lpModel.Address=this.textBoxLP.Text;
            this.lpModel.AmountLiquidity=Convert.ToDecimal(textBoxLPRemoveAmount.Text);
            if (string.IsNullOrEmpty(this.textBoxLP.Text))
            {
                MessageBox.Show("lp地址不能为空，请点击查看LP地址后操作!", "提示");
                return;
            }
            ResultModel resultModel = Web3Ext.getAllowance(rPCModel, this.accountModel, this.lpModel, this.routerContract.Address);//
            if (resultModel.State == 1)
            {
                if (resultModel.DecimalValue < this.lpModel.AmountLiquidity)
                {
                    resultModel = Web3Ext.tokenApprove(this.rPCModel, this.accountModel, this.lpModel, 1000000000000000, this.routerContract.Address);
                    textBoxLPResult.Text = resultModel.Message;
                    if (resultModel.State != 1)
                    {
                        MessageBox.Show("授权失败!", "提示");
                        return;
                    }
                }
            }
            else
            {
                textBoxLPResult.Text = resultModel.Message;
            }
            resultModel =Web3Ext.removeLiquidity(this.rPCModel,this.accountModel,this.routerContract,this.lpModel,this.contractToken0,this.contractToken1);
            if (resultModel.State == 1)
            {
                textBoxLPResult.Text = "成功!";
                //this.lpModel.Address = this.textBoxLP.Text;
                //resultModel = Web3Ext.getTokenBalance(this.rPCModel, this.lpModel, this.textBoxAccountAddress.Text);
                //if (resultModel.State == 1)
                //{
                //    textBoxLPBalance.Text = resultModel.DecimalValue.ToString();
                //}
                //else
                //{
                //    textBoxLPBalance.Text = resultModel.Message;
                //}
            }
            else
            {
                textBoxLPResult.Text = resultModel.Message;
            }
        }

        private void SwapSell_Click(object sender, RoutedEventArgs e)
        {
            this.rPCModel.URL = this.textBoxUrl.Text;
            this.rPCModel.NetworkId = Convert.ToInt32(this.textBoxNetworkId.Text);
            this.rPCModel.GasPrice = Convert.ToDouble(this.textBoxGasPrice.Text);
            this.rPCModel.GasLimit = Convert.ToDouble(this.textBoxGasLimit.Text);
            this.accountModel.Address = this.textBoxAccountAddress.Text;
            this.accountModel.PrivateKey = this.textBoxPrivateKey.Text;
            this.routerContract.Address = this.textBoxRouter.Text;
            this.contractToken0.Address = this.textBoxToken0.Text;
            this.contractToken1.Address = this.textBoxToken1.Text;
            this.contractToken0.UnitDecimal = Convert.ToInt32(this.textBoxToken0Decimals.Text);
            this.contractToken1.UnitDecimal = Convert.ToInt32(this.textBoxToken1Decimals.Text);
            SwapModel swapModel = new SwapModel();
            swapModel.Slippage = Convert.ToDouble(textBoxSwapSlippage.Text);
            swapModel.Path[0] = this.contractToken0.Address;
            swapModel.Path[1] = this.contractToken1.Address;
            contractToken0.AmountLiquidity = Convert.ToDecimal(this.textBoxSwapToken0Amount.Text);
            ResultModel resultModel = Web3Ext.swapExactTokensForTokensSupportingFeeOnTransferTokens(rPCModel, accountModel, routerContract, contractToken0, contractToken1, swapModel);
            if (resultModel.State == 1)
            {
                textBoxLPResult.Text = "成功!";
            }
            else
            {
                textBoxLPResult.Text = resultModel.Message;
            }
        }

        private void SwapBuy_Click(object sender, RoutedEventArgs e)
        {
            this.rPCModel.URL = this.textBoxUrl.Text;
            this.rPCModel.NetworkId = Convert.ToInt32(this.textBoxNetworkId.Text);
            this.rPCModel.GasPrice = Convert.ToDouble(this.textBoxGasPrice.Text);
            this.rPCModel.GasLimit = Convert.ToDouble(this.textBoxGasLimit.Text);
            this.accountModel.Address = this.textBoxAccountAddress.Text;
            this.accountModel.PrivateKey = this.textBoxPrivateKey.Text;
            this.routerContract.Address = this.textBoxRouter.Text;
            this.contractToken0.Address = this.textBoxToken0.Text;
            this.contractToken1.Address = this.textBoxToken1.Text;
            this.contractToken0.UnitDecimal = Convert.ToInt32(this.textBoxToken0Decimals.Text);
            this.contractToken1.UnitDecimal = Convert.ToInt32(this.textBoxToken1Decimals.Text);
            SwapModel swapModel= new SwapModel();
            swapModel.Slippage =Convert.ToDouble(textBoxSwapSlippage.Text);
            swapModel.Path[0] = this.contractToken1.Address;
            swapModel.Path[1] = this.contractToken0.Address;
            contractToken1.AmountLiquidity=Convert.ToDecimal(this.textBoxSwapToken1Amount.Text);
            ResultModel resultModel = Web3Ext.swapExactTokensForTokens(rPCModel, accountModel, routerContract, contractToken0, contractToken1, swapModel);
            if (resultModel.State == 1)
            {
                textBoxLPResult.Text = "成功!";
            }
            else
            {
                textBoxLPResult.Text = resultModel.Message;
            }
        }

        private void GetReserve_Click(object sender, RoutedEventArgs e)
        {
            this.rPCModel.URL = this.textBoxUrl.Text;
            this.rPCModel.NetworkId = Convert.ToInt32(this.textBoxNetworkId.Text);
            this.contractToken0.Address = this.textBoxToken0.Text;
            this.contractToken1.Address = this.textBoxToken1.Text;
            this.contractToken0.UnitDecimal = Convert.ToInt32(this.textBoxToken0Decimals.Text);
            this.contractToken1.UnitDecimal = Convert.ToInt32(this.textBoxToken1Decimals.Text);
            if (string.IsNullOrEmpty(this.textBoxLP.Text))
            {
                MessageBox.Show("lp地址不能为空，请点击查看LP地址后操作!", "提示");
                return;
            }
            this.lpModel.Address = this.textBoxLP.Text;
            ResultModel resultModel = Web3Ext.getLPReserves(rPCModel, lpModel, contractToken0, contractToken1);
            if(resultModel.State == 1)
            {
                this.textBoxReserveA.Text = resultModel.TokenAReserves.ToString();
                this.textBoxReserveB.Text = resultModel.TokenBReserves.ToString();
                this.textBoxPriceA.Text = Convert.ToDecimal(resultModel.TokenBReserves / resultModel.TokenAReserves).ToString();
                this.textBoxPriceB.Text = Convert.ToDecimal(resultModel.TokenAReserves / resultModel.TokenBReserves).ToString();
            }
            else
            {
                this.textBoxLPResult.Text = resultModel.Message;
            }
        }

        private void MoreResources_MouseUp(object sender, MouseButtonEventArgs e)
        {
            ProcessStartInfo processStartInfo= new ProcessStartInfo("https://www.bbctool.com") { UseShellExecute = true };
            System.Diagnostics.Process.Start(processStartInfo);// "www.bbctool.com");
        }
    }
}
