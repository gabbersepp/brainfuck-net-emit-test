using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System;
using System.Reflection.Emit;


namespace BfCompiler
{
    public class BfCompiler
    {
        private string code;
        private ILGenerator ilg;
        private LocalBuilder array;
        private LocalBuilder working;
        private LocalBuilder index;

        //Stack<Label> startStack = new Stack<Label>();
        //Stack<Label> endStack = new Stack<Label>();
        //private Dictionary<Label, Label> endStart = new Dictionary<Label, Label>();


        public BfCompiler(string code)
        {
            this.code = code;
        }

        public void Compile(string filename)
        {
            var ab = GetAssemblyBuilder();
            var tb = GetTypeBuilder(ab, filename);
            var mb = GetMainMethod(tb);
            ilg = mb.GetILGenerator();
            WriteLine("Starting Brainfuck...");
            AllocateLongMemory(10000);

            working = ilg.DeclareLocal(typeof(long));
            index = ilg.DeclareLocal(typeof(int));

            // start at middle of memory
            ilg.Emit(OpCodes.Ldc_I4, 5000);
            ilg.Emit(OpCodes.Stloc, index);

            code.ToCharArray().ToList().ForEach(c =>
            {
                switch (c)
                {
                    case '+':
                        Add(1);
                        break;
                    case '-':
                        Add(-1);
                        break;
                    case '>':
                        AddToIndex(1);
                        break;
                    case '<':
                        AddToIndex(-1);
                        break;
                    case '.':
                        PrintChar();
                        break;
                    case ',':
                        ReadChar();
                        break;
                    /*case '[':
                        WhileStart();
                        break;
                    case ']':
                        WhileEnd();
                        break;*/

                }
            });

            Return();
            Seal(tb, ab, mb, filename);
        }

       /* private void WhileStart()
        {
            var startLabel = ilg.DefineLabel();
            var endLabel = ilg.DefineLabel();
            endStack.Push(endLabel);
            endStart[endLabel] = startLabel;

            ilg.MarkLabel(startLabel);
            ilg.Emit(OpCodes.Ldloc, array);
            ilg.Emit(OpCodes.Ldloc, index);
            ilg.Emit(OpCodes.Ldelem);
            ilg.Emit(OpCodes.Ldc_I4, 0);
            ilg.Emit(OpCodes.Beq, endLabel);
        }

        private void WhileEnd()
        {
            var endLabel = endStack.Pop();

            ilg.MarkLabel(endLabel);
            ilg.Emit(OpCodes.Ldloc, array);
            ilg.Emit(OpCodes.Ldloc, index);
            ilg.Emit(OpCodes.Ldelem);
            ilg.Emit(OpCodes.Ldc_I4, 0);
            ilg.Emit(OpCodes.Bne_Un, endStart[endLabel]);
        }*/

        private void ReadChar()
        {
            ilg.Emit(OpCodes.Call, typeof(Console).GetMethod(nameof(Console.Read), new Type[0]));
            ilg.Emit(OpCodes.Stloc, working);
            ilg.Emit(OpCodes.Ldloc, array);
            ilg.Emit(OpCodes.Ldloc, index);
            ilg.Emit(OpCodes.Ldloc, working);
            ilg.Emit(OpCodes.Stelem, typeof(long));
        }

        private void PrintChar()
        {
            ilg.Emit(OpCodes.Ldloc, array);
            ilg.Emit(OpCodes.Ldloc, index);
            ilg.Emit(OpCodes.Ldelem, typeof(long));
            
            ilg.Emit(OpCodes.Call, typeof(Console).GetMethod(nameof(Console.Write), new[] { typeof(char) }));
        }

        private AssemblyBuilder GetAssemblyBuilder()
        {
            AssemblyName an = new AssemblyName();
            an.Name = "Brainfuck";
            AppDomain ad = AppDomain.CurrentDomain;
            AssemblyBuilder ab = ad.DefineDynamicAssembly(an,
                AssemblyBuilderAccess.Save);
            return ab;
        }

        private TypeBuilder GetTypeBuilder(AssemblyBuilder ab, string fileName)
        {
            ModuleBuilder mb = ab.DefineDynamicModule(ab.GetName().Name, fileName);

            TypeBuilder tb = mb.DefineType("Gabbersepp.Brainfuck",
                TypeAttributes.Public | TypeAttributes.Class);

            return tb;
        }

        private MethodBuilder GetMainMethod(TypeBuilder tb)
        {
            return tb.DefineMethod("Main",
                MethodAttributes.Public |
                MethodAttributes.Static,
                typeof(int), new Type[] { typeof(string[]) });
        }

        private void Seal(TypeBuilder tb, AssemblyBuilder ab, MethodBuilder mb, string filename)
        {
            tb.CreateType();
            ab.SetEntryPoint(mb, PEFileKinds.ConsoleApplication);
            ab.Save(filename);
        }

        private void Return()
        {
            ilg.Emit(OpCodes.Ldc_I4, 0);
            ilg.Emit(OpCodes.Ret);
        }

        private void WriteLine(string str)
        {
            ilg.Emit(OpCodes.Ldstr, str);
            ilg.Emit(OpCodes.Call, typeof(Console).GetMethod("WriteLine", new Type[] { typeof(string) }));
        }

        private void AllocateLongMemory(int size)
        {
            array = ilg.DeclareLocal(typeof(long[]));
            ilg.Emit(OpCodes.Ldc_I4, size);
            ilg.Emit(OpCodes.Newarr, typeof(long));
            ilg.Emit(OpCodes.Stloc, array);
        }

        private void Add(int val)
        {
            // stack: array adresse
            ilg.Emit(OpCodes.Ldloc, array);
            // stack: array index
            ilg.Emit(OpCodes.Ldloc, index);
            ilg.Emit(OpCodes.Ldelem, typeof(long));

            ilg.Emit(OpCodes.Ldc_I4, val);
            ilg.Emit(OpCodes.Add);

            ilg.Emit(OpCodes.Stloc, working);

            ilg.Emit(OpCodes.Ldloc, array);
            ilg.Emit(OpCodes.Ldloc, index);
            ilg.Emit(OpCodes.Ldloc, working);
            ilg.Emit(OpCodes.Stelem, typeof(long));
        }

        private void AddToIndex(int val)
        {
            ilg.Emit(OpCodes.Ldloc, index);
            ilg.Emit(OpCodes.Ldc_I4, val);
            ilg.Emit(OpCodes.Add);
            ilg.Emit(OpCodes.Stloc, index);
        }
    }
}
