using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using EHentaiModel;
using System.IO;
using System.Threading.Tasks;
using System.Net;

namespace EHentaiViewer
{
    /// <summary>
    /// ShowPhoto.xaml 的交互逻辑
    /// </summary>
    public partial class ShowPhoto : Window
    {
        private List<PicModel> pms = new List<PicModel>();
        private string cacheFolder = "cache";
        private int currentNumber = 0;
        private string name = "";
        public ShowPhoto(string name)
        {
            InitializeComponent();
            if (!Directory.Exists(cacheFolder + "\\" + name))
                Directory.CreateDirectory(cacheFolder + "\\" + name);
            this.name = name;
        }

        public void AddPicModel(PicModel pic)
        {
            var ext = System.IO.Path.GetExtension(pic.ImgUrl);
            var newPath = cacheFolder + "\\" + name + "\\" + pic.Index + ext;
            var downloadPath = name + "\\" + System.IO.Path.GetFileName(pic.ImgUrl);
            if (File.Exists(downloadPath))
            {
                pic.localUrl = System.IO.Path.GetFullPath(downloadPath);
            }
            else if (File.Exists(newPath))
            {
                pic.localUrl = System.IO.Path.GetFullPath(newPath);
            }
            else
            {
                Task.Factory.StartNew<string>(() =>
                {
                    int tryNumber = 0;
                    do
                    {
                        try
                        {
                            WebClient client = new WebClient();
                            client.DownloadFile(pic.ImgUrl, newPath);
                            return newPath;
                        }
                        catch { }
                    } while (tryNumber++ < 3);
                    return pic.ImgUrl;
                }, TaskCreationOptions.PreferFairness).ContinueWith((x) =>
                {
                    if (x.Status == TaskStatus.RanToCompletion)
                    {
                        pic.localUrl = System.IO.Path.GetFullPath(x.Result);
                    }
                });
            }
            this.pms.Add(pic);
            if (pms.Count == 1)
            {
                ShowPic();
            }
        }

        public void Clear()
        {
            this.pms.Clear();
        }

        private void ShowPic()
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                if (pms.Count == 0) return;
                if (currentNumber >= pms.Count - 1)
                    currentNumber = 0;
                else
                    currentNumber++;
                var pm = pms[currentNumber];
                try
                {
                    if (!string.IsNullOrEmpty(pm.localUrl))
                        mainImageBox.Source = new BitmapImage(new Uri(pm.localUrl));
                    else
                        mainImageBox.Source = new BitmapImage(new Uri(pm.ImgUrl));
                }
                catch
                {
                    mainImageBox.Source = new BitmapImage(new Uri(pm.ImgUrl));
                }
            }));
        }

        private void ShowPicPre()
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                if (pms.Count == 0) return;
                if (currentNumber <= 0)
                    currentNumber = pms.Count - 1;
                else
                    currentNumber--;
                var pm = pms[currentNumber];
                try
                {
                    if (!string.IsNullOrEmpty(pm.localUrl))
                        mainImageBox.Source = new BitmapImage(new Uri(pm.localUrl));
                    else
                        mainImageBox.Source = new BitmapImage(new Uri(pm.ImgUrl));
                }
                catch
                {
                    mainImageBox.Source = new BitmapImage(new Uri(pm.ImgUrl));
                }
            }));
        }

        private void mainImageBox_Loaded(object sender, RoutedEventArgs e)
        {
            currentNumber = -1;
            ShowPic();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Right)
            {
                ShowPic();
            }else if(e.Key == Key.Left)
            {
                ShowPicPre();
            }
        }
    }
}
