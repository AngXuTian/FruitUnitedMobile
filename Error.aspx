<%@ Page Title="FruitUnited Mobile" Language="C#" MasterPageFile="FruitUnitedMobile.Master" AutoEventWireup="true" CodeBehind="Error.aspx.cs" Inherits="FruitUnitedMobile.Modules.Login" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link rel="stylesheet" href="CSS/Login.css" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container">
        <div class="row" style="text-align:center;">
            <div>
                <asp:Label runat="server" Text="Please access the page using mobile device."></asp:Label>
            </div>
        </div>
    </div>
</asp:Content>
