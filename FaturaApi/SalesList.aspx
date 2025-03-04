<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SalesList.aspx.cs"
         Inherits="FaturaApi.SalesList" Async="true" %>
<!DOCTYPE html>
<html>
<head runat="server">
    <title>Fatura Tablosu</title>
    <link 
        href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" 
        rel="stylesheet" />
</head>
<body class="bg-light">
    <form runat="server">
        <div class="container py-4">
            <h1 class="mb-4">Fatura Tablosu</h1>

            <div class="d-flex gap-3 mb-3">
                <asp:Button ID="btnGetToken" runat="server" 
                            Text="Token Al" 
                            OnClick="btnGetToken_Click" 
                            CssClass="btn btn-primary" />
                
                <asp:Button ID="btnLoadFatura" runat="server" 
                            Text="Faturaları yükle" 
                            OnClick="btnLoadFatura_Click" 
                            CssClass="btn btn-success" />
            </div>

            <asp:GridView ID="gvFaturaUrun" runat="server" 
                          AutoGenerateColumns="False"
                          OnRowCommand="gvFaturaUrun_RowCommand"
                          CssClass="table table-striped table-bordered">

                <Columns>
                    <asp:BoundField DataField="FaturaID" HeaderText="Fatura ID" />
                    <asp:BoundField DataField="MusteriAdi" HeaderText="Müşteri Adı" />
                    <asp:BoundField DataField="MusteriAdresi" HeaderText="Adres" />
                    <asp:BoundField DataField="MusteriTel" HeaderText="Tel" />
                    <asp:BoundField DataField="MusteriSehir" HeaderText="Şehir" />
                    <asp:BoundField DataField="MusteriTCVKN" HeaderText="TC/VKN" />
                    <asp:BoundField DataField="MusteriVergiDairesi" HeaderText="Vergi Dairesi" />

                    <asp:BoundField DataField="UrunID" HeaderText="Ürün ID" />
                    <asp:BoundField DataField="UrunAdi" HeaderText="Ürün Adı" />
                    <asp:BoundField DataField="StokKodu" HeaderText="Stok Kodu" />
                    <asp:BoundField DataField="SatisAdeti" HeaderText="Adet" />
                    <asp:BoundField DataField="KDVOrani" HeaderText="KDV (%)" />
                    <asp:BoundField DataField="KDVDahilBirimFiyati" HeaderText="Fiyat (KDV Dahil)" 
                                    DataFormatString="{0:C}" />

                    <asp:TemplateField HeaderText="İşlem">
                        <ItemTemplate>
                            <asp:Button ID="btnFaturalandir" runat="server" 
                                        Text="Faturalandır"
                                        CommandName="CreateInvoice"
                                        CommandArgument='<%# Eval("FaturaID") %>'
                                        CssClass="btn btn-warning" />
                        </ItemTemplate>
                    </asp:TemplateField>

                </Columns>
            </asp:GridView>
        </div>

        <script 
            src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/js/bootstrap.bundle.min.js">
        </script>
    </form>
</body>
</html>
