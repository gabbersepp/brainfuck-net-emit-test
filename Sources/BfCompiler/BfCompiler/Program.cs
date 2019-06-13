using System.IO;

namespace BfCompiler
{
    class Program
    {
        static void Main(string[] args)
        {
            string code = "";

            if (args.Length == 0)
            {
                code = "++[>++++++++++++++++++++[>+<-]<-]>>.";
            }
            else if (args[0] == "-f")
            {
                code = ReadFile(args[1]);
            }
            else if(args[0] == "-c")
            {
                code = args[1];
            }
            //var bfCode = ;// = "+[-[<<[+[--->]-[<<<]]]>>>-]>-.---.>..>.<<<<-.<+.>>>>>.>.<<.<-.";

            new BfCompiler(code).Compile("bf.exe");
        }

        private static string ReadFile(string path)
        {
            return File.ReadAllText(path);
        }
    }
}
