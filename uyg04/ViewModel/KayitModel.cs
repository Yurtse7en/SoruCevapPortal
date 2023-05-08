using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace uyg04.ViewModel
{
    public class KayitModel
    {
        public string kayitId { get; set; }
        public string kayitCevapId { get; set; }
        public string kayitSoruId { get; set; }
        public string kayitKatId { get; set; }
        public SoruModel soruBilgi { get; set; }
        public CevapModel cevapBilgi { get; set; }
    }
}