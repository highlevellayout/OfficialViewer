using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using HLL.Viewer;
using HLL;
using System.Net;
using System.Reflection;
using ReCT;
namespace HLL_Viewer
{
    class Program
    {
        static void Main(string[] args)
        {
            Core.Config.printLog = true;
            //Core.Config.saveLog = true;
            Logger.BeginLog();
            WebClient webClient = new WebClient();
            Core.Open(webClient.DownloadString("http://bytespace.tk/hll/rect"));
        }
    }

    public static class Logger
    {
        static string output = "";
        static bool logging = false;
        static List<ErrorData> errors = new List<ErrorData>();
        public static void WriteLine(string data)
        {
            Write(data + "\n");
        }
        public static void Write(string data)
        {
            if (Core.Config.printLog)
            Console.Write(data);
            if (logging)
                output += data;
        }

        public static void BeginLog()
        {
            logging = true;
            output = "";
            errors = new List<ErrorData>();
        }

        public static string EndLog(string fileName)
        {
            logging = false;
            if (Core.Config.saveLog)
                File.WriteAllText(fileName, output);
            return output;
        }

        public static void LogError(string error, bool crashes)
        {
            errors.Add(new ErrorData(error, crashes));
        }

        public static void Error(string error, bool crashes)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            WriteLine("ERROR: " + error);
            Console.ResetColor();
            if (crashes)
            {
                EndLog(Core.logout);
                Environment.Exit(-1);
            }
        }

        public static void ReleaseErrors()
        {
            bool shouldCrash = false;
            for (int i = 0; i < errors.Count; i++)
            {
                Error(errors[i].message, false);
                if (errors[i].crashes)
                {
                    shouldCrash = true;
                }
            }

            if (shouldCrash)
            {
                EndLog(Core.logout);
                Environment.Exit(-1);
            }
        }

        private class ErrorData
        {

            public string message;
            public bool crashes;

            public ErrorData(string message, bool crashes)
            {
                this.message = message;
                this.crashes = crashes;
            }
        }
    }
}
