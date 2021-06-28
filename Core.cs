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
        public static float scrollOffset = 0;
        public static int lowestObj = 0;
        public static string logout
        {
            get { return Directory.GetCurrentDirectory() + $"/logs/log-{DateTime.Now.Year}-{DateTime.Now.Month}-{DateTime.Now.Day}-{DateTime.Now.Second}.log"; }
        }

        static Raylib.TraceLogCallback LogCustom()
        {
            return null;
        }


        public static void Open(string page)
        {

            HLLCompiler compiler = new HLLCompiler();
            LoadWidgets(compiler.Compile(page));

            Raylib.InitWindow(500, 500, "HLL Viewer");
            Raylib.SetTargetFPS(60);
            Raylib.SetTraceLogLevel(TraceLogType.LOG_NONE);
            int defaultTXTSize = WidgetData.textSize;

            while (!Raylib.WindowShouldClose())
            {
                WidgetData.textSize = defaultTXTSize;
                Raylib.BeginDrawing();
                Raylib.ClearBackground(Color.WHITE);
                for (int i = 0; i < widgets.Length; i++)
                {
                    widgets[i].OnDraw();
                }
                Raylib.EndDrawing();

                scrollOffset += Raylib.GetMouseWheelMove() * 10;

                if(scrollOffset < lowestObj)
                {
                    scrollOffset = Lerp(scrollOffset, lowestObj, 0.1f);
                }
                if (scrollOffset > 0)
                {
                    scrollOffset = Lerp(scrollOffset, lowestObj, 0.1f);
                }
            }
            Raylib.CloseWindow();
            Logger.EndLog(logout);
            
        }

        static float Lerp(float firstFloat, float secondFloat, float by)
        {
            return firstFloat * (1 - by) + secondFloat * by;
        }


        public static class Config
        {
            public static bool saveLog;
            public static bool printLog;
        }

        public static void LoadWidgets(Widget[] widgets)
        {
            Core.widgets = widgets;
            scrollOffset = 0;
            foreach (var widget in widgets)
            {
                lowestObj = (int)MathF.Min(widget.y - widget.GetHeight() - 1, lowestObj);
            }
        }
    }

    public static class WidgetData
    {
        public static int textSize = 10;
    }

    public abstract class Widget
    {
        public abstract void OnDraw();
        public virtual int GetHeight() { return 0; }
        public int x, y;
    }

    public class Text : Widget
    {
        public string text;
        public override void OnDraw()
        {
            Raylib.DrawText(text, x, y + (int)Core.scrollOffset, WidgetData.textSize, Color.BLACK);
        }

        public override int GetHeight()
        {
            return Raylib.MeasureText("#", WidgetData.textSize);
        }
    }

    public class Link : Widget
    {
        public string text, url;
        public override void OnDraw()
        {
            Vector2 mousePos = Raylib.GetMousePosition();
            if (mousePos.X > x && mousePos.Y > y + (int)Core.scrollOffset && mousePos.X < x + Raylib.MeasureText(text, WidgetData.textSize) && mousePos.Y < y + (int)Core.scrollOffset + Raylib.MeasureText("#", WidgetData.textSize))
            {
                Raylib.DrawText(text, x, y + (int)Core.scrollOffset, WidgetData.textSize, Color.BLUE);
                if (Raylib.IsMouseButtonPressed(MouseButton.MOUSE_LEFT_BUTTON))
                {
                    Logger.WriteLine("Creating compiler...");
                    HLLCompiler compiler = new HLLCompiler();
                    WebClient webClient = new WebClient();
                    Logger.WriteLine("Downloading data...");
                    if (!url.Contains("?")) {
                        url += "/?hll=true";
                        
                    }
                    else
                        url += "&hll=true";
                    Console.WriteLine(url);
                    string data = webClient.DownloadString(url);
                    Logger.WriteLine("Compiling data...");
                    Core.LoadWidgets(compiler.Compile(data));
                }
            }
            else
            {
                Raylib.DrawText(text, x, y + (int)Core.scrollOffset, WidgetData.textSize, Color.BLACK);
            }
        }

        public override int GetHeight()
        {
            return Raylib.MeasureText("#", WidgetData.textSize);
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