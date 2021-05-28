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
                if(c.type == "text")
                {
                    Text text = new Text();
                    text.x = (int)c.parameters[0];
                    text.y = (int)c.parameters[1];
                    text.text = (string)c.parameters[2];
                    widgets.Add(text);
                }
                if (c.type == "font")
                {
                    Font text = new Font();
                    text.size = (int)c.parameters[0];
                    widgets.Add(text);
                }
                if (c.type == "link")
                {
                    Link link = new Link();
                    link.x = (int)c.parameters[0];
                    link.y = (int)c.parameters[1];
                    link.text = (string)c.parameters[2];
                    link.url = (string)c.parameters[3];
                    widgets.Add(link);
                }


                ptr++;
            }
            return widgets.ToArray();
        }
    }
}
