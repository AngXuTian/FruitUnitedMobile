<%@ Page Title="" Language="C#" MasterPageFile="~/ModulesPage.Master" AutoEventWireup="true" CodeBehind="Worker-Timesheet-Out.aspx.cs" Inherits="FruitUnitedMobile.Modules.Worker_Timesheet_Out" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>

        table {
            width: 100%;
        }

        td {
            padding: 10px;
            vertical-align: top;
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
        .form-control1{
            width:105%;
        }
        .form-control2{
            width:200px;
        }
    </style>
    <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
    <link href="https://cdn.jsdelivr.net/npm/select2@4.1.0-rc.0/dist/css/select2.min.css" rel="stylesheet" />
    <script src="https://cdn.jsdelivr.net/npm/select2@4.1.0-rc.0/dist/js/select2.min.js"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div style="font-family: Arial, sans-serif;">

      <div class="container mt-4">
        <div class="form-container">
            <div class="container" style="border:2px solid black; width:95%; overflow-x: hidden; background-color: white;">
                 <table style="width: 100%; border-collapse: collapse;">
                      <tr>
                          <td colspan="2"><span class="badge bg-primary">Project Info</span></td>
                      </tr>
                        <tr>
                            <td class="label-column" style="font-weight: bold;">Shift Type:</td>
                            <td class="label-column"><asp:Label runat="server" ID="txtShiftType"></asp:Label></td>
                        </tr>
                        <tr>
                            <td class="label-column" style="font-weight: bold;">Type:</td>
                            <td class="label-column"><asp:Label runat="server" ID="txtType"></asp:Label></td>
                        </tr>
                        <tr>
                            <td class="label-column" style="font-weight: bold;">Project:</td>
                            <td class="label-column"><asp:Label runat="server" ID="txtProject"></asp:Label></td>
                        </tr>
                    </table>
        
                </div>
                <br />
                <table style="width: 100%;">
                    <!-- Clock Type -->
                    <tr>
                        <td><label for="ddlWorker">Clock Type</label></td>
                        <td>
                            <asp:DropDownList ID="ddlClockType" runat="server" style="width:100%;" >
                                <asp:ListItem Text="-- Please Select --" Value="0"  />
                                <asp:ListItem Text="Normal-Out" Value="Normal-Out"  />
                                <asp:ListItem Text="Project-Out" Value="Project-Out" />
                                <asp:ListItem Text="OT-Out" Value="OT-Out" />
                            </asp:DropDownList>
                            <span id="ddlError" style="color: red; display: none;">Please select a valid clock type.</span>
                        </td>
                    </tr>

                   <!-- Date In and Time In -->
                    <tr class="split-row">
                        <td>
                            <label for="txtDateIn">Time</label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtTimeIn" runat="server" CssClass="form-control input-control" TextMode="Time" placeholder="Time"></asp:TextBox>
                             <span id="timeInError" style="color: red; display: none;">Time Out field cannot be empty.</span>
                        </td>
                    </tr>

                    <!-- Break -->
                    <tr class="split-row">
                        <td>
                            <label for="txtDateIn">Break (min)</label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtBreak" runat="server" CssClass="form-control input-control" TextMode="Number" placeholder=""></asp:TextBox>
                           <span id="breakHourError" style="color: red; display: none;">Please enter a valid number with at most 1 decimal place.</span>

                        </td>
                    </tr>

                    <!-- Remark -->
                    <tr>
                        <td><label for="txtRemark">Remark</label></td>
                        <td>
                            <asp:TextBox ID="txtRemark" runat="server" CssClass="form-control input-control" TextMode="MultiLine" placeholder="Remarks..."></asp:TextBox>
                        </td>
                    </tr>

                    <!-- Location -->
                    <tr>
                        <td><label for="txtLocation">Location</label></td>
                        <td>
<asp:TextBox ID="txtLocation" runat="server" CssClass="form-control input-control" 
            placeholder="Select location from map"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <label for="txtLocation">Upload Image *</label>
                
                        </td>
                        <td>
                                <asp:FileUpload ID="fileUpload1" runat="server" CssClass="form-control" style="width: 100%; margin-bottom: 10px;" accept="image/*" />
                                        <span id="fileUploadError" style="color: red; display: none;">Please upload an image file.</span>

                        </td>
                    </tr>
                </table>
        </div>
    </div>
        <!-- Image Upload Section -->
       
         <br><br><br><br>

        <!-- Bottom Buttons Section -->
        <div style="position: fixed; bottom: 0; left: 0; width: 100%; background-color: white; border-top: 1px solid #ccc; padding: 10px;">
            <div style="display: flex; justify-content: space-between; align-items: center;">
                <%--<div>
                    <asp:Button ID="btnToProject" runat="server" Text="Project" CausesValidation="false"  OnClick="btnToProject_Click" CssClass="btn btn-secondary" style="padding: 10px 20px;" />
                    <asp:Button ID="btnToScope" runat="server" Text="Scope" CausesValidation="false"  OnClick="btnToScope_Click" CssClass="btn btn-secondary" style="padding: 10px 20px;" />
                    <asp:Button ID="btnToTask" runat="server" Text="Task" CausesValidation="false"  OnClick="btnToTask_Click" CssClass="btn btn-secondary" style="padding: 10px 20px;" />

                </div>--%>
                <div>

                </div>
                <div>
                    <asp:Button ID="btnSubmit" runat="server" Text="Check Out" OnClientClick="return validateForm();" OnClick="btnSubmit_Click" CssClass="btn btn-danger" style="padding: 10px 20px; margin-right:10px;" />
                </div>
            </div>
        </div>
    </div>
    <script>
        $(document).ready(function () {
            // Initialize Select2 on the dropdowns
        $('#<%= ddlClockType.ClientID %>').select2();
        });

        function validateForm() {
            const ddlValid = validateDropDownList();
            const timeInValid = validateTimeIn();
            const breakHourValid = validateBreakHour(document.getElementById('<%= txtBreak.ClientID %>'));
            const fileValid = validateFileUpload();

            // Prevent form submission if any validation fails
            return ddlValid && timeInValid && breakHourValid && fileValid;
        }

        function validateDropDownList() {
            const ddl = document.getElementById('<%= ddlClockType.ClientID %>');
            const errorSpan = document.getElementById('ddlError');

            if (ddl.value === "0") {
                errorSpan.style.display = 'inline';
                return false;
            } else {
                errorSpan.style.display = 'none';
                return true;
            }
        }

        function validateTimeIn() {
            const timeInInput = document.getElementById('<%= txtTimeIn.ClientID %>');
            const errorSpan = document.getElementById('timeInError');

            if (!timeInInput.value) {
                errorSpan.style.display = 'inline';
                return false;
            } else {
                errorSpan.style.display = 'none';
                return true;
            }
        }

        function validateBreakHour(input) {
            const ddl = document.getElementById('<%= ddlClockType.ClientID %>');
            const value = input.value;
            const errorSpan = document.getElementById('breakHourError');
            const regex = /^\d+(\.\d{1})?$/; // Allows numeric input with at most 1 decimal place

            // Only validate if ddlClockType value is 'OT-Out'
            if (ddl.value === 'OT out') {
                if (!regex.test(value)) {
                    errorSpan.style.display = 'inline';
                    errorSpan.textContent = "Please enter a valid number with at most 1 decimal place.";
                    return false;
                } else {
                    errorSpan.style.display = 'none';
                    return true;
                }
            } else {
                // Clear any previous errors when validation is not required
                errorSpan.style.display = 'none';
                return true;
            }
        }

        function validateFileUpload() {
            const fileInput = document.getElementById('<%= fileUpload1.ClientID %>');
            const errorSpan = document.getElementById('fileUploadError');

            if (!fileInput.value) {
                errorSpan.style.display = 'inline';
                return false;
            } else {
                errorSpan.style.display = 'none';
                return true;
            }
        }

</script>
</asp:Content>