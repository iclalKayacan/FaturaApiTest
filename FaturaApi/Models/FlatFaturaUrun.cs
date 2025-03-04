using System;

namespace FaturaApi.Models
{
    public class FlatFaturaUrun
    {
        public int FaturaID { get; set; }
        public string MusteriAdi { get; set; }
        public string MusteriAdresi { get; set; }
        public string MusteriTel { get; set; }
        public string MusteriSehir { get; set; }
        public string MusteriTCVKN { get; set; }
        public string MusteriVergiDairesi { get; set; }

        public int UrunID { get; set; }
        public string UrunAdi { get; set; }
        public string StokKodu { get; set; }
        public int SatisAdeti { get; set; }
        public int KDVOrani { get; set; }
        public decimal KDVDahilBirimFiyati { get; set; }
    }
}
