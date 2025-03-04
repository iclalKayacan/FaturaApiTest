using FaturaApi.Helpers;
using FaturaApi.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace FaturaApi
{
    public partial class SalesList : Page
    {
        protected async void btnGetToken_Click(object sender, EventArgs e)
        {
            try
            {
                string token = await GetTokenAsync();
                Session["BirFaturaToken"] = token;
                Response.Write("Token alındı, session'a yazıldı: " + token);
            }
            catch (Exception ex)
            {
                Response.Write("Token alınamadı: " + ex.Message);
            }
        }

        protected async void btnLoadFatura_Click(object sender, EventArgs e)
        {
            var token = Session["BirFaturaToken"] as string;
            if (string.IsNullOrEmpty(token))
            {
                Response.Write("Önce 'Token Al' butonuna basınız!");
                return;
            }

            try
            {
                List<FaturaModel> faturaList = await SalesHelper.GetFaturaListAsync(token);

                List<FlatFaturaUrun> flatList = SalesHelper.FlattenFaturalar(faturaList);

                gvFaturaUrun.DataSource = flatList;
                gvFaturaUrun.DataBind();

                Session["FlatData"] = flatList;
            }
            catch (Exception ex)
            {
                Response.Write("Hata: " + ex.Message);
            }
        }

        protected void gvFaturaUrun_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "CreateInvoice")
            {
                string faturaIdString = e.CommandArgument.ToString();
                int faturaId = int.Parse(faturaIdString);
                List<FlatFaturaUrun> allData = gvFaturaUrun.DataSource as List<FlatFaturaUrun>;
                if (allData == null) allData = Session["FlatData"] as List<FlatFaturaUrun>;
                if (allData == null) return;
                var seciliFaturaSatirlari = allData.FindAll(x => x.FaturaID == faturaId);
                if (seciliFaturaSatirlari.Count == 0) return;
                using (MemoryStream ms = new MemoryStream())
                {
                    BaseFont bf = BaseFont.CreateFont("C:\\Windows\\Fonts\\Arial.ttf", BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                    iTextSharp.text.Font titleFont = new iTextSharp.text.Font(bf, 14, iTextSharp.text.Font.NORMAL);
                    iTextSharp.text.Font normalFont = new iTextSharp.text.Font(bf, 12, iTextSharp.text.Font.NORMAL);
                    Document doc = new Document(PageSize.A4);
                    PdfWriter writer = PdfWriter.GetInstance(doc, ms);
                    doc.Open();
                    Paragraph title = new Paragraph("FATURA", titleFont);
                    title.Alignment = Element.ALIGN_CENTER;
                    doc.Add(title);
                    doc.Add(new Paragraph(" ", normalFont));
                    doc.Add(new Paragraph("Fatura No: " + faturaId, normalFont));
                    doc.Add(new Paragraph("Müşteri Adı: " + seciliFaturaSatirlari[0].MusteriAdi, normalFont));
                    doc.Add(new Paragraph("Adres: " + seciliFaturaSatirlari[0].MusteriAdresi, normalFont));
                    doc.Add(new Paragraph("Tel: " + seciliFaturaSatirlari[0].MusteriTel, normalFont));
                    doc.Add(new Paragraph("Şehir: " + seciliFaturaSatirlari[0].MusteriSehir, normalFont));
                    doc.Add(new Paragraph("Vergi Dairesi: " + seciliFaturaSatirlari[0].MusteriVergiDairesi, normalFont));
                    doc.Add(new Paragraph("TC/VKN: " + seciliFaturaSatirlari[0].MusteriTCVKN, normalFont));
                    doc.Add(new Paragraph(" ", normalFont));
                    Paragraph altBaslik = new Paragraph("Satılan Ürünler:", titleFont);
                    doc.Add(altBaslik);
                    doc.Add(new Paragraph(" ", normalFont));
                    PdfPTable table = new PdfPTable(5);
                    table.WidthPercentage = 100;
                    PdfPCell c1 = new PdfPCell(new Phrase("Ürün Adı", titleFont));
                    c1.HorizontalAlignment = Element.ALIGN_CENTER;
                    c1.BackgroundColor = BaseColor.LIGHT_GRAY;
                    table.AddCell(c1);
                    PdfPCell c2 = new PdfPCell(new Phrase("Stok Kodu", titleFont));
                    c2.HorizontalAlignment = Element.ALIGN_CENTER;
                    c2.BackgroundColor = BaseColor.LIGHT_GRAY;
                    table.AddCell(c2);
                    PdfPCell c3 = new PdfPCell(new Phrase("Adet", titleFont));
                    c3.HorizontalAlignment = Element.ALIGN_CENTER;
                    c3.BackgroundColor = BaseColor.LIGHT_GRAY;
                    table.AddCell(c3);
                    PdfPCell c4 = new PdfPCell(new Phrase("KDV (%)", titleFont));
                    c4.HorizontalAlignment = Element.ALIGN_CENTER;
                    c4.BackgroundColor = BaseColor.LIGHT_GRAY;
                    table.AddCell(c4);
                    PdfPCell c5 = new PdfPCell(new Phrase("Fiyat (KDV Dahil)", titleFont));
                    c5.HorizontalAlignment = Element.ALIGN_CENTER;
                    c5.BackgroundColor = BaseColor.LIGHT_GRAY;
                    table.AddCell(c5);
                    foreach (var urun in seciliFaturaSatirlari)
                    {
                        table.AddCell(new PdfPCell(new Phrase(urun.UrunAdi, normalFont)));
                        table.AddCell(new PdfPCell(new Phrase(urun.StokKodu, normalFont)));
                        table.AddCell(new PdfPCell(new Phrase(urun.SatisAdeti.ToString(), normalFont)));
                        table.AddCell(new PdfPCell(new Phrase(urun.KDVOrani.ToString(), normalFont)));
                        table.AddCell(new PdfPCell(new Phrase(urun.KDVDahilBirimFiyati.ToString("N2"), normalFont)));
                    }
                    doc.Add(table);
                    doc.Close();
                    writer.Close();
                    byte[] pdfBytes = ms.ToArray();
                    Response.Clear();
                    Response.ContentType = "application/pdf";
                    Response.AddHeader("content-disposition", "attachment;filename=Fatura_" + faturaId + ".pdf");
                    Response.Buffer = true;
                    Response.Cache.SetCacheability(System.Web.HttpCacheability.NoCache);
                    Response.BinaryWrite(pdfBytes);
                    Response.End();
                }
            }
        }


        private async System.Threading.Tasks.Task<string> GetTokenAsync()
        {
            using (var client = new System.Net.Http.HttpClient())
            {
                var values = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string,string>("grant_type","password"),
                    new KeyValuePair<string,string>("username","test@test.com"),
                    new KeyValuePair<string,string>("password","Test123."),
                };

                var content = new System.Net.Http.FormUrlEncodedContent(values);
                var response = await client.PostAsync("http://istest.birfatura.net/Token", content);
                response.EnsureSuccessStatusCode();

                var responseString = await response.Content.ReadAsStringAsync();
                dynamic tokenObj = Newtonsoft.Json.JsonConvert.DeserializeObject(responseString);
                string token = tokenObj.access_token;

                return token;
            }
        }
    }
}
