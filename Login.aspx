<%@ Page Title="FruitUnited Mobile" Language="C#" MasterPageFile="FruitUnitedMobile.Master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="FruitUnitedMobile.Modules.Login" %>
<%@ Register Src="~/Component/Toast.ascx" TagPrefix="uc" TagName="Toast" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">

    <style>
        * { 
            box-sizing: border-box; 
            margin: 0;
            padding: 0;
        }
        
        body, html {
            height: 100%;
            font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif;
            background: #f5f5f5;
        }

        .page {
            min-height: 100vh;
            display: flex;
            align-items: center;
            justify-content: center;
            padding: 20px;
        }

        .card {
            background: white;
            width: 100%;
            max-width: 420px;
            border-radius: 12px;
            box-shadow: 0 2px 8px rgba(0,0,0,0.1);
            padding: 48px 36px;
        }

        .logo {
            width: 180px;
            height: 180px;
            margin: 0 auto 32px;
            background: white;
            border-radius: 50%;
            padding: 12px;
            box-shadow: 0 2px 12px rgba(0,0,0,0.08);
            border: 3px solid #f5f5f5;
            display: flex;
            align-items: center;
            justify-content: center;
        }

        .logo img {
            width: 100%;
            height: 100%;
            object-fit: contain;
            border-radius: 40%;
        }

        h4 {
            margin: 0 0 32px 0;
            color: #1a1a1a;
            font-size: 24px;
            font-weight: 600;
            text-align: center;
        }

        .field {
            margin-bottom: 20px;
        }

        .field label {
            display: block;
            margin-bottom: 8px;
            font-weight: 500;
            color: #333;
            font-size: 14px;
        }

        .field input, 
        .field select {
            width: 100%;
            height: 48px;
            padding: 0 14px;
            border: 1px solid #ddd;
            border-radius: 8px;
            font-size: 15px;
            background: white;
            color: #1a1a1a;
            transition: border-color 0.2s;
        }

        .field input:focus, 
        .field select:focus {
            outline: none;
            border-color: #28a745;
        }

        .field select {
            cursor: pointer;
        }

        .btn {
            width: 100%;
            height: 48px;
            background: #28a745;
            color: white;
            border: none;
            border-radius: 8px;
            font-size: 16px;
            font-weight: 600;
            cursor: pointer;
            margin-top: 8px;
            transition: background 0.2s;
        }

        .btn:hover {
            background: #218838;
        }

        .btn:active {
            background: #1e7e34;
        }

        .error {
            color: #dc3545;
            font-weight: 500;
            margin-top: 20px;
            min-height: 20px;
            font-size: 14px;
            text-align: center;
        }

        .error:empty {
            display: none;
        }

        @media (max-width: 480px) {
            .page {
                padding: 0;
            }

            .card {
                max-width: 100%;
                min-height: 100vh;
                border-radius: 0;
                box-shadow: none;
                padding: 36px 24px;
                display: flex;
                flex-direction: column;
                justify-content: center;
            }

            h4 {
                font-size: 22px;
            }
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="page">
        <uc:Toast ID="Toast1" runat="server" />
        <div class="card">
            <div class="logo">
                <img src="Images/CompLogo.jpg" alt="FruitUnited" />
            </div>

            <h4>Employee Login</h4>

            <div class="field">
                <label>Primary Driver <span style="color:#dc3545;">*</span></label>
                <asp:DropDownList ID="Driver1DDL" runat="server"></asp:DropDownList>
            </div>

            <div class="field">
                <label>Secondary Driver (Optional)</label>
                <asp:DropDownList ID="Driver2DDL" runat="server"></asp:DropDownList>
            </div>

            <div class="field">
                <label>Vehicle <span style="color:#dc3545;">*</span></label>
                <asp:DropDownList ID="VehicleDDL" runat="server"></asp:DropDownList>
            </div>

            <div class="field">
                <label>Password <span style="color:#dc3545;">*</span></label>
                <asp:HiddenField ID="PasswordHidden" runat="server" />
                <input id="Password" runat="server" type="password" placeholder="Enter password" />
            </div>

            <div id="FAValidationDiv" runat="server" visible="false" class="field">
                <label>2FA Code</label>
                <input id="FAPassword" runat="server" type="text" placeholder="Enter code" maxlength="9" />
            </div>

            <input type="submit"
                   runat="server"
                   onserverclick="LoginBtn_Click"
                   value="Sign In"
                   class="btn" />

            <div class="error">
                <asp:Label ID="ErrorLabel" runat="server" />
                <asp:Label ID="Test" runat="server" />
            </div>
        </div>
    </div>
</asp:Content>