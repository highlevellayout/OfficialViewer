using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using HLL.Compiler;
using HLL_Viewer;

namespace HLL
{
    public static class Core
    {
        public static List<Token> tokens;
        public static string logout
        {
            get { return Directory.GetCurrentDirectory() + $"/logs/log-{DateTime.Now.Year}-{DateTime.Now.Month}-{DateTime.Now.Day}-{DateTime.Now.Second}.log"; }
        }
        public static void Open(string page)
        {

            HLLCompiler compiler = new HLLCompiler();
            compiler.Compile("[hi];[text];aaa");

            Logger.EndLog(logout);
            
        }


        public static class Config
        {
            public static bool saveLog;
            public static bool printLog;
        }
    }

    public abstract class Widget
    {
        public abstract void OnDraw();
    }
}