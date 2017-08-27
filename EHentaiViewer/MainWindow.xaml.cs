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
using System.Windows.Navigation;
using System.Windows.Shapes;
using EHentaiManager;
using EHentaiModel;

namespace EHentaiViewer
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        PhotoHelper ph = new PhotoHelper();
        ShowPhoto sh = null;
        private int currentPage = 0;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ph.ProcessComplite += Ph_ProcessComplite;
            ph.PageComplite += Ph_PageComplite;
            ph.AddPicture += Ph_ShowPicture;
            ph.GetModelByIndexAsync();
            ph.GetPageModelAsync(currentPage);
            ph.MsgBox += (x) =>
            {
                MessageBox.Show(x);
            };
            ph.DownloadProgress += (x, y) =>
            {
                this.Dispatcher.Invoke(new Action(() =>
                {
                    downloadPro.Value = x;
                    downloadPro.Maximum = y;
                }));
            };
            //加载下拉框的选择项
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("同人志", "doujinshi");
            dict.Add("漫画", "manga");
            dict.Add("插画", "artistcg");
            dict.Add("游戏CG", "gamecg");
            dict.Add("西方类型", "western");
            dict.Add("正常向", "non-h");
            dict.Add("图集", "imageset");
            dict.Add("cosplay", "cosplay");
            dict.Add("亚洲类型", "asianporn");
            dict.Add("其他", "misc");
            cagetoryCombo.ItemsSource = dict;
        }

        //显示图片方法
        private void Ph_ShowPicture(PicModel obj)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                sh.AddPicModel(obj);
            }));
        }

        //显示页面方法
        private void Ph_PageComplite(PageModel[] obj)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                StepList.ItemsSource = obj;
            }));
        }

        //显示进度方法
        private void Ph_ProcessComplite(PhotoModel[] obj)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                mainListBox.ItemsSource = obj;
            }));
        }

        private void mainListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sh != null)
                sh.Close();
            var item = mainListBox.SelectedItem;
            if (item == null) return;
            var pm = item as PhotoModel;
            sh = new ShowPhoto(pm.Name);
            ph.GetPicModelAsync(pm.Url);
            sh.Show();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var item = cagetoryCombo.SelectedValue;
            var keyword = KeyWord.Text;
            if (item == null)
                item = "";
            var kv = item.ToString();
            currentPage = 0;
            ph.GetModelByCagetoryAsync(kv, keyword, currentPage);
        }

        private void StepList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var tb = StepList.SelectedValue;
            if (tb != null)
            {
                var item = cagetoryCombo.SelectedValue;
                var keyword = KeyWord.Text;
                if (item == null)
                    item = "";
                var kv = item.ToString();
                var index = int.Parse(tb.ToString());
                currentPage = index;
                ph.GetModelByIndexAsync(index);
                ph.GetModelByCagetoryAsync(kv, keyword, currentPage);
                ph.GetPageModelAsync(currentPage);
            }
        }

        private void Button_Click_Download(object sender, RoutedEventArgs e)
        {
            var item = mainListBox.SelectedItem;
            if (item == null) return;
            var pm = item as PhotoModel;
            if (pm != null)
            {
                ph.Download(pm.Url, pm.Name);
            }
        }
    }
}
