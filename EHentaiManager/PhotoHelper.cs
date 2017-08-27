using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using HtmlAgilityPack;
using EHentaiModel;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.IO;

namespace EHentaiManager
{
    public class PhotoHelper
    {
        //https://e-hentai.org/?page=1&f_doujinshi=on&f_apply=Apply+Filter
        public static string HOSTADDRESS = "https://e-hentai.org/";
        private int threadPoolSize = 10;
        private bool isDownloading = false;
        public int ThreadPoolSize
        {
            set
            {
                threadPoolSize = value;
            }get
            {
                return threadPoolSize;
            }
        }
        public event Action<string> MsgBox;
        public event Action<int, int> DownloadProgress;
        public event Action<PhotoModel[]> ProcessComplite;
        public event Action<PageModel[]> PageComplite;
        public event Action<PicModel> AddPicture;

        public PhotoModel[] GetModelByIndex(int pageIndex = 0)
        {
            var url = string.Format(HOSTADDRESS + "?page={0}", pageIndex);
            var htmlContent = GetPageString(url);
            var result = GetModelByHtmlContent(htmlContent);
            return result;
        }

        //未实现的方法
        public PhotoModel[] GetModelByCagetory(string cagetory,string keyWord,int pageIndex = 0)
        {
            var url = string.Format(HOSTADDRESS + "?page={0}", pageIndex);
            //分类是在后面添加参数
            StringBuilder sb = new StringBuilder(url);
            if (!string.IsNullOrEmpty(cagetory))
                sb.AppendFormat("&f_{0}=on", cagetory);
            if (!string.IsNullOrEmpty(keyWord))
                sb.AppendFormat("&f_search={0}", keyWord);
            if (!string.IsNullOrEmpty(cagetory) || !string.IsNullOrEmpty(keyWord))
                sb.Append("&f_apply=Apply+Filter");
            url = sb.ToString();
            var htmlContent = GetPageString(url);
            return GetModelByHtmlContent(htmlContent);
        }

        public void GetModelByCagetoryAsync(string cagetory, string keyWord,int pageIndex = 0)
        {
            Task.Factory.StartNew<PhotoModel[]>(() =>
            {
                return GetModelByCagetory(cagetory, keyWord, pageIndex);
            }).ContinueWith((x) =>
            {
                if (x.Status == TaskStatus.RanToCompletion)
                    ProcessComplite.Invoke(x.Result);
            });
        }

        public void GetModelByIndexAsync(int pageIndex = 0)
        {
            Task.Factory.StartNew<PhotoModel[]>(() =>
            {
                var url = string.Format(HOSTADDRESS + "?page={0}", pageIndex);
                var htmlContent = GetPageString(url);
                return GetModelByHtmlContent(htmlContent);
            }).ContinueWith((x) =>
            {
                if(x.Status == TaskStatus.RanToCompletion)
                {
                    ProcessComplite?.Invoke(x.Result);
                }
            });
        }

        public PhotoModel[] GetModelByHtmlContent(string htmlContent)
        {
            if (htmlContent == "") return null;
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(htmlContent);
            var rootNode = doc.DocumentNode;
            var tableNode = rootNode.SelectSingleNode("//table[@class='itg']");
            var trNodes = tableNode.ChildNodes;
            List<PhotoModel> phms = new List<PhotoModel>();
            foreach(var x in trNodes)
            {
                if (x.HasAttributes)
                {
                    var type = x.SelectSingleNode("./td[1]/a/img[@alt]").GetAttributeValue("alt", "unknown");
                    var publishDate = x.SelectSingleNode("./td[2]").InnerText;
                    var name = x.SelectSingleNode("./td[3]/div/div[@class='it5']/a").InnerText;
                    name = name.Replace("&gt;", ">").Replace("&lt;", "<").Replace("&nbsp;", " ").Replace("&quot;", ((char)34) + "").Replace("&#39;", ((char)39) + "");
                    name = name.Replace("\"", "").Replace("?", "").Replace("!", "");
                    var url = x.SelectSingleNode("./td[3]/div/div[@class='it5']/a").GetAttributeValue("href", "");
                    var imgNode = x.SelectSingleNode("./td[3]/div/div[@class='it2']/img");
                    var titleImg = "";
                    if (imgNode != null && imgNode.Name == "img")
                    {
                        //如果直接是图片标签，则直接取src内容即可
                        titleImg = imgNode.GetAttributeValue("src", "");
                    }
                    else
                    {
                        var otherInfo = x.SelectSingleNode("./td[3]/div/div[@class='it2']").InnerText;
                        var info = otherInfo.Split("~".ToArray(), StringSplitOptions.RemoveEmptyEntries);
                        //拆分后 其中第一个固定为inits 第二个为服务器地址 第三个为图片链接地址
                        //评分由于是使用的背景图片的方式 所以不采集了
                        if (info[0] == "inits")
                        {
                            titleImg = string.Format("https://{0}/{1}", info[1], info[2]);
                        }
                    }
                    var author = x.SelectSingleNode("./td[@class='itu']/div/a").InnerHtml;
                    PhotoModel pm = new PhotoModel() { Name = name, Cagetory = type, Cover = titleImg, PublishDate = publishDate, Url = url,Uploader = author };
                    phms.Add(pm);
                }
            }
            return phms.ToArray();
        }

        public void GetPicModelAsync(string pageUrl)
        {
            Task.Factory.StartNew(() =>
            {
                var url = pageUrl;
                var client = new WebClient();
                var pageSize = this.GetPageCount(GetPageString(pageUrl));
                List<PicModel> pms = new List<PicModel>();
                Regex reg = new Regex(@"<div class=""gdtm"".*?<a href=""(.*?)"".*?</div>");
                var ChildReg = new Regex(@"<div id=""i3"">.*?<img id=""img"" src=""(.*?)"".*?</div>");
                int index = 0;
                //先读取所有的链接之后再进行下载，可以得到进度
                List<string> links = new List<string>();
                for (var i = 0; i < pageSize; i++)
                {
                    var masterPageHtml = client.DownloadString(string.Format(url, i));
                    if (reg.IsMatch(masterPageHtml))
                    {
                        foreach (Match m in reg.Matches(masterPageHtml))
                        {
                            //解析LINK链接 从新的html中找到Image的地址并且下载图片
                            var downloadClient = new WebClient();
                            var ChildPageHtml = downloadClient.DownloadString(m.Groups[1].Value);
                            if (ChildReg.IsMatch(ChildPageHtml))
                            {
                                Match mm = ChildReg.Match(ChildPageHtml);
                                var imgLink = mm.Groups[1].Value;
                                var pm = new PicModel() { ImgUrl = imgLink, Index = index++ };
                                AddPicture?.Invoke(pm);
                            }
                        }
                    }
                }
            });
        }

        public void Download(string pageUrl,string name)
        {
            if (isDownloading)
            {
                MsgBox?.Invoke("已有任务正在下载中");
                return;
            }
            new System.Threading.Thread(() =>
            {
                isDownloading = true;
                var url = pageUrl;
                var CurrentThreadSize = 0;
                var client = new WebClient();
                var pageSize = this.GetPageCount(GetPageString(pageUrl));
                if (!Directory.Exists(name))
                {
                    Directory.CreateDirectory(name);
                }
                Regex reg = new Regex(@"<div class=""gdtm"".*?<a href=""(.*?)"".*?</div>");
                //先读取所有的链接之后再进行下载，可以得到进度
                List<string> links = new List<string>();
                for (var i = 0; i < pageSize; i++)
                {
                    var masterPageHtml = client.DownloadString(string.Format(url, i));
                    if (reg.IsMatch(masterPageHtml))
                    {
                        foreach (Match m in reg.Matches(masterPageHtml))
                        {
                            links.Add(m.Groups[1].Value);
                        }
                    }
                    while (CurrentThreadSize >= ThreadPoolSize)
                    {
                        System.Threading.Thread.Sleep(500);
                    }
                }
                //开始进行下载
                var ChildReg = new Regex(@"<div id=""i3"">.*?<img id=""img"" src=""(.*?)"".*?</div>");
                int max = links.Count;
                int value = 0;
                foreach (var x in links)
                {
                    while (CurrentThreadSize > ThreadPoolSize)
                    {
                        System.Threading.Thread.Sleep(500);
                    }
                    CurrentThreadSize++;
                    Task.Factory.StartNew(() =>
                    {
                        var downloadClient = new WebClient();
                        //解析LINK链接 从新的html中照到Image的地址并且下载图片
                        var ChildPageHtml = downloadClient.DownloadString(x);
                        if (ChildReg.IsMatch(ChildPageHtml))
                        {
                            Match m = ChildReg.Match(ChildPageHtml);
                            var imgLink = m.Groups[1].Value;
                            var tryNumer = 0;
                            while (tryNumer < 3)
                            {
                                try
                                {
                                    downloadClient.DownloadFile(imgLink, name + "\\" + Path.GetFileName(imgLink));
                                    break;
                                }
                                catch
                                {
                                    //重试三次
                                    tryNumer++;
                                    System.Threading.Thread.Sleep(3000);
                                }
                            }
                        }

                    }, TaskCreationOptions.LongRunning).ContinueWith((c) =>
                     {
                         if (c.Status == TaskStatus.RanToCompletion)
                         {
                             CurrentThreadSize--;
                             DownloadProgress?.Invoke(value++, max);
                         }
                     });
                }
                System.Threading.Thread.Sleep(5000);
                while (CurrentThreadSize > 0)
                {
                    System.Threading.Thread.Sleep(1000);
                }
                DownloadProgress?.Invoke(max, max);
                MsgBox?.Invoke(name + "下载完成");
                isDownloading = false;
                DownloadProgress?.Invoke(0, max);
            }){ IsBackground = true }.Start();
        }

        private int GetPageCount(string htmlContent)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(htmlContent);
            var rootNode = doc.DocumentNode;
            var targetNode = rootNode.SelectSingleNode("//table[@class='ptt']/tr[1]/td[last()-1]/a");
            if (targetNode == null) return 1;
            int pageCount = 1;
            if(int.TryParse(targetNode.InnerText,out pageCount))
            {
                return pageCount;
            }
            return 1;
        }

        private int GetPageCount()
        {
            return GetPageCount(GetPageString(HOSTADDRESS));
        }

        public PageModel[] GetPageModels(int currentPage)
        {
            var mainPageCount = GetPageCount();
            List<PageModel> pms = new List<PageModel>();
            if (mainPageCount > 10)
            {
                for (var i = (currentPage - 5 >= 0 ? currentPage - 5 : 0); i < 5; i++)
                {
                    pms.Add(new PageModel() { pageId = i, showString = (i + 1) + "" });
                }
                while (pms.Count < 10 && pms.Last().pageId < mainPageCount)
                {
                    pms.Add(new PageModel() { pageId = pms.Last().pageId + 1, showString = (pms.Last().pageId + 2) + "" });
                }
                //然后添加...和最后一页
                pms.Add(new PageModel() { pageId = mainPageCount - 1, showString = "..." });
                pms.Add(new PageModel() { pageId = mainPageCount, showString = (mainPageCount + 1) + "" });
            }
            else
            {
                for (var i = 0; i < mainPageCount; i++)
                {
                    pms.Add(new PageModel() { pageId = i, showString = (i + 1) + "" });
                }
            }
            return pms.ToArray();
        }

        public void GetPageModelAsync(int currentPage)
        {
            Task.Factory.StartNew<PageModel[]>(() =>
            {
                return GetPageModels(currentPage);
            }).ContinueWith((x) =>
            {
                if (x.Status == TaskStatus.RanToCompletion)
                    PageComplite.Invoke(x.Result);
            });
        }

        private string GetPageString(string url)
        {
            try
            {
                WebClient client = new WebClient();
                return client.DownloadString(new Uri(url));
            }
            catch (WebException we)
            {
                //记录日志
                return "";
            }
            catch (NotSupportedException nse)
            {
                //记录日志
                return "";
            }
        }
    }
}
