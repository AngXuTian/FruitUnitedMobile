<%@ Page Title="FruitUnited Mobile" Language="C#" MasterPageFile="~/ModulesPage.Master"
    AutoEventWireup="true"
    CodeBehind="Menu.aspx.cs" 
    Inherits="FruitUnitedMobile.Modules.Menu" %>
<%@ Register Src="~/Component/Toast.ascx" TagPrefix="uc" TagName="Toast" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="../CSS/Basic.css" rel="stylesheet" />
    <link rel="stylesheet" href="../CSS/Menu.css" />
    <script type="text/javascript" src="../JS/DOList.js" defer="defer"></script>
    <style>
        /* Style for the fixed top menu */
        .top-menu {
            position: fixed;
            top: 0;
            left: 0;
            width: 100%;
            background-color: white;
            z-index: 1000;
            padding: 10px 15px;
            border-bottom: 2px solid #ccc;
        }

        .btn-custom:hover {
            opacity: 0.9;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container text-center">
        <uc:Toast ID="Toast1" runat="server" />
        <div class="d-grid gap-3" style="padding-bottom: 50px; padding-top: 50px;">
            <asp:PlaceHolder ID="ModuleContainer" runat="server"></asp:PlaceHolder>
        </div>
    </div>


</asp:Content>

