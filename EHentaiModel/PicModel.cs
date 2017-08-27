using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EHentaiModel
{
    public class PicModel : IComparable<PicModel>
    {
        public int Index { set; get; }
        public string ImgUrl { set; get; }
        public string localUrl { set; get; }

        public int CompareTo(PicModel other)
        {
            return this.Index - other.Index;
        }
    }
}
