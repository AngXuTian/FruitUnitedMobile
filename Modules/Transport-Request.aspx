<%@ Page Title="" Language="C#" MasterPageFile="~/ModulesPage.Master" AutoEventWireup="true" CodeBehind="Transport-Request.aspx.cs" Inherits="FruitUnitedMobile.Modules.Transport_Request" %>

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

        .goBackOption label {
            margin-left: 10px; /* Adjust as needed for the desired gap */
        }
    </style>
    <link href="../CSS/Basic.css" rel="stylesheet" />
    <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
    <link href="https://cdn.jsdelivr.net/npm/select2@4.1.0-rc.0/dist/css/select2.min.css" rel="stylesheet" />
    <script src="https://cdn.jsdelivr.net/npm/select2@4.1.0-rc.0/dist/js/select2.min.js"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div style="font-family: Arial, sans-serif;">
        <div class="container mt-4 parent">
            <div class="form-container">
                <table>
                    <!-- Date & Time & Go/Back-->
                    <tr>
                        <td colspan="3">
                            <label for="txtDate">Date*</label>
                        </td>
                        <td colspan="9">
                            <asp:TextBox ID="txtDate" runat="server" CssClass="form-control input-control" placeholder="Date"></asp:TextBox>
                            <span id="ddlDateError" style="color: red; display: none;">Please select Date.</span>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="3">
                            <label for="txtTime">Time*</label>
                        </td>
                        <td colspan="9">
                            <asp:TextBox ID="txtTime" runat="server" CssClass="form-control input-control" TextMode="Time" placeholder="Time"></asp:TextBox>
                            <span id="ddlTimeError" style="color: red; display: none;">Please select Time.</span>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="3">
                            <label>Go/Back*</label>
                        </td>
                        <td colspan="9">
                            <asp:RadioButtonList ID="rblGoBack" runat="server" RepeatDirection="Horizontal" CssClass="goBackOption">
                                <asp:ListItem Text="  Go" Value="Go" />
                                <asp:ListItem Text="  Back" Value="Back" />
                            </asp:RadioButtonList>
                            <span id="ddlGoBackError" style="color: red; display: none;">Please select Type.</span>
                        </td>

                    </tr>
                    <!-- Project -->
                    <tr>
                        <td colspan="3">
                            <label for="ddlProject">Project*</label>
                        </td>
                        <td colspan="9">
                            <asp:DropDownList ID="ddlProject" runat="server" CssClass="dropdown-mobile form-control" AutoPostBack="false" ClientIDMode="Static">
                            </asp:DropDownList>
                            <span id="ddlProjectError" style="color: red; display: none;">Please select Project.</span>
                        </td>
                    </tr>
                    <!-- No Of Seats, Manpower, Material/Tools -->
                    <tr>
                        <td colspan="3">
                            <label for="ManpowerSeatTB">No of Seat*</label></td>
                        <td colspan="6">
                            <asp:TextBox ID="ManpowerSeatTB" runat="server" CssClass="form-control input-control" TextMode="Number" Text ="0" onchange="setZeroIfEmpty(this);"></asp:TextBox>
                            <span id="ddlSeatError" style="color: red; display: none;">Please Key in Manpower Seat No.</span>
                        </td>
                        <td colspan="3">
                            <label for="MaterialSeatTB">Manpower</label>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="3">
                        <td colspan="6">
                            <asp:TextBox ID="MaterialSeatTB" runat="server" CssClass="form-control input-control" TextMode="Number" Text ="0" onchange="setZeroIfEmpty(this);"></asp:TextBox>
                            <span id="ddlManError" style="color: red; display: none;">Please Key in Material Seat No.</span>

                        </td>
                        <td colspan="3">
                            <label for="txtMaterial">Material/Tool</label>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="3">
                            <label for="txtTotal">Total Seats</label>
                        </td>
                        <td colspan="9">
                            <asp:TextBox ID="txtTotal" runat="server" CssClass="form-control input-control" Text ="0" ReadOnly="true" BackColor="LightGray"></asp:TextBox>
                            <asp:HiddenField ID="hdnTotal" runat="server" />
                        </td>
                    </tr>

                     <tr>
                        <td colspan="3">
                            <label for="ddlTransport">Transport Unavailable*</label>
                        </td>
                        <td colspan="9">
                            <asp:DropDownList ID="ddlTransport" runat="server" CssClass="dropdown-mobile form-control" AutoPostBack="false" ClientIDMode="Static">
                            </asp:DropDownList>
                            <span id="ddlTransportError" style="color: red; display: none;">Please select Transport.</span>
                        </td>
                    </tr>

                    <!-- Remark -->
                    <tr>
                        <td colspan="3">
                            <label for="txtRemark">Remark</label></td>
                        <td colspan="9">
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
            $('#<%= ddlProject.ClientID %>').select2();
            $('#<%= ddlTransport.ClientID %>').select2();
        });
        // Run on page load

        $("[id$='txtGo']").val("Go");
        $("[id$='txtBack']").val("Back");


        // --- Init Date Picker ---
        $("#<%= txtDate.ClientID %>").datepicker({
            dateFormat: 'dd-M-yy',
            minDate: 0 // Disable back date
        }).datepicker("setDate", 'now');;



        // --- Restrict Seats/Manpower to positive integers ---
        function restrictToPositiveInteger(selector) {
            $(selector).on("input", function () {
                this.value = this.value.replace(/[^0-9]/g, '');
            });
        }

        // --- Enable Submit if User Edits Any Input ---
        $("input, select").on("input change", function () {
            $("#<%= btnSubmit.ClientID %>").prop("disabled", false);
        });


        restrictToPositiveInteger("#<%= ManpowerSeatTB.ClientID %>");
        restrictToPositiveInteger("#<%= MaterialSeatTB.ClientID %>");

        // --- Calculate total ---
        function calculateTotal() {
            let ManpowerSeat = parseInt($("#<%= ManpowerSeatTB.ClientID %>").val()) || 0;
            let MaterialSeat = parseInt($("#<%= MaterialSeatTB.ClientID %>").val()) || 0;
            let total = ManpowerSeat + MaterialSeat;
            $("#<%= txtTotal.ClientID %>").val(total);
            $("#<%= hdnTotal.ClientID %>").val(total);
            return total;
        }

        $("#<%= ManpowerSeatTB.ClientID %>, #<%= MaterialSeatTB.ClientID %>").on("input", function () {
            calculateTotal();
        });


        // --- Validation Logic ---
        function validateForm() {

            let valid = true;
            calculateTotal();

            // Date
            const dateVal = $("#<%= txtDate.ClientID %>").val();
            if (!dateVal) {
                $("#ddlDateError").show();
                valid = false;
            } else {
                $("#ddlDateError").hide();
            }

            // Time
            const timeVal = $("#<%= txtTime.ClientID %>").val();
            if (!timeVal) {
                $("#ddlTimeError").show();
                valid = false;
            } else {
                $("#ddlTimeError").hide();
            }
            var selected = $("input[name*='rblGoBack']:checked").val(); // works with ASP.NET ID rendering

            if (!selected) {
                $("#ddlGoBackError").show();
                valid = false;
            } else {
                $("#ddlGoBackError").hide();
            }


            // Validate Seat
            const matVal = $("#<%= MaterialSeatTB.ClientID %>").val();
            if (!matVal && !manVal) {
                $("#ddlSeatError").show();
                $("#ddlManError").show();
                valid = false;
            } else {
                $("#ddlSeatError").hide();
                $("#ddlManError").hide();
            }

            // Validate Manpower
            const manVal = $("#<%= ManpowerSeatTB.ClientID %>").val();
            if (!manVal) {
                $("#ddlManError").show();
                valid = false;
            } else {
                $("#ddlManError").hide();
            }

            // Project
            const projectVal = $("#<%= ddlProject.ClientID %>").val();
            if (projectVal === "0") {
                $("#ddlProjectError").show();
                valid = false;
            } else {
                $("#ddlProjectError").hide();
            }

            const transportVal = $("#<%= ddlTransport.ClientID %>").val();
            if (transportVal === "0") {
                $("#ddlTransportError").show();
                valid = false;
            } else {
                $("#ddlTransportError").hide();
            }

            if (!valid) {
                $("#<%= btnSubmit.ClientID %>").prop("disabled", true);
                return false;
            }

            return true;

        }
        // --- Wrapper to Validate and Disable Submit Button ---
        function validateAndDisable(btn) {
            const isValid = validateForm();
            if (isValid) {
                btn.disabled = true; // disable the button to prevent multiple clicks
            }

            return isValid;
        }

        function setZeroIfEmpty(textbox) {
            if (textbox.value.trim() === "") {
                textbox.value = "0";
            }
        }

    </script>
</asp:Content>
