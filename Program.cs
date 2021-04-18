﻿using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using HLL_Viewer;
using HLL;
using System.Net;
namespace HLL_Viewer
{
    class Program
    {
        static string file = Directory.GetCurrentDirectory() + "/file.hll";
        static string homePage = "https://highlevellayout.github.io/index.hll";
        static void Main(string[] args)
        {
            Logger.BeginLog();
            Logger.WriteLine("---START OF LOG---");
            if (!Directory.Exists(Directory.GetCurrentDirectory() + "/logs/"))
            {
                Logger.WriteLine("log folder missing! Creating...");
                Directory.CreateDirectory(Directory.GetCurrentDirectory() + "/logs/");
            }
            //string data = File.ReadAllText(file).Replace("\n", "");
            string data = Networking.DownloadPage(homePage);
            string[] lineDataRaw = data.Split(";", StringSplitOptions.RemoveEmptyEntries);
            Logger.WriteLine("---DATA START---");
            for (int i = 0; i < lineDataRaw.Length; i++)
            {
                Logger.WriteLine(lineDataRaw[i]);
            }
            Logger.WriteLine("---DATA END---");
            HLL_Viewer.Core.View(lineDataRaw);
            Logger.EndLog(Directory.GetCurrentDirectory() + "/logs/Log-" + DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day + "-" + DateTime.Now.Hour + "-" + DateTime.Now.Minute + "-" + DateTime.Now.Second + ".log");
        }
    }

    public static class Logger
    {
        static string output = "";
        static bool logging = false;
        public static void WriteLine(string data)
        {
            Write(data + "\n");
        }
        public static void Write(string data)
        {
            Console.Write(data);
            if (logging)
                output += data;
        }

        public static void BeginLog()
        {
            logging = true;
            output = "";
        }

        public static string EndLog(string fileName)
        {
            logging = false;
            if (Core.Config.saveLog)
                File.WriteAllText(fileName, output);
            return output;
        }
    }
}