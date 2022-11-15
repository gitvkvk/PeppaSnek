using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;



namespace Snek
{
    //container for image assets
    public static class Images
    {
        //add static variables for each image asset
        public readonly static ImageSource Empty = LoadImage("Empty.png");
        public readonly static ImageSource Body = LoadImage("Body.png");
        public readonly static ImageSource Head = LoadImage("Head.png");
        public readonly static ImageSource Food = LoadImage("Food.png");
        public readonly static ImageSource DeadBody = LoadImage("DeadBody.png");
        public readonly static ImageSource DeadHead = LoadImage("DeadHead.png");


        //loads image with given file name and returns as image source
        private static ImageSource LoadImage(string fileName)
        {
            return new BitmapImage(new Uri($"Assets/{fileName}", UriKind.Relative)); //what is uri, string formatting $
        }
        
    }
}
