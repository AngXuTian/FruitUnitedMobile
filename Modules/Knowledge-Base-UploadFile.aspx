<%@ Page Title="" Language="C#" MasterPageFile="~/ModulesPage.Master" AutoEventWireup="true" CodeBehind="Knowledge-Base-UploadFile.aspx.cs" Inherits="FruitUnitedMobile.Modules.KnowledgeBase_UploadFile" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .table tbody tr:hover {
            background-color: #f1f1f1;
        }

        .container .label-column {
            vertical-align: top;
        }

        @media (max-width: 768px) {
            .container .label-column {
                vertical-align: top; /* Align top in mobile view */
            }
        }
    </style>
    <link href="../CSS/Basic.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div style="font-family: Arial, sans-serif;">
        <div class="container mt-4 parent">
            <div class="form-container">
                <table>
                    <tr>
                        <td class="col-md-3">
                            <asp:Label runat="server" Text="Upload File:" AssociatedControlID="fileUploadPDF"></asp:Label>
                        </td>
                        <td class="col-md-9">
                            <asp:FileUpload ID="fileUploadPDF" runat="server" AllowMultiple="true" CssClass="form-control" Style="width: 100%; margin-bottom: 10px;" />
                        </td>
                    </tr>
                </table>
            </div>
        </div>

        <!-- Bottom Buttons Section -->
        <div style="position: fixed; bottom: 0; left: 0; width: 100vw; background-color: white; border-top: 1px solid #ccc; padding: 10px;">
            <div style="display: flex; justify-content: space-between; align-items: center;">
                <div>
                </div>
                <div>
                    <asp:Button ID="btnSubmit" runat="server" Text="Submit" OnClick="btnSubmit_Click" CssClass="btn btn-success" Style="padding: 10px 20px; margin-right: 10px;" />
                </div>
            </div>
        </div>
    </div>
    <script>
</script>
</asp:Content>


