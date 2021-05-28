using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HLL.Compiler
{
    public class Emitter
    {
        public int ptr;
        public Statement[] code;
        public List<Widget> widgets;
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

        public Widget[] Emit(Statement[] codeToEmit)
        {
            widgets = new List<Widget>();
            ptr = 0;
            code = codeToEmit;

            while (!EOF)
            {

            }

            return widgets.ToArray();
        }
    }
}
