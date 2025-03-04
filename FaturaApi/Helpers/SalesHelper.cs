using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using FaturaApi.Models;  

namespace FaturaApi.Helpers
{
    public static class SalesHelper
    {
        private static string baseUrl = "http://istest.birfatura.net";

        public static async Task<List<FaturaModel>> GetFaturaListAsync(string token)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);

                var content = new StringContent("{}", Encoding.UTF8, "application/json");

                HttpResponseMessage response =
                    await client.PostAsync($"{baseUrl}/api/test/SatislarGetir", content);

                string responseBody = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception(
                        $"GetFaturaListAsync Error: {response.StatusCode}, {responseBody}");
                }

                var faturaList = JsonConvert.DeserializeObject<List<FaturaModel>>(responseBody);
                return faturaList;
            }
        }

        public static List<FlatFaturaUrun> FlattenFaturalar(List<FaturaModel> faturalar)
        {
            var result = new List<FlatFaturaUrun>();

            foreach (var fatura in faturalar)
            {
                if (fatura.SatilanUrunler != null)
                {
                    foreach (var urun in fatura.SatilanUrunler)
                    {
                        var flat = new FlatFaturaUrun
                        {
                            FaturaID = fatura.FaturaID,
                            MusteriAdi = fatura.MusteriAdi,
                            MusteriAdresi = fatura.MusteriAdresi,
                            MusteriTel = fatura.MusteriTel,
                            MusteriSehir = fatura.MusteriSehir,
                            MusteriTCVKN = fatura.MusteriTCVKN,
                            MusteriVergiDairesi = fatura.MusteriVergiDairesi,

                            UrunID = urun.UrunID,
                            UrunAdi = urun.UrunAdi,
                            StokKodu = urun.StokKodu,
                            SatisAdeti = urun.SatisAdeti,
                            KDVOrani = urun.KDVOrani,
                            KDVDahilBirimFiyati = urun.KDVDahilBirimFiyati
                        };
                        result.Add(flat);
                    }
                }
            }

            return result;
        }
    }
}
