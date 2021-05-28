using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HLL_Viewer;

namespace HLL.Compiler
{
    public class Parser
    {
        public int ptr;
        public int line;
        public Token[] code;
        public List<Statement> statements;

        public Token next
        {
            get { return code[ptr + 1]; }
        }

        public Token c
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

        public Statement[] Parse(Token[] codeToParse)
        {
            line = 0;
            ptr = 0;
            code = codeToParse;
            statements = new List<Statement>();

            while (!EOF)
            {
                if(c.type == TokenType.OpenBracket)
                {
                    ParseFunc();
                    continue;
                }
                if(c.type == TokenType.Newline)
                {
                    line++;
                }
                ptr++;
            }
            Logger.ReleaseErrors();
            return statements.ToArray();
        }

        public void ParseFunc()
        {
            ptr++;
            int i = 0;
            Statement statement = new Statement();
            List<object> parameters = new List<object>();
            List<TokenType> types = new List<TokenType>();
            while(!EOF && c.type != TokenType.CloseBracket)
            {
                if (EOFN)
                    Logger.LogError($"Unclosed statement at line: {line}", true);
                if (c.type == TokenType.Newline)
                {
                    line++;
                    Logger.LogError($"Unclosed statement at line: {line}", true);
                }
                if (i == 0)
                {
                    if (c.type == TokenType.Keyword)
                        statement.type += c.value;
                    else
                        Logger.LogError($"Invalid type: {c.type.ToString()} at line: {line}", true);
                }
                else
                {
                    if (c.type == TokenType.String) {
                        parameters.Add(c.value);
                        types.Add(c.type);
                    }
                    else if (c.type == TokenType.Int)
                    {
                        parameters.Add(int.Parse(c.value));
                        types.Add(c.type);
                    }
                    else
                    {
                        Logger.LogError($"Invalid type: {c.type.ToString()} at line: {line}", true);
                    }
                }

                i++;
                ptr++;
            }
            ptr++;
            statement.parameters = parameters.ToArray();
            statement.parameterTypes = types.ToArray();
            statements.Add(statement);
        }
    }

    public class Statement
    {
        public string type = "";
        public object[] parameters;
        public TokenType[] parameterTypes;
    }
}
