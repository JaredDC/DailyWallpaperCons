using System.Runtime.InteropServices;
using System.Drawing;
using Microsoft.Win32;
using System.IO;
using System.Text.RegularExpressions;

public class Wallpaper
{
    const int SPI_SETDESKWALLPAPER = 20;
    const int SPIF_UPDATEINIFILE = 0x01;
    const int SPIF_SENDWININICHANGE = 0x02;

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);


    public enum Style : int
    {
        Fill,
        Fit,
        Span,
        Stretch,
        Tile,
        Center
    }
    
    private static int GetHanNumFromString(string str)
    {
        int count = 0;
        Regex regex = new Regex(@"^[\u4E00-\u9FA5]{0,}$");

        for (int i = 0; i < str.Length; i++)
        {
            if (regex.IsMatch(str[i].ToString()))
            {
                count++;
            }
        }

        return count;
    }
    public static void AddWaterMark(string sourceFile, string destFile, string waterMark)
    {
        System.Drawing.Image bitmap = (System.Drawing.Image)Bitmap.FromFile(sourceFile); // set image     
        // Font font = new Font("Microsoft YaHei", 20, FontStyle.Regular, GraphicsUnit.Pixel);
        int fontSize = 18;
        Font font = new Font("Microsoft YaHei", fontSize, FontStyle.Regular);

        //Color color = FromArgb(255, 255, 0, 0);
        // bitmap.Width - font_len - 50
        int font_len = waterMark.Length + GetHanNumFromString(waterMark);
        /*System.Console.WriteLine("waterMark:" + waterMark);
        System.Console.WriteLine("waterMark.Length:" + waterMark.Length);
        System.Console.WriteLine("font_len:" + font_len);
        System.Console.WriteLine("GetHanNumFromString:" + GetHanNumFromString(waterMark));*/
        
        int wid = bitmap.Width - font_len * fontSize / 2 + 50;
        int hei = bitmap.Height - fontSize - 50;
        Point atpoint = new Point(wid, hei);
        System.Console.WriteLine("[" + wid +","+ hei + "]");
        System.Console.WriteLine("[" + bitmap.Width + ","+ bitmap.Height + "]");
        SolidBrush brush = new SolidBrush(Color.White);
        Graphics graphics = Graphics.FromImage(bitmap);
        StringFormat stringFormat = new StringFormat();
        stringFormat.Alignment = StringAlignment.Center;
        stringFormat.LineAlignment = StringAlignment.Center;
        graphics.DrawString(waterMark, font, brush, atpoint, stringFormat);
        graphics.Dispose();
        MemoryStream m = new MemoryStream();
        bitmap.Save(m, System.Drawing.Imaging.ImageFormat.Jpeg);
        byte[] convertedToBytes = m.ToArray();
        System.IO.File.WriteAllBytes(destFile, convertedToBytes);
    }

    public static void SetWallPaper(string wallpaper, Style style = Style.Fill)
    {
        using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true))
        {
            if (style == Style.Fill)
            {
                key.SetValue(@"WallpaperStyle", 10.ToString());
                key.SetValue(@"TileWallpaper", 0.ToString());
            }
            if (style == Style.Fit)
            {
                key.SetValue(@"WallpaperStyle", 6.ToString());
                key.SetValue(@"TileWallpaper", 0.ToString());
            }
            if (style == Style.Span) // Windows 8 or newer only!
            {
                key.SetValue(@"WallpaperStyle", 22.ToString());
                key.SetValue(@"TileWallpaper", 0.ToString());
            }
            if (style == Style.Stretch)
            {
                key.SetValue(@"WallpaperStyle", 2.ToString());
                key.SetValue(@"TileWallpaper", 0.ToString());
            }
            if (style == Style.Tile)
            {
                key.SetValue(@"WallpaperStyle", 0.ToString());
                key.SetValue(@"TileWallpaper", 1.ToString());
            }
            if (style == Style.Center)
            {
                key.SetValue(@"WallpaperStyle", 0.ToString());
                key.SetValue(@"TileWallpaper", 0.ToString());
            }
        }

        SystemParametersInfo(SPI_SETDESKWALLPAPER,
                0,
                wallpaper,
                SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
    }
}