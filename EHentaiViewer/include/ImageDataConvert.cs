using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using System.Threading.Tasks;

namespace EHentaiViewer.include
{
    [ValueConversion(typeof(string),typeof(BitmapImage))]
    public class ImageDataConvert : IMultiValueConverter
    {
        //private string cacheFolder = "titleCache";
        //SHA1 sha1 = SHA1.Create();

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var imgPath = values[0].ToString();
            var name = values[1].ToString();
            //if (!Directory.Exists(cacheFolder))
            //{
            //    Directory.CreateDirectory(cacheFolder);
            //}
            if (!string.IsNullOrEmpty(imgPath))
            {
                //name = BitConverter.ToString(sha1.ComputeHash(Encoding.UTF8.GetBytes(name))).ToUpper(); ;
                //var newPath = cacheFolder + "\\" + name + System.IO.Path.GetExtension(imgPath);
                //if (File.Exists(newPath))
                //{
                //    imgPath = Path.GetFullPath(newPath);
                //}
                //else
                //{
                //    Task.Factory.StartNew(() =>
                //    {
                //        WebClient client = new WebClient();
                //        client.DownloadFile(imgPath, newPath);
                //    });
                //}
                BitmapImage bit = new BitmapImage(new Uri(imgPath));
                return bit;
            }
            else
            {
                return null;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
