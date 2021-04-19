using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HLL_Viewer;
using Raylib_cs;
using System.Numerics;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Net;
using System.IO;
using HLL;

namespace HLL_Viewer
{
    public static class Core
    {
        static Dictionary<string, object> data;
        public static string currentSite;
        public static void LoadData(string[] rawdata)
        {
            Logger.WriteLine("---Preparing the data---");
            data = new Dictionary<string, object>();
            for (int i = 0; i < rawdata.Length; i++)
            {
                Logger.WriteLine("Preparing " + rawdata[i]);
                string[] dataBlock = rawdata[i].Replace("[", "").Replace("]", "").Trim(new char[] { (char)10, (char)32, (char)9 }).Split(",");

                for (int dataBlockI = 0; dataBlockI < dataBlock.Length; dataBlockI++)
                {
                    dataBlock[dataBlockI] = dataBlock[dataBlockI].Trim();
                }
                if (dataBlock[0] == "text")
                {
                    try
                    {
                        Text outData = new Text(int.Parse(dataBlock[1]), int.Parse(dataBlock[2]), dataBlock[3].Replace("\"", ""));
                        data.Add("text" + i, outData);
                    }
                    catch (Exception e)
                    {
                        Logger.WriteLine("### ERROR! UNABLE TO DEFINE TEXT. INFEED WAS " + rawdata[i].Trim(new char[] { (char)10, (char)32, (char)9 }));
                        Logger.WriteLine("CS error:\n" + e.Message);
                    }
                }
                else if (dataBlock[0] == "savelog")
                {
                    Config.saveLog = true;
                }
                else if (dataBlock[0] == "config")
                {
                    Config.textSize = int.Parse(dataBlock[1]);
                }
                else if (dataBlock[0] == "font")
                {
                    Font font = new Font(int.Parse(dataBlock[1]));
                    data.Add("font" + i, font);
                }
                else if(dataBlock[0] == "style")
                {
                    Style style = new Style(Font.LoadFont(dataBlock[1].Replace("\"", "")));
                    
                    data.Add("style" + i, style);
                }
                else if (dataBlock[0] == "link")
                {
                    try
                    {
                        Link outData = new Link(int.Parse(dataBlock[1]), int.Parse(dataBlock[2]), dataBlock[3].Replace("\"", ""), dataBlock[4].Replace("\"", ""));
                        data.Add("link" + i, outData);
                    }
                    catch (Exception e)
                    {
                        Logger.WriteLine("### ERROR! UNABLE TO DEFINE TEXT. INFEED WAS " + rawdata[i].Trim(new char[] { (char)10, (char)32, (char)9 }));
                        Logger.WriteLine("CS error:\n" + e.Message);
                    }
                }
                if (dataBlock[0] == "image")
                {
                    Image outData = new Image(int.Parse(dataBlock[1]), int.Parse(dataBlock[2]), Image.LoadImage(dataBlock[3].Replace("\"", "")));
                    data.Add("img" + i, outData);
                    /*Logger.WriteLine("### ERROR! UNABLE TO DEFINE TEXT. INFEED WAS " + rawdata[i].Trim(new char[] { (char)10, (char)32, (char)9 }));
                    Logger.WriteLine("CS error:\n" + e.Message);*/
                }

            }
            Logger.WriteLine("---Data prepared enough---");
        }

        public static void View(string[] rawdata)
        {
            int startSize = Config.textSize;
            currentSite = Program.homePage;

            Logger.WriteLine("---Started Viewing---\n");
            Logger.WriteLine("Initializing raylib window");
            Raylib.InitWindow(500, 500, "HLL Viewer");
            Raylib.SetTargetFPS(60);
            LoadData(rawdata);
            Raylib_cs.Font font = Raylib.GetFontDefault();

            Logger.WriteLine("Starting window loop");
            while (!Raylib.WindowShouldClose())
            {
                Raylib.BeginDrawing();
                Raylib.ClearBackground(Color.RAYWHITE);
                Vector2 mousePos = Raylib.GetMousePosition();
                for (int i = 0; i < data.Count; i++)
                {
                    string dataKey = data.ElementAt(i).Key;
                    if (dataKey.StartsWith("text"))
                    {
                        Text dataToUse = (Text)data[dataKey];
                        Raylib.DrawTextEx(font, dataToUse.content, new Vector2(dataToUse.x, dataToUse.y), Config.textSize, 2, Color.BLACK);
                    }
                    else if (dataKey.StartsWith("link"))
                    {
                        Link dataToUse = (Link)data[dataKey];
                        Raylib.DrawTextEx(font, dataToUse.content, new Vector2(dataToUse.x, dataToUse.y), Config.textSize, 2, Color.BLACK);

                        if (mousePos.X > dataToUse.x && mousePos.Y > dataToUse.y && mousePos.Y < dataToUse.y + Config.textSize && mousePos.X < dataToUse.x + Raylib.MeasureText(dataToUse.content, Config.textSize))
                        {
                            Raylib.DrawTextEx(font, dataToUse.content, new Vector2(dataToUse.x, dataToUse.y), Config.textSize, 2, Color.BLUE);
                            if (Raylib.IsMouseButtonPressed(MouseButton.MOUSE_LEFT_BUTTON))
                            {
                                LoadData(Networking.DownloadPage(dataToUse.link).Replace("\n", "").Split(";", StringSplitOptions.RemoveEmptyEntries));
                                currentSite = dataToUse.link;
                            }
                        }
                    }
                    else if (dataKey.StartsWith("font"))
                    {
                        Font dataToUse = (Font)data[dataKey];
                        Config.textSize = dataToUse.size;
                    }
                    else if (dataKey.StartsWith("img"))
                    {
                        Image dataToUse = (Image)data[dataKey];
                        Raylib.DrawTexture(dataToUse.image, dataToUse.x, dataToUse.y, Color.WHITE);
                    }
                    else if (dataKey.StartsWith("style"))
                    {
                        Style dataToUse = (Style)data[dataKey];
                        font = dataToUse.font;
                        Config.textSize = dataToUse.font.baseSize;
                    }
                }

                Raylib.EndDrawing();
                Config.textSize = startSize;
            }
            Logger.WriteLine("Window should close! Is it a hang or normal closing?");
            Raylib.CloseWindow();
            Logger.WriteLine("Window closed");
            Logger.WriteLine("---Stopped Viewing---");
        }

        public class Text
        {
            public int x;
            public int y;
            public string content;

            public Text(int x, int y, string content)
            {
                this.x = x;
                this.y = y;
                this.content = content;
            }
        }

        public class Image
        {
            public int x;
            public int y;
            public Texture2D image;

            public Image(int x, int y, Texture2D image)
            {
                this.x = x;
                this.y = y;
                this.image = image;
            }

            public static Texture2D LoadImage(string path)
            {
                WebClient client = new WebClient();
                
                client.DownloadFile(path, Directory.GetCurrentDirectory() + "/" + Path.GetFileName(path));
                
                Texture2D returnVal = Raylib.LoadTexture(Directory.GetCurrentDirectory() + "/" + Path.GetFileName(path));
                File.Delete(Directory.GetCurrentDirectory() + "/" + Path.GetFileName(path));
                return returnVal;
            }
        }

        public class Link
        {
            public int x;
            public int y;
            public string content;
            public string link;

            public Link(int x, int y, string content, string link)
            {
                this.x = x;
                this.y = y;
                this.content = content;
                this.link = link;
            }
        }

        public class Font
        {
            public int size;

            public Font(int size)
            {
                this.size = size;
            }

            public static Raylib_cs.Font LoadFont(string path)
            {
                WebClient client = new WebClient();

                client.DownloadFile(path, Directory.GetCurrentDirectory() + "/" + Path.GetFileName(path));

                Raylib_cs.Font returnVal = Raylib.LoadFont(Directory.GetCurrentDirectory() + "/" + Path.GetFileName(path));
                File.Delete(Directory.GetCurrentDirectory() + "/" + Path.GetFileName(path));
                return returnVal;
            }
        }

        public class Style
        {
            public Raylib_cs.Font font;

            public Style(Raylib_cs.Font font)
            {
                this.font = font;
            }

            public static Raylib_cs.Font LoadFont(string path)
            {
                WebClient client = new WebClient();

                client.DownloadFile(path, Directory.GetCurrentDirectory() + "/" + Path.GetFileName(path));

                Raylib_cs.Font returnVal = Raylib.LoadFont(Directory.GetCurrentDirectory() + "/" + Path.GetFileName(path));
                File.Delete(Directory.GetCurrentDirectory() + "/" + Path.GetFileName(path));
                return returnVal;
            }
        }

        public static class Config
        {
            public static bool saveLog = false;
            public static int textSize = 20;
        }
    }
    public static class WebBrowser
    {
        public static void OpenUrl(string url)
        {
            try
            {
                Process.Start(url);
            }
            catch
            {
                // hack because of this: https://github.com/dotnet/corefx/issues/10361
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    url = url.Replace("&", "^&");
                    Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    Process.Start("xdg-open", url);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    Process.Start("open", url);
                }
                else
                {
                    throw;
                }
            }
        }
    }
}
