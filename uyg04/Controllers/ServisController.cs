using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SoruCevap.Models;
using SoruCevap.ViewModel;

namespace SoruCevap.Controllers
{
    public class ServisController : ApiController
    {
        DB03Entities3 db = new DB03Entities3();
        SonucModel sonuc = new SonucModel();

        #region Soru
        [HttpGet]
        [Route("api/soruliste")]
        public List<SoruModel> SoruListe()
        {
            List<SoruModel> liste = db.Soru.Select(x => new SoruModel()
            {
                soruId = x.soruId,
                soruKatId = x.soruKatId,
                soru = x.soru
            }).ToList();
            return liste;
        }
        [HttpGet]
        [Route("api/sorubyid/{soruId}")]
        public SoruModel SoruById(string soruId)
        {
            SoruModel kayit = db.Soru.Where(s => s.soruId == soruId).Select(x => new SoruModel()
            {
                soruId = x.soruId,
                soruKatId = x.soruKatId,
                soru = x.soru,
            }).SingleOrDefault();
            return kayit;
        }

        [HttpPost]
        [Route("api/soruekle")]
        public SonucModel SoruEkle(SoruModel model)
        {
            if (db.Soru.Count(c => c.soru == model.soru) > 0)
            {
                sonuc.islem = false;
                sonuc.mesaj = "Girilen Soru Kayıtlıdır!";
                return sonuc;
            }

            Soru yeni = new Soru();
            yeni.soruId = Guid.NewGuid().ToString();
            yeni.soru = model.soru;
            yeni.soruKatId = model.soruKatId;
            db.Soru.Add(yeni);
            db.SaveChanges();
            sonuc.islem = true;
            sonuc.mesaj = "Soru Eklendi";

            return sonuc;
        }
        [HttpPut]
        [Route("api/soruduzenle")]
        public SonucModel SoruDuzenle(SoruModel model)
        {
            Soru kayit = db.Soru.Where(s => s.soruId == model.soruId).SingleOrDefault();

            if (kayit == null)
            {
                sonuc.islem = false;
                sonuc.mesaj = "Kayıt Bulunamadı!";
                return sonuc;
            }

            kayit.soru = model.soru;
            kayit.soruKatId = model.soruKatId;

            db.SaveChanges();
            sonuc.islem = true;
            sonuc.mesaj = "Soru Düzenlendi";

            return sonuc;
        }

        [HttpDelete]
        [Route("api/sorusil/{soruId}")]
        public SonucModel SoruSil(string soruId)
        {
            Soru kayit = db.Soru.Where(s => s.soruId == soruId).SingleOrDefault();

            if (kayit == null)
            {
                sonuc.islem = false;
                sonuc.mesaj = "Kayıt Bulunamadı!";
                return sonuc;
            }

            if (db.Kayit.Count(c => c.kayitSoruId == soruId) > 0)
            {
                sonuc.islem = false;
                sonuc.mesaj = "Cevaplanmis Soru Silinemez!";
                return sonuc;
            }

            db.Soru.Remove(kayit);
            db.SaveChanges();
            sonuc.islem = true;
            sonuc.mesaj = "Soru Silindi";

            return sonuc;
        }
        #endregion

        #region Kategori

        [HttpGet]
        [Route("api/kategoriliste")]
        public List<KategoriModel> KategoriListe()
        {
            List<KategoriModel> liste = db.Kategori.Select(x => new KategoriModel()
            {
                kategoriId = x.kategoriId,
                katKodu = x.katKodu,
                kategori = x.kategori,
                katSoruSay = x.Kayit.Count()
            }).ToList();
            return liste;
        }

        [HttpGet]
        [Route("api/kategoribyid/{kategoriId}")]
        public KategoriModel KatById(string kategoriId)
        {
            KategoriModel kayit = db.Kategori.Where(s => s.kategoriId == kategoriId).Select(x => new KategoriModel()
            {
                kategoriId = x.kategoriId,
                katKodu = x.katKodu,
                kategori = x.kategori,

            }).SingleOrDefault();
            return kayit;
        }

        [HttpPost]
        [Route("api/kategoriekle")]
        public SonucModel KategoriEkle(KategoriModel model)
        {
            if (db.Kategori.Count(c => c.katKodu == model.katKodu || c.kategori == model.kategori) > 0)
            {
                sonuc.islem = false;
                sonuc.mesaj = "Girilen Kategori Kayıtlıdır!";
                return sonuc;
            }

            Kategori yeni = new Kategori();
            yeni.kategoriId = Guid.NewGuid().ToString();
            yeni.kategori = model.kategori;
            yeni.katKodu = model.katKodu;
            db.Kategori.Add(yeni);
            db.SaveChanges();
            sonuc.islem = true;
            sonuc.mesaj = "Kategori Eklendi";

            return sonuc;
        }

        [HttpDelete]
        [Route("api/kategorisil/{kategoriId}")]
        public SonucModel KategoriSil(string kategoriId)
        {
            Kategori kayit = db.Kategori.Where(s => s.kategoriId == kategoriId).SingleOrDefault();

            if (kayit == null)
            {
                sonuc.islem = false;
                sonuc.mesaj = "Kategori Bulunamadı!";
                return sonuc;
            }

            if (db.Kayit.Count(c => c.kayitKatId == kategoriId) > 0)
            {
                sonuc.islem = false;
                sonuc.mesaj = "Soru Bulunduran Kategori Silinemez!";
                return sonuc;
            }

            db.Kategori.Remove(kayit);
            db.SaveChanges();
            sonuc.islem = true;
            sonuc.mesaj = "Kategori Silindi";

            return sonuc;
        }

        [HttpGet]
        [Route("api/cevapbyid/{cevapId}")]
        public CevapModel CevapById(string cevapId)
        {
            CevapModel kayit = db.Cevap.Where(s => s.cevapId == cevapId).Select(x => new CevapModel()
            {
                cevapId = x.cevapId,
                cevap = x.cevap
            }).SingleOrDefault();
            return kayit;
        }

        [HttpPut]
        [Route("api/cevapduzenle")]
        public SonucModel CevapDuzenle(CevapModel model)
        {
            Cevap kayit = db.Cevap.Where(s => s.cevapId == model.cevapId).SingleOrDefault();

            if (kayit == null)
            {
                sonuc.islem = false;
                sonuc.mesaj = "Kayıt Bulunamadı!";
                return sonuc;
            }

            kayit.cevap = model.cevap;

            db.SaveChanges();
            sonuc.islem = true;
            sonuc.mesaj = "Cevap Düzenlendi";
            return sonuc;
        }

        [HttpDelete]
        [Route("api/cevapsil/{cevapId}")]
        public SonucModel CevapSil(string cevapId)
        {
            Cevap kayit = db.Cevap.Where(s => s.cevapId == cevapId).SingleOrDefault();

            if (kayit == null)
            {
                sonuc.islem = false;
                sonuc.mesaj = "Cevap Bulunmadı!";
                return sonuc;
            }

            db.Cevap.Remove(kayit);
            db.SaveChanges();
            sonuc.islem = true;
            sonuc.mesaj = "Cevap Silindi";
            return sonuc;
        }

        #endregion

        #region Cevap

        [HttpGet]
        [Route("api/sorucevapliste/{soruId}")]
        public List<KayitModel> SoruCevapListe(string soruId)
        {
            List<KayitModel> liste = db.Kayit.Where(s => s.kayitSoruId == soruId).Select(x => new KayitModel()
            {
                kayitId = x.kayitId,
                kayitSoruId = x.kayitSoruId,
                kayitCevapId = x.kayitCevapId
            }).ToList();

            foreach (var kayit in liste)
            {
                kayit.cevapBilgi = CevapById(kayit.kayitCevapId);
                kayit.soruBilgi = SoruById(kayit.kayitSoruId);

            }
            return liste;

        }

        #endregion

        #region Kayıt
        [HttpPost]
        [Route("api/cevapekle")]
        public SonucModel Cevapla(KayitModel model)
        {
            if (db.Kayit.Count(c => c.kayitSoruId == model.kayitSoruId & c.kayitCevapId == model.kayitCevapId) > 0)
            {
                sonuc.islem = false;
                sonuc.mesaj = "Soru daha once soruldu!";
                return sonuc;
            }

            Kayit yeni = new Kayit();
            yeni.kayitId = Guid.NewGuid().ToString();
            yeni.kayitCevapId = model.kayitCevapId;
            yeni.kayitSoruId = model.kayitSoruId;
            db.Kayit.Add(yeni);
            db.SaveChanges();
            sonuc.islem = true;
            sonuc.mesaj = "Soru Cevaplandi";
            return sonuc;
        }
        #endregion

        //#region Uye

        //[HttpPost]
        //[Route("api/uyeEkle")]
        //public SonucModel UyeEkle(UyeModel model)
        //{
        //    if (db.Uye.Count(c => c.uyeMail == model.uyeMail) > 0)
        //    {
        //        sonuc.islem = false;
        //        sonuc.mesaj = "Girilen Mail Kullaniliyor!";
        //        return sonuc;
         

        //    Uye yeni = new Uye();
        //    yeni.uyeId = Guid.NewGuid().ToString();
        //    yeni.uyeAdsoyad = model.uyeAdsoyad;
        //    yeni.uyeMail = model.uyeMail;
        //    db.Uye.Add(yeni);
        //    db.SaveChanges();
        //    sonuc.islem = true;
        //    sonuc.mesaj = "Yeni Uye Eklendi";
        //    return sonuc;
        //    }
        //}

        //[HttpPut]
        //[Route("api/uyeduzenle")]
        //public SonucModel UyeDuzenle(UyeModel model)
        //{
        //    Cevap kayit = db.Uye.Where(s => s.uyeId == model.uyeId).SingleOrDefault();

        //    if (kayit == null)
        //    {
        //        sonuc.islem = false;
        //        sonuc.mesaj = "Kayıtli Uye Bulunamadı!";
        //        return sonuc;
        //    }

        //    kayit.uyeAdsoyad = model.uyeAdsoyad;

        //    db.SaveChanges();
        //    sonuc.islem = true;
        //    sonuc.mesaj = "Uye Bilgileri Düzenlendi";
        //    return sonuc;
        //}
        //#endregion
    }
}
