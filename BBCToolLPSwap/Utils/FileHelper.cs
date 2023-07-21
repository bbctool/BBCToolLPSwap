using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBCToolLPSwap.Utils
{
    public class FileHelper
    {
        public static string ReadFile(string fileName)
        {
            StreamReader sr = new StreamReader(fileName, Encoding.UTF8);
            string context = sr.ReadToEnd();
            sr.Close();
            return context;
        }
    }
}
