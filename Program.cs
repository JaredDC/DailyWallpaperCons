using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wallpaper_today
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            // Wallpaper.SetWallPaper("C:\\Users\\jared\\Pictures\\pic\\61e030ca5106db3731eb1058432c6bbb7b4b5d3efbfe63001fbd56d9ae288daf.jpg");
            Wallpaper.AddWaterMark("C:\\Users\\jared\\Pictures\\photo_of_the_day\\黑沙滩上Reynisdrangar的玄武岩_冰岛_Cavan_Images_Getty_Images_watermark.jpg",
                "D:\\jared\\coding\\wallpaper_today\\watermark.jpg",
                "黑沙滩上Reynisdrangar的玄武岩,冰岛@Cavan Images Getty Images");
        }

    }
}
