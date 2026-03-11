<%@ Page Title="" Language="C#" MasterPageFile="~/ModulesPage.Master" AutoEventWireup="true" CodeBehind="Verification-Subcon.aspx.cs" Inherits="FruitUnitedMobile.Modules.Verification_Subcon" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
    .custom-rbl input[type="radio"] {
        margin-right: 5px; /* Space between the radio button and label */
    }
    .custom-rbl label {
        margin-right: 20px; /* Space between Yes and No */
    }
</style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
        <link href="../CSS/Basic.css" rel="stylesheet" />
    <div class="container" style="border:5px solid black; width:95%; overflow-x: hidden; background-color: white;">
     <div>
            <asp:Label runat="server" Font-Bold="true" Text="Project:" AssociatedControlID="txtProject"></asp:Label>
            <asp:Label runat="server" ID="txtProject"></asp:Label>
            <%--<asp:TextBox ID="txtProject" runat="server" Text="" ReadOnly="true" CssClass="form-control" style="width: 100%; margin-bottom: 10px;"></asp:TextBox>--%>
        </div>
    </div>
    <br><br><br>
    <div class="container" style="width:95%; overflow-x: hidden; background-color: white;">
    <table style="width: 100%; border-collapse: collapse;">
        <tr>
            <td class="label-column" style="font-weight: bold;">Date:</td>
            <td>
                <asp:TextBox runat="server" ID="txtDate"></asp:TextBox>
              
            </td>
        </tr>
        <tr>
            <td class="label-column" style="font-weight: bold;">Is there any subcon involved:</td>
            <td>
                <asp:RadioButtonList ID="rblSubcon" CssClass="custom-rbl" runat="server" RepeatDirection="Horizontal">
                    <asp:ListItem Text="Yes" Value="Y" ></asp:ListItem>
                    <asp:ListItem Text="No" Value="N"></asp:ListItem>
                </asp:RadioButtonList>
            </td>
        </tr>
    </table>
          <asp:Label runat="server" ID="lblDateError" ForeColor="Red" Visible="false"></asp:Label>
</div>
    <div style="position: fixed; bottom: 0; left: 0; width: 100vw; background-color: white; border-top: 1px solid #ccc; padding: 10px;">
            <div style="display: flex; justify-content: space-between; align-items: center;">
                <div>
                    <asp:Button ID="btnToProject" runat="server" Text="Project" CausesValidation="false"  OnClick="btnToProject_Click" CssClass="btn btn-secondary" style="padding: 10px 20px;" />

                </div>
                <div>
                    <asp:Button ID="btnSubmit" runat="server" Text="✓" OnClick="btnSubmit_Click" CssClass="btn btn-success" style="padding: 10px 20px; margin-right:10px;" />                </div>
            </div>
        </div>

</asp:Content>
