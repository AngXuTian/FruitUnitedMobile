<%@ Page Title="" Language="C#" MasterPageFile="~/ModulesPage.Master" AutoEventWireup="true" CodeBehind="Leave-Application.aspx.cs" Inherits="FruitUnitedMobile.Modules.Leave_Application" %>

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
        <div class="container mt-4 parent">
            <div class="form-container">
                <table>
                    <!-- Leave Type -->
                    <tr>
                        <td>
                            <label for="ddlType">Leave*</label>
                        </td>
                        <td colspan="3">
                            <asp:DropDownList ID="ddlType" runat="server" CssClass="form-control dropdown-mobile">
                                <asp:ListItem Text="-- Please Select --" Value="0" />
                            </asp:DropDownList>
                            <span id="ddlLeaveError" style="color: red; display: none;">Please select Leave.</span>
                        </td>
                    </tr>
                    <!-- Clock Type -->
                    <tr>
                        <td>
                            <label for="ddlSession">Session*</label>
                        </td>
                        <td colspan="3">
                            <asp:DropDownList ID="ddlSession" runat="server" CssClass="form-control dropdown-mobile">
                                <asp:ListItem Text="-- Please Select --" Value="0" />
                                <asp:ListItem Text="Full Day" Value="Full Day" />
                                <asp:ListItem Text="AM" Value="AM" />
                                <asp:ListItem Text="PM" Value="PM" />
                            </asp:DropDownList>
                            <span id="ddlSessionError" style="color: red; display: none;">Please select Session.</span>
                        </td>
                    </tr>
                    <!-- Date-->
                    <tr>
                        <td class="col-2">
                            <label for="txtDateFrom">Date*</label>
                        </td>
                        <td  class="col-4">
                            <asp:TextBox ID="txtDateFrom" runat="server" CssClass="form-control input-control" placeholder="Date"></asp:TextBox>
                            <span id="DateFromError" style="color: red; display: none;">Date field cannot be empty.</span>
                        </td>
                        <td  class="col-2">
                            <label for="txtDateTo">To</label>
                        </td>
                        <td  class="col-4">
                            <asp:TextBox ID="txtDateTo" runat="server" CssClass="form-control input-control" placeholder="Date"></asp:TextBox>
                            <span id="DateToError" style="color: red; display: none;">Date field cannot be empty.</span>
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
                    <tr>
                        <td>
                            <label for="fileUpload">Upload File (Image Only)</label>
                        </td>
                        <td colspan="3">
                            <asp:FileUpload ID="fileUpload" runat="server" CssClass="form-control" Style="width: 100%; margin-bottom: 10px;" accept="image/*" />
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
            $('#<%= ddlType.ClientID %>').select2();
            $('#<%= ddlSession.ClientID %>').select2();
            $("#" + '<%=txtDateFrom.ClientID%>').datepicker(
                {
                    dateFormat: 'dd-M-yy',
                });
            $("#" + '<%=txtDateTo.ClientID%>').datepicker(
                {
                    dateFormat: 'dd-M-yy',
                });
            $("#<%= ddlSession.ClientID %>").change(function () {
                if ($(this).val() === "Full Day") {
                    $("#<%= txtDateTo.ClientID %>").prop("disabled", false);
                } else {
                    $("#<%= txtDateTo.ClientID %>").val('');
                    $("#<%= txtDateTo.ClientID %>").prop("disabled", true);
                }
            });

            // Trigger the change event initially to set the correct state
            $("#<%= ddlSession.ClientID %>").trigger("change");
        });

        function validateAndDisable(btn) {
            if (validateForm()) {
                btn.disabled = true;
                return true;
            }
            return false;
        }

        function validateForm() {
            const ddlTypeValid = validateTypeList();
            const ddlSessionValid = validateSessionDropDownList();
            const dateValid = validateDate();
            const dateToValid = validateToDate();
            const fileValid = validateFileUpload();

            return fales;
        }

        function validateTypeList() {
            const ddl = document.getElementById('<%= ddlType.ClientID %>');
            const errorSpan = document.getElementById('ddlLeaveError');

            if (ddl.value === "0") {
                errorSpan.style.display = 'inline';
                return false;
            } else {
                errorSpan.style.display = 'none';
                return true;
            }
        }

        function validateSessionDropDownList() {
            const ddl = document.getElementById('<%= ddlSession.ClientID %>');
            const errorSpan = document.getElementById('ddlSessionError');

            if (ddl.value === "0") {
                errorSpan.style.display = 'inline';
                return false;
            } else {
                errorSpan.style.display = 'none';
                return true;
            }
        }

        function validateDate() {
            const dateInput = document.getElementById('<%= txtDateFrom.ClientID %>');
            const errorSpan = document.getElementById('DateFromError');
            if (!dateInput.value) {
                errorSpan.style.display = 'inline';
                return false;
            } else {
                errorSpan.style.display = 'none';
                return true;
            }
        }

        function validateToDate() {
            const ddlSession = document.getElementById('<%= ddlSession.ClientID %>');
            const dateInput = document.getElementById('<%= txtDateTo.ClientID %>');
            const errorSpan = document.getElementById('DateToError');

            if (ddlSession.value === "Full Day" && !dateInput.value) {
                errorSpan.style.display = 'inline';
                return false;
            } else {
                errorSpan.style.display = 'none';
                return true;
            }
        }

</script>
</asp:Content>
