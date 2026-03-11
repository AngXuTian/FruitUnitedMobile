<%@ Page Title="" Language="C#" MasterPageFile="~/ModulesPage.Master" AutoEventWireup="true" CodeBehind="GRTN-Location.aspx.cs" Inherits="FruitUnitedMobile.Modules.GRTN_Location" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        table {
            width: 100%;
        }

        td {
            padding: 10px;
            vertical-align: middle;
        }

        .label {
            font-weight: bold;
        }

        .not-mandatory {
            color: red;
            font-size: 12px;
        }

        .form-control {
            width: 100%; /* Or a specific width like 300px */
        }

        .form-control2 {
            width: 200px;
        }

        .dropdown-input {
            width: 100%;
            cursor: pointer;
        }

        .dropdown-panel {
            position: absolute;
            width: 100%;
            border: 1px solid #ccc;
            background-color: white;
            z-index: 1000;
            padding: 5px;
        }

        .CheckBoxListItem input[type="checkbox"] {
            margin-right: 8px;
        }
    </style>
    <link href="../CSS/Basic.css" rel="stylesheet" />
    <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
    <link href="https://cdn.jsdelivr.net/npm/select2@4.1.0-rc.0/dist/css/select2.min.css" rel="stylesheet" />
    <script src="https://cdn.jsdelivr.net/npm/select2@4.1.0-rc.0/dist/js/select2.min.js"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div style="font-family: Arial, sans-serif;">
        <div class="parent" style="height: 100%;">
            <div class="form-container" style="padding-bottom: 80px">
                <table tyle="width: 100%;">
                    <tr>
                       <td class="TitleLabel">
                            <label class="label" for="ddlLocation">Location</label>
                        </td>
                        <td>
                            <asp:DropDownList ID="ddlLocation" CssClass="form-control dropdown-mobile" runat="server"></asp:DropDownList>
                        </td>
                    </tr>
                </table>
            </div>
            <div style="position: fixed; bottom: 0; left: 0; width: 100vw; background-color: white; border-top: 1px solid #ccc; padding: 10px;">
                <div style="display: flex; justify-content: space-between; align-items: center;">
                    <div>
                    </div>
                    <div>
                        <asp:Button ID="btnSubmit" runat="server" Text="Submit" OnClientClick="return validateLocation();" OnClick="btnSubmit_Click" CssClass="btn btn-success" Style="padding: 10px 20px; margin-right: 10px;" />
                    </div>
                </div>
            </div>
        </div>
    </div>
    <script>
        $(document).ready(function () {
            $('#<%= ddlLocation.ClientID %>').select2();
        });

        function validateLocation() {
            var val1 = $("#<%= ddlLocation.ClientID %>").val();

            if (val1 == "0") {
                alert("Please make sure location is selected.");
                return false;
            }
        }

</script>
</asp:Content>
