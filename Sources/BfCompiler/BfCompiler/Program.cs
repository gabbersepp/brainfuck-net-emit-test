using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BfCompiler
{
    class Program
    {
        static void Main(string[] args)
        {
            var bfCode = "++++++++++++++++++++++++++++++++++++++++++++++++++.>+++++++++++++++++++++++++++++++++++++++++++++++++++++.";// = "+[-[<<[+[--->]-[<<<]]]>>>-]>-.---.>..>.<<<<-.<+.>>>>>.>.<<.<-.";

            new BfCompiler(bfCode).Compile("bf.exe");
        }
    }
}
