using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EHentaiModel
{
    public class PhotoModel
    {
        public string Name { set; get; }
        public string Cover { set; get; }
        public string PublishDate { set; get; }
        public string Cagetory { set; get; }
        public int Score { set; get; }
        public string Uploader { set; get; }
        public string Url { set; get; }

        private List<PicModel> pics = new List<PicModel>();

        public void AddPicModel(PicModel pic)
        {
            pics.Add(pic);
        }

        public bool RemovePicModel(int index)
        {
            return pics.Remove(pics.First(x => x.Index == index));
        }

        public PicModel[] GetAllPicModel()
        {
            var temp = pics.ToList();
            temp.Sort();
            return temp.ToArray();
        }
    }
}
