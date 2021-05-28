using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HLL_Viewer;
namespace HLL.Compiler
{
    public class Lexer
    {
        public int ptr;
        public string code;
        public List<Token> tokens;


        public int line;

        public Char nextC
        {
            get { return code[ptr + 1]; }
        }

        public string next
        {
            get { return code[ptr + 1].ToString(); }
        }

        public string c
        {
            get { return code[ptr].ToString(); }
        }

        public char cc
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

        public Token[] Lex(string codeToLex)
        {
            ptr = 0;
            code = codeToLex;
            tokens = new List<Token>();
            line = 0;


            while (!EOF)
            {
                if (c == ";") { tokens.Add(new Token(TokenType.Newline)); ptr++; continue; line++; }
                if (c == "\n") { ptr++; continue; }
                if (c == "%") { while (c != "\n") { ptr++; } continue; }
                if (c == "[") { tokens.Add(new Token(TokenType.OpenBracket)); ptr++; continue; }
                if (c == "]") { tokens.Add(new Token(TokenType.CloseBracket)); ptr++; continue; }
                if (c == ",") { /*tokens.Add(new Token(TokenType.Comma));*/ ptr++; continue; }
                if (c == " ") { ptr++; continue; }


                if (Char.IsDigit(cc)) { LexNumber(); continue; }
                if (c == "\"") { LexString(); continue; }
                if (Char.IsLetter(cc)) { LexWord(); continue; }



                Logger.LogError("Unknown token: " + c, true);
                ptr++;
            }
            return tokens.ToArray();
        }

        private void LexNumber()
        {
            string num = "";
            while (!EOF && Char.IsDigit(cc))
            {
                num += c;
                ptr++;
            }
            tokens.Add(new Token(TokenType.Int, num));

        }

        private void LexWord()
        {
            string word = "";
            while (!EOF && Char.IsLetterOrDigit(cc))
            {
                word += c;
                ptr++;
            }
            PWord(word);

        }

        private void PWord(string word)
        {
            if (Defs.keywords.Contains(word)) { tokens.Add(new Token(TokenType.Keyword, word)); return; }
            //tokens.Add(new Token(TokenType.Word, word));
            Logger.LogError($"Invalid word: \"{word}\" at line: {line}", true);
        }

        private void LexString()
        {
            string str = "";
            ptr++;
            while (c != "\"")
            {
                if (EOFN)
                {
                    Logger.LogError($"Unfinished string at line: {line}", true);
                }
                str += c;
                ptr++;
            }
            ptr++;

            tokens.Add(new Token(TokenType.String, str));

        }

    }

    public class Token
    {
        public TokenType type;
        public string value;

        public Token(TokenType type, string value)
        {
            this.type = type;
            this.value = value;
        }

        public Token(TokenType type)
        {
            this.type = type;
            this.value = "null";
        }

        public override string ToString()
        {
            return $"[{type.ToString()}, '{value}']";
        }
    }

    public enum TokenType
    {
        Keyword, String, Int, OpenBracket, CloseBracket, Newline, Comment, CommentEnd, Comma, Word
    }
}
