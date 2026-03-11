<%@ Page Title="" Language="C#" MasterPageFile="~/ModulesPage.Master" AutoEventWireup="true" CodeBehind="Equipment-List.aspx.cs" Inherits="FruitUnitedMobile.Modules.Equipment_List" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        table {
            width: 100%;
        }

        td {
            padding: 10px;
            vertical-align: top;
        }

        .not-mandatory {
            color: red;
            font-size: 12px;
        }

        .form-control {
            width: 100%; /* Or a specific width like 300px */
        }

        .form-control1 {
            width: 105%;
        }

        .form-control2 {
            width: 200px;
        }

        .WorkerLabel {
            width: 40%;
            padding-left: 5px;
            padding-right: 5px;
            display: inline-block;
            vertical-align: middle;
        }

        .WorkerInfoLabel {
            width: 58%;
            height:100%;
            display: inline-block;
            vertical-align: middle;
        }
    </style>
    <link href="../CSS/Basic.css" rel="stylesheet" />
    <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
    <link href="https://cdn.jsdelivr.net/npm/select2@4.1.0-rc.0/dist/css/select2.min.css" rel="stylesheet" />
    <script src="https://cdn.jsdelivr.net/npm/select2@4.1.0-rc.0/dist/js/select2.min.js"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div style="font-family: Arial, sans-serif;">
        <div class="container mt-4">
            <div class="form-container">
                <table style="width: 100%;">
                    <!-- Type DDL-->
                    <tr>
                        <td style="width:20%">
                            <asp:Label runat="server" Text="Type" Font-Bold="true" AssociatedControlID="ddlEquipment"></asp:Label>
                        <td style="width:75%">
                            <asp:DropDownList ID="ddlType" AutoPostBack="true" runat="server" Style="width: 100%;" OnSelectedIndexChanged="TypeListDropDown_Change">
                                <asp:ListItem Text="-- Please Select --" Value="0" />
                            </asp:DropDownList>
                            <span id="ddlTypeError" style="color: red; display: none;">Please select Type.</span>
                        </td>
                    </tr>
                    <!-- Equipment DDL-->
                    <tr class="split-row">
                        <td style="width:20%">
                            <asp:Label runat="server" Text="Equipment" Font-Bold="true" AssociatedControlID="ddlEquipment"></asp:Label>
                        <td style="width:75%">
                            <asp:DropDownList ID="ddlEquipment" AutoPostBack="true" runat="server" Style="width: 100%;" OnSelectedIndexChanged="EquipmentListDropDown_Change">
                                <asp:ListItem Text="-- Please Select --" Value="0" />
                            </asp:DropDownList>
                            <span id="ddlEquipmentError" style="color: red; display: none;">Please select Equipment.</span>
                        </td>
                    </tr>
                    <!-- Location DDL -->
                    <tr class="split-row">
                        <td style="width:20%">
                            <asp:Label runat="server" Text="Location" Font-Bold="true" AssociatedControlID="CurrentLocationTB"></asp:Label>
                        </td>
                        <td style="width:75%">
                            <asp:TextBox ID="CurrentLocationTB" runat="server" Enabled="false" Width="100%"></asp:TextBox>
                        </td>
                    </tr>
                    <tr class="split-row">
                        <td style="width:20%">
                            <asp:Label runat="server" Text="Status" Font-Bold="true" AssociatedControlID="StatusTB"></asp:Label>
                        </td>
                        <td style="width:75%">
                            <asp:TextBox ID="StatusTB" runat="server" Enabled="false" Width="100%"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <div class="container TitleDiv">
                                <div>
                                    <div class="WorkerLabel">
                                        <asp:Label runat="server" Text="Worker" Font-Bold="true"></asp:Label>
                                    </div>
                                    <div class="WorkerInfoLabel">
                                        <asp:Label runat="server" ID="FirstWorkerName"  AssociatedControlID="CurrentLocationTB"></asp:Label>
                                    </div>
                                </div>
                                <div>
                                    <div class="WorkerLabel">
                                        <asp:Label runat="server" Text="Movement" Font-Bold="true"></asp:Label>
                                    </div>
                                    <div class="WorkerInfoLabel">
                                        <asp:Label runat="server" ID="FirstWorkerMovement"  AssociatedControlID="CurrentLocationTB"></asp:Label>
                                    </div>
                                </div>
                                <div>
                                    <div class="WorkerLabel">
                                        <asp:Label runat="server" Text="Project" Font-Bold="true"></asp:Label>
                                    </div>
                                    <div class="WorkerInfoLabel">
                                        <asp:Label runat="server" ID="FirstWorkerProject" AssociatedControlID="CurrentLocationTB"></asp:Label>
                                    </div>
                                </div>
                                <div>
                                    <div class="WorkerLabel">
                                        <asp:Label runat="server" Text="Date / Time Out" Font-Bold="true"></asp:Label>
                                    </div>
                                    <div class="WorkerInfoLabel">
                                        <asp:Label runat="server" ID="FirstWorkerTimeOut" AssociatedControlID="CurrentLocationTB"></asp:Label>
                                    </div>
                                </div>
                                <div>
                                    <div class="WorkerLabel">
                                        <asp:Label runat="server" Text="Location Out" Font-Bold="true"></asp:Label>
                                    </div>
                                    <div class="WorkerInfoLabel">
                                        <asp:Label runat="server" ID="FirstWorkerTimeOutLocation" AssociatedControlID="CurrentLocationTB"></asp:Label>
                                    </div>
                                </div>
                                <div>
                                    <div class="WorkerLabel">
                                        <asp:Label runat="server" Text="Date / Time In" Font-Bold="true"></asp:Label>
                                    </div>
                                    <div class="WorkerInfoLabel">
                                        <asp:Label runat="server" ID="FirstWorkerTimeIn" AssociatedControlID="CurrentLocationTB"></asp:Label>
                                    </div>
                                </div>
                                <div>
                                    <div class="WorkerLabel">
                                        <asp:Label runat="server" Text="Location In" Font-Bold="true"></asp:Label>
                                    </div>
                                    <div class="WorkerInfoLabel">
                                        <asp:Label runat="server" ID="FirstWorkerTimeInLocation" AssociatedControlID="CurrentLocationTB"></asp:Label>
                                    </div>
                                </div>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                             <div class="container TitleDiv">
                                <div>
                                    <div class="WorkerLabel">
                                        <asp:Label runat="server" Text="Worker" Font-Bold="true"></asp:Label>
                                    </div>
                                    <div class="WorkerInfoLabel">
                                        <asp:Label runat="server" ID="SecondWorkerName" AssociatedControlID="CurrentLocationTB"></asp:Label>
                                    </div>
                                </div>
                                <div>
                                    <div class="WorkerLabel">
                                        <asp:Label runat="server" Text="Movement" Font-Bold="true"></asp:Label>
                                    </div>
                                    <div class="WorkerInfoLabel">
                                        <asp:Label runat="server" ID="SecondWorkerMovement" AssociatedControlID="CurrentLocationTB"></asp:Label>
                                    </div>
                                </div>
                                <div>
                                    <div class="WorkerLabel">
                                        <asp:Label runat="server" Text="Project" Font-Bold="true"></asp:Label>
                                    </div>
                                    <div class="WorkerInfoLabel">
                                        <asp:Label runat="server" ID="SecondWorkerProject" AssociatedControlID="CurrentLocationTB"></asp:Label>
                                    </div>
                                </div>
                                <div>
                                    <div class="WorkerLabel">
                                        <asp:Label runat="server" Text="Date / Time Out" Font-Bold="true"></asp:Label>
                                    </div>
                                    <div class="WorkerInfoLabel">
                                        <asp:Label runat="server" ID="SecondWorkerTimeOut" AssociatedControlID="CurrentLocationTB"></asp:Label>
                                    </div>
                                    <div class="WorkerLabel">
                                        <asp:Label runat="server" Text="Location Out" Font-Bold="true"></asp:Label>
                                    </div>
                                    <div class="WorkerInfoLabel">
                                        <asp:Label runat="server" ID="SecondWorkerTimeOutLocation" AssociatedControlID="CurrentLocationTB"></asp:Label>
                                    </div>
                                </div>
                                <div>
                                    <div class="WorkerLabel">
                                        <asp:Label runat="server" Text="Date / Time In" Font-Bold="true"></asp:Label>
                                    </div>
                                    <div class="WorkerInfoLabel">
                                        <asp:Label runat="server" ID="SecondWorkerTimeIn" AssociatedControlID="CurrentLocationTB"></asp:Label>
                                    </div>
                                    <div class="WorkerLabel">
                                        <asp:Label runat="server" Text="Location In" Font-Bold="true"></asp:Label>
                                    </div>
                                    <div class="WorkerInfoLabel">
                                        <asp:Label runat="server" ID="SecondWorkerTimeInLocation" AssociatedControlID="CurrentLocationTB"></asp:Label>
                                    </div>
                                </div>
                            </div>
                        </td>
                    </tr>
                </table>
            </div>
            <div>
                <asp:Label ID="ErrorMessage" runat="server"></asp:Label>
            </div>
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
                    <asp:Button ID="btnViewImage" runat="server" Text="View Image" OnClientClick="return validateForm();this.disabled=true;"  OnClick="btnSubmit_Click" Enabled="false" ForeColor="White" CssClass="btn" Style="padding: 10px 20px; margin-right: 10px;" />
                    <asp:Button ID="btnViewFile" runat="server" Text="View File" OnClientClick="return validateForm();this.disabled=true;" OnClick="btnSubmit_Click" Enabled="false" ForeColor="White" CssClass="btn" Style="padding: 10px 20px; margin-right: 10px;" />
                </div>
            </div>
        </div>
    </div>
    <link rel="stylesheet" href="https://code.jquery.com/ui/1.14.1/themes/base/jquery-ui.css">
    <script src="https://code.jquery.com/ui/1.14.1/jquery-ui.js"></script>
    <script>
        $(document).ready(function () {
            // Initialize Select2 on the dropdowns
            $('#<%= ddlType.ClientID %>').select2();
            $('#<%= ddlEquipment.ClientID %>').select2();
        });

        function formatDesign(item) {
            var selectionText = item.text.split("|");
            var $returnString = selectionText[0] + "</br>" + selectionText[1];
            return $returnString;
        };

        function validateForm() {

        }
</script>
</asp:Content>
