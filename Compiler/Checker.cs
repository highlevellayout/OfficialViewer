using HLL_Viewer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HLL.Compiler
{
    public class Checker
    {
        public int ptr;
        public Statement[] code;

        public Statement next
        {
            get { return code[ptr + 1]; }
        }

        public Statement c
        {
            get { return code[ptr]; }
        }

        public bool EOF
        {
            get { return ptr >= code.Length; }
        }

        public bool EOFN
        {
            get { return ptr + 1 >= code.Length; }
        }

        public void Check(Statement[] codeToCheck)
        {
            ptr = 0;
            code = codeToCheck;
            while (!EOF)
            {


                if(c.type == "text")
                {
                    if(c.parameters.Length != 3)
                    {
                        Logger.LogError("Invalid amount of parameters in text statement, expected 3, got " + c.parameters.Length, true);
                    }
                    if(c.parameterTypes != new TokenType[] {TokenType.Int, TokenType.Int, TokenType.String })
                    {
                        Logger.LogError("Invalid parameter types in text statement", true);
                    }
                }


                ptr++;
            }

            Logger.ReleaseErrors();
        }
    }
}
