using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using HLL.Compiler;
using HLL_Viewer;
using Raylib_cs;
using System.Numerics;
using System.Net;

namespace HLL
{
    public static class Core
    {
        public static Widget[] widgets;
        public static string logout
        {
            get { return Directory.GetCurrentDirectory() + $"/logs/log-{DateTime.Now.Year}-{DateTime.Now.Month}-{DateTime.Now.Day}-{DateTime.Now.Second}.log"; }
        }
        public static void Open(string page)
        {

            HLLCompiler compiler = new HLLCompiler();
            widgets = compiler.Compile(page);

            Raylib.InitWindow(500, 500, "HLL Viewer");
            Raylib.SetTargetFPS(60);
            int defaultTXTSize = WidgetData.textSize;

            while (!Raylib.WindowShouldClose())
            {
                Raylib.BeginDrawing();
                Raylib.ClearBackground(Color.WHITE);
                for (int i = 0; i < widgets.Length; i++)
                {
                    widgets[i].OnDraw();
                }
                Raylib.EndDrawing();
            }
            Raylib.CloseWindow();
            Logger.EndLog(logout);
            
        }


        public static class Config
        {
            public static bool saveLog;
            public static bool printLog;
        }
    }

    public static class WidgetData
    {
        public static int textSize = 10;
    }

    public abstract class Widget
    {
        public abstract void OnDraw();
    }

    public class Text : Widget
    {
        public string text;
        public int x, y;
        public override void OnDraw()
        {
            Raylib.DrawText(text, x, y, WidgetData.textSize, Color.BLACK);
        }
    }

    public class Link : Widget
    {
        public string text, url;
        public int x, y;
        public override void OnDraw()
        {
            Vector2 mousePos = Raylib.GetMousePosition();
            if (mousePos.X > x && mousePos.Y > y && mousePos.X < x + Raylib.MeasureText(text, WidgetData.textSize) && y < y + WidgetData.textSize)
            {
                Raylib.DrawText(text, x, y, WidgetData.textSize, Color.BLUE);
                if (Raylib.IsMouseButtonPressed(MouseButton.MOUSE_LEFT_BUTTON))
                {
                    Logger.WriteLine("Creating compiler...");
                    HLLCompiler compiler = new HLLCompiler();
                    WebClient webClient = new WebClient();
                    Logger.WriteLine("Downloading data...");
                    string data = webClient.DownloadString(url);
                    Logger.WriteLine("Compiling data...");
                    Core.widgets = compiler.Compile(data);
                }
            }
            else
            {
                Raylib.DrawText(text, x, y, WidgetData.textSize, Color.BLACK);
            }
        }
    }

    public class Font : Widget
    {
        public int size;

        public override void OnDraw()
        {
            WidgetData.textSize = size;
        }
    }
}