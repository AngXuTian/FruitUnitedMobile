<%@ Page Title="" Language="C#" MasterPageFile="~/ModulesPage.Master" AutoEventWireup="true" CodeBehind="Equipment-ViewFile.aspx.cs" Inherits="FruitUnitedMobile.Modules.Equipment_ViewFile" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
        <link href="../CSS/Basic.css" rel="stylesheet" />
    <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
    <link href="https://cdn.jsdelivr.net/npm/select2@4.1.0-rc.0/dist/css/select2.min.css" rel="stylesheet" />
    <script src="https://cdn.jsdelivr.net/npm/select2@4.1.0-rc.0/dist/js/select2.min.js"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div style="font-family: Arial, sans-serif;">
        <div class="container mt-4">
            <div class="form-container">
                <div id="GridDiv" class="container" style="height: 200px;">
                    <div class="row" style="margin-top: 10px;">
                        <div class="col-12" id="OutputGridDiv" style="overflow: auto;">
                            <asp:GridView ID="ViewFileGrid" runat="server" AutoGenerateColumns="false" OnRowDataBound="EquipmentFileGrid_RowDataBound" CssClass="table table-bordered table-responsive">
                                <Columns>
                                    <asp:TemplateField HeaderText="File Name">
                                        <ItemTemplate>
                                            <asp:Label ID="Name" Text='<%# Bind("Name") %>' Font-Size="Small" runat="server"></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="" HeaderStyle-Width="20%" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                                        <ItemTemplate>
                                           <asp:LinkButton ID="lnkDownload" runat="server" Text="<i class='fa fa-download' aria-hidden='true'></i>" OnClick="lnkDownload_Click"></asp:LinkButton>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                            <div></div>
                        </div>
                    </div>
                </div>
            </div>
            <div id="iFrameDiv" runat="server" class="container" style="margin-top: 10px;">
                <div class="row">
                    <div id="ViewPDFBtnDiv">
                        <button type="button" id="ViewPDFBtn" class="BackNextButtons" style="display: none">View PDF</button>
                    </div>
                </div>
                <div class="row">
                    <div id="PDFViewer" style="text-align: center;" class="col-sm-12 col-md-12 col-lg-12">
                        <iframe id="DownloadIFrame" runat="server" style="display: none"></iframe>
                        <iframe id="PreviewIFrame" runat="server" style="width: 100%;"></iframe>
                    </div>
                </div>
            </div>
            <div id="ImageDiv" runat="server" class="container" style="margin-top: 10px;">
                <img id="ImageFrame" src="~/Document/test.png" runat="server" style="width: 100%; height: 100%; object-fit: scale-down;" />
            </div>
            <asp:Label ID="ErrorMessage" runat="server" />
        </div>
        <br>
        <br>
        <br>
        <br>
        <!-- Bottom Buttons Section -->
        <div style="position: fixed; bottom: 0; left: 0; width: 100vw; background-color: white; border-top: 1px solid #ccc; padding: 10px;">
            <div style="display: flex; justify-content: space-between; align-items: center;">
                <div>
                </div>
                <div>
                    <asp:Button ID="btnViewImage" runat="server" Text="Back" OnClick="btnBack_Click" BackColor="Green" ForeColor="White" CssClass="btn" Style="padding: 10px 20px; margin-right: 10px;" />
                </div>
            </div>
        </div>
    </div>
    <link rel="stylesheet" href="https://code.jquery.com/ui/1.14.1/themes/base/jquery-ui.css">
    <script src="https://code.jquery.com/ui/1.14.1/jquery-ui.js"></script>
</asp:Content>
