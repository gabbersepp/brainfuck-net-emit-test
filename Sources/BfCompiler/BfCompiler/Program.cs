namespace BfCompiler
{
    class Program
    {
        static void Main(string[] args)
        {
            var bfCode = "++[>++++++++++++++++++++[>+<-]<-]>>.";// = "+[-[<<[+[--->]-[<<<]]]>>>-]>-.---.>..>.<<<<-.<+.>>>>>.>.<<.<-.";

            new BfCompiler(bfCode).Compile("bf.exe");
        }
    }
}
