<%@ Page Title="" Language="C#" MasterPageFile="~/ModulesPage.Master" AutoEventWireup="true" CodeBehind="WFH.aspx.cs" Inherits="FruitUnitedMobile.Modules.WFH" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        table {
            width: 100%;
        }

        td {
            padding: 10px;
            vertical-align: middle;
        }

        label {
            font-weight: bold;
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
                <table>
                    
                    <!-- Date-->
                    <tr>
                        <td style="width:20%">
                            <label for="txtDateFrom">Date*</label>
                        </td>
                        <td style="width:35%">
                            <asp:TextBox ID="txtDateFrom" runat="server" CssClass="form-control input-control" placeholder="Date"></asp:TextBox>
                            <span id="DateFromError" style="color: red; display: none;">Date field cannot be empty.</span>
                        </td>
                        
                    </tr>
                    <!-- Remark -->
                    <tr>
                        <td>
                            <label for="txtRemark">Remark</label></td>
                        <td colspan="3">
                            <asp:TextBox ID="txtRemark" runat="server" CssClass="form-control input-control" TextMode="MultiLine" placeholder="Remarks"></asp:TextBox>
                        </td>
                    </tr>
                </table>
            </div>
        </div>
        <!-- Image Upload Section -->
        <br>
        <br>
        <br>
        <br>
        <!-- Bottom Buttons Section -->
        <div style="position: fixed; bottom: 0; left: 0; width: 100vw; background-color: white; border-top: 1px solid #ccc; padding: 10px;">
            <div style="display: flex; justify-content: space-between; align-items: center;">
                <%--<div>
                    <asp:Button ID="btnToProject" runat="server" Text="Project" CausesValidation="false"  OnClick="btnToProject_Click" CssClass="btn btn-secondary" style="padding: 10px 20px;" />
                    <asp:Button ID="btnToScope" runat="server" Text="Scope" CausesValidation="false"  OnClick="btnToScope_Click" CssClass="btn btn-secondary" style="padding: 10px 20px;" />
                    <asp:Button ID="btnToTask" runat="server" Text="Task" CausesValidation="false"  OnClick="btnToTask_Click" CssClass="btn btn-secondary" style="padding: 10px 20px;" />

                </div>--%>
                <div>
                </div>
                <div>
                    <asp:Button ID="btnSubmit" runat="server" Text="Submit" OnClientClick="return validateForm();this.disabled=true;" OnClick="btnSubmit_Click" CssClass="btn btn-success" Style="padding: 10px 20px; margin-right: 10px;" />
                </div>
            </div>
        </div>
    </div>
    <script
        src="https://maps.googleapis.com/maps/api/js?key=AIzaSyCc9Hs72T26oGRbjLnJc5JBsTGVj57rdF8&libraries=places">
</script>
    <link rel="stylesheet" href="https://code.jquery.com/ui/1.14.1/themes/base/jquery-ui.css">
<script src="https://code.jquery.com/ui/1.14.1/jquery-ui.js"></script>
    <script>
        $(document).ready(function () {
            // Initialize Select2 on the dropdowns
            
            $("#" + '<%=txtDateFrom.ClientID%>').datepicker(
                {
                    dateFormat: 'dd-M-yy',
                    minDate: 0
                });
            
        });

       

        function validateAndDisable(btn) {
            if (validateForm()) {
                btn.disabled = true;
                return true;
            }
            return false;
        }

        function validateForm() {
            var isValid = true;

            // Validate date
            var dateValue = $("#" + '<%=txtDateFrom.ClientID%>').val().trim();
            if (dateValue === "") {
                $("#DateFromError").show();
                isValid = false;
            } else {
                $("#DateFromError").hide();
            }

            return isValid;
        }
            </script>
</asp:Content>
