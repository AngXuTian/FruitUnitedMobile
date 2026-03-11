<%@ Page Title="" Language="C#" MasterPageFile="~/ModulesPage.Master" AutoEventWireup="true" CodeBehind="Knowledge-Base-ViewFileList.aspx.cs" Inherits="FruitUnitedMobile.Modules.KnowledgeBase_ViewFileList" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="../CSS/Basic.css" rel="stylesheet" />
    <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
    <link href="https://cdn.jsdelivr.net/npm/select2@4.1.0-rc.0/dist/css/select2.min.css" rel="stylesheet" />
    <script src="https://cdn.jsdelivr.net/npm/select2@4.1.0-rc.0/dist/js/select2.min.js"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div style="font-family: Arial, sans-serif;">
        <div class="container">
            <div class="form-container" style="padding-top: 10px;">
                <div id="GridDiv">
                    <div class="container TitleDiv">
                        <div>
                            <asp:Label runat="server" Text="Title : " Font-Bold="true" AssociatedControlID="ProjectTitle"></asp:Label>
                            <asp:Label runat="server" ID="ProjectTitle" Font-Bold="true"></asp:Label>
                        </div>
                    </div>
                    <div class="col-12" id="OutputGridDiv" style="height: 85vh; overflow-y: auto;">
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
                    </div>
                    <asp:Label ID="ErrorMessage" runat="server" />
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
            </div>
        </div>
    </div>
    <link rel="stylesheet" href="https://code.jquery.com/ui/1.14.1/themes/base/jquery-ui.css">
    <script src="https://code.jquery.com/ui/1.14.1/jquery-ui.js"></script>
    <script>
        function clickButton() {
            $('button[id$=ViewPDFBtn]').click()
        }

        const clickBtn = setTimeout('clickButton()', 3000)

        $('button[id$=ViewPDFBtn]').click((e) => {
            let frame = $('iframe[id$=PreviewIFrame]')
            const params = new Proxy(new URLSearchParams(window.location.search), {
                get: (searchParams, prop) => searchParams.get(prop),
            })

            let IDvalue = params.ID
            let FileNamevalue = params.FileName

            frame.attr('src', '../Scripts/pdfjs-2.14.305-dist/web/viewer.html?file=/FruitUnitedMobileTest/Document/Knowledge_Base/File/' + IDvalue.replace('/', '_') + '/' + FileNamevalue.replace('/', '_'))
        })
    </script>
</asp:Content>
