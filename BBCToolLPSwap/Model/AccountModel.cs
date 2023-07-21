using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBCToolLPSwap.Model
{
    public class AccountModel
    {
        private string address;//钱包地址
        private string name;
        private string privateKey;//私钥

        public string Address { get => address; set => address = value; }
        public string Name { get => name; set => name = value; }
        public string PrivateKey { get => privateKey; set => privateKey = value; }
    }
}
