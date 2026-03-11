<%@ Page Title="Comnet Mobile" Language="C#" MasterPageFile="~/ModulesPage.Master" AutoEventWireup="true" CodeBehind="User-Password.aspx.cs" Inherits="FruitUnitedMobile.Modules.User_Password" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link rel="stylesheet" href="../CSS/Menu.css" />
    <script type="text/javascript" src="../JS/DOList.js" defer="defer"></script>
        <link href="../CSS/Basic.css" rel="stylesheet" />
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

        /* Adjust page content to avoid overlap with the fixed menu */
        .content {
            margin-top: 10px;
            margin-bottom: 30px;
        }

        .btn-custom:hover {
            opacity: 0.9;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container content text-center">
        <div style="font-family: Arial, sans-serif;">
            <div class="container mt-4">
                <div class="form-container">
                    <table>
                        <tr>
                            <td style="text-align: left; padding-bottom:20px;">
                                <label style="text-align: left;font-weight:bold;" id="UserName" runat="server"></label>
                            </td>
                        </tr>
                        <tr>
                            <td style="text-align: left">
                                <label for="CurrentPasswordText" style="text-align: left">Current Password:</label></td>
                            <td>
                                <asp:TextBox ID="CurrentPasswordText" CssClass="form-control input-control" runat="server" TextMode="Password"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td style="text-align: left">
                                <label for="ConfirmPasswordText1" style="text-align: left">New Password:</label></td>
                            <td>
                                <asp:TextBox ID="ConfirmPasswordText1" CssClass="form-control input-control" runat="server" TextMode="Password"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td style="text-align: left">
                                <label for="ConfirmPasswordText2">Confirm New Password:</label></td>
                            <td>
                                <asp:TextBox ID="ConfirmPasswordText2" CssClass="form-control input-control" runat="server" TextMode="Password"></asp:TextBox>
                            </td>
                        </tr>
                    </table>
                    <asp:Label ID="errormessage" runat="server"></asp:Label>
                </div>
            </div>
        </div>
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
</asp:Content>
