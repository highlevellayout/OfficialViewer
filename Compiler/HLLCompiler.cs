using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HLL_Viewer;

namespace HLL.Compiler
{
    public class HLLCompiler
    {
        public Widget[] Compile(string code)
        {
            Lexer lexer = new Lexer();
            Token[] tokenArr = lexer.Lex(code);

            Logger.WriteLine("Tokens:");
            foreach (var token in tokenArr)
            {
                Logger.WriteLine(" " + token.ToString());
            }

            Parser parser = new Parser();
            Statement[] statements = parser.Parse(tokenArr);

            Logger.WriteLine("Statements:");
            foreach (var statement in statements)
            {
                Logger.Write($" [{statement.type}");
                for (int i = 0; i < statement.parameters.Length; i++)
                {
                    object param = statement.parameters[i];
                    Logger.Write($", [{param}, {statement.parameterTypes[i]}]");
                }
                Logger.WriteLine("]");
            }

            Checker checker = new Checker();
            checker.Check(statements);

            Emitter emitter = new Emitter();

            return emitter.Emit(statements);
        }
    }
}
