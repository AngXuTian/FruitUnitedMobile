<%@ Page Title="" Language="C#" MasterPageFile="~/ModulesPage.Master" AutoEventWireup="true" CodeBehind="Subcon-Timesheet.aspx.cs" Inherits="FruitUnitedMobile.Modules.Subcon_Timesheet" %>

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
        <div class="container mt-4 parent">
            <div class="form-container">
                <table>
                    <tr>
                        <td class="col-md-3">
                            <label class="label" for="ddlProject">Project</label>
                        </td>
                        <td class="col-md-9">
                            <asp:DropDownList ID="ddlProject" CssClass="form-control dropdown-mobile" runat="server">
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td class="col-md-3">
                            <label class="label" for="ddlSubcon">Subcon</label>
                        </td>
                        <td class="col-md-9">
                            <asp:DropDownList ID="ddlSubcon" CssClass="form-control dropdown-mobile" AutoPostBack="true" OnSelectedIndexChanged="ddlSubcon_SelectedIndexChanged" runat="server">
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td class="col-md-3">
                            <label class="label">Worker</label>
                        </td>
                        <td class="col-md-9">
                            <div style="position: relative;">
                                <asp:TextBox ID="txtSelectedItems" runat="server" ReadOnly="true" CssClass="dropdown-input" OnClick="toggleCheckboxList()" />
                                <asp:Panel ID="pnlCheckboxList" runat="server" CssClass="dropdown-panel" Style="display: none;">
                                    <div style="max-height: 300px; overflow-y: auto; ">
                                        <asp:CheckBoxList ID="chkListItems" CssClass="CheckBoxListItem" runat="server" AutoPostBack="false">
                                        </asp:CheckBoxList>
                                    </div>
                                </asp:Panel>
                            </div>
                        </td>
                    </tr>
                    <tr class="split-row">
                        <td class="col-md-3">
                            <label class="label" for="txtDateIn">Date In</label>
                            <asp:TextBox ID="txtDateIn" runat="server" CssClass="form-control input-control" TextMode="Date" placeholder="Default Today's Date"></asp:TextBox>
                        </td>
                        <td class="col-md-9">
                            <label class="label" for="txtTimeIn">Time In</label>
                            <asp:TextBox ID="txtTimeIn" runat="server" CssClass="form-control input-control" TextMode="Time" placeholder="Time"></asp:TextBox>
                        </td>
                    </tr>
                    <tr class="split-row">
                        <td class="col-md-3">
                            <label class="label" for="txtDateOut">Date Out</label>
                            <asp:TextBox ID="txtDateOut" runat="server" CssClass="form-control input-control" TextMode="Date" placeholder="Default to Date In"></asp:TextBox>
                        </td>
                        <td class="col-md-9">
                            <label class="label" for="txtTimeOut">Time Out</label>
                            <asp:TextBox ID="txtTimeOut" runat="server" CssClass="form-control input-control" TextMode="Time" placeholder="Time"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="col-md-3">
                            <label class="label" for="txtBreakHour">Break Min</label>
                        </td>
                        <td class="col-md-9">
                            <asp:TextBox ID="txtBreakHour" onblur="validateBreakHour(this)" runat="server" CssClass="form-control input-control" placeholder="Numeric 1 decimal"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="col-md-3">
                            <label class="label" for="txtRemark">Remark</label>
                        </td>
                        <td class="col-md-9">
                            <asp:TextBox ID="txtRemark" runat="server" CssClass="form-control input-control" TextMode="MultiLine" placeholder="Remarks..."></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="col-md-3">
                            <label class="label" for="txtLocation">Location</label>
                        </td>
                        <td class="col-md-9">
                            <asp:TextBox ID="txtLocation" runat="server" CssClass="form-control input-control" placeholder="Select location from map"></asp:TextBox>
                        </td>
                    </tr>
                </table>
                <div style="padding: 10px;">
                    <asp:Label class="label" runat="server" Text="Upload Image (Optional):" AssociatedControlID="fileUploadImage"></asp:Label>
                    <asp:FileUpload ID="fileUploadImage" runat="server" CssClass="form-control" Style="width: 100%; margin-bottom: 10px;" accept="image/*" />
                    <asp:Label class="label" ID="lblUploadHint" runat="server" Text="You can upload only one image. This field is optional." ForeColor="Gray"></asp:Label>
                </div>
            </div>
        </div>
        <br>
        <br>
        <br>
        <br>
        <div style="position: fixed; bottom: 0; left: 0; width: 100vw; background-color: white; border-top: 1px solid #ccc; padding: 10px;">
            <div style="display: flex; justify-content: space-between; align-items: center;">
                <div>
                </div>
                <div>
                    <asp:Button ID="btnSubmit" runat="server" Text="Submit" OnClientClick="return validateDateAndTime();" OnClick="btnSubmit_Click" CssClass="btn btn-success" Style="padding: 10px 20px; margin-right: 10px;" />
                </div>
            </div>
        </div>
    </div>
    <script src="https://maps.googleapis.com/maps/api/js?key=AIzaSyCc9Hs72T26oGRbjLnJc5JBsTGVj57rdF8&libraries=places"></script>
    <script>
        $(document).ready(function () {
            // Initialize Select2 on the dropdowns
            $('#<%= ddlProject.ClientID %>').select2();
            $('#<%= ddlSubcon.ClientID %>').select2();

            var panel = document.getElementById('<%= pnlCheckboxList.ClientID %>');
            var input = document.getElementById('<%= txtSelectedItems.ClientID %>');


            // Get selected checkboxes and update input
            var checkboxes = panel.querySelectorAll('input[type="checkbox"]');
            var selected = [];
            checkboxes.forEach(function (cb) {
                if (cb.checked) {
                    var label = cb.parentElement.textContent.trim();
                    selected.push(label);
                }
            });

            input.value = selected.join(', ');
        });

        function toggleCheckboxList() {
            var panel = document.getElementById('<%= pnlCheckboxList.ClientID %>');
            panel.style.display = (panel.style.display === 'none') ? 'block' : 'none';
        }

        document.addEventListener('click', function (e) {
            var panel = document.getElementById('<%= pnlCheckboxList.ClientID %>');
            var input = document.getElementById('<%= txtSelectedItems.ClientID %>');

            if (!panel.contains(e.target) && e.target !== input) {
                panel.style.display = 'none';

                // Get selected checkboxes and update input
                var checkboxes = panel.querySelectorAll('input[type="checkbox"]');
                var selected = [];
                checkboxes.forEach(function (cb) {
                    if (cb.checked) {
                        var label = cb.parentElement.textContent.trim();
                        selected.push(label);
                    }
                });

                input.value = selected.join(', ');
            }
        });

        function validateDateAndTime() {
            var dateIn = document.getElementById('<%= txtDateIn.ClientID %>').value;
            var timeIn = document.getElementById('<%= txtTimeIn.ClientID %>').value;
            var dateOut = document.getElementById('<%= txtDateOut.ClientID %>').value;
            var timeOut = document.getElementById('<%= txtTimeOut.ClientID %>').value;

            var today = new Date();
            var yesterday = new Date();
            yesterday.setDate(today.getDate() - 1);

            // Normalize today's and yesterday's dates to remove the time part
            today.setHours(0, 0, 0, 0);
            yesterday.setHours(0, 0, 0, 0);

            var parsedDateIn = new Date(dateIn);
            parsedDateIn.setHours(0, 0, 0, 0);

            var parsedDateOut = new Date(dateOut);
            parsedDateOut.setHours(0, 0, 0, 0);

            // Date In Validation: Only today's date or 1 day earlier allowed
            if (parsedDateIn > today || parsedDateIn < yesterday) {
                alert("Date In can only be today's date or 1 day earlier.");
                return false;
            }


            // Date Out Validation: Only today's date or 1 day earlier, cannot be earlier than Date In
            if (parsedDateOut > today || parsedDateOut < parsedDateIn) {
                alert("Date Out can only be today's date or 1 day earlier, and cannot be earlier than Date In.");
                return false;
            }

            // Time Validation: Time Out cannot be earlier than Time In if Date Out == Date In
            if (dateIn === dateOut) {
                // Parse Time In and Time Out
                var parsedTimeIn = new Date(`1970-01-01T${timeIn}`);
                var parsedTimeOut = new Date(`1970-01-01T${timeOut}`);

                if (parsedTimeOut < parsedTimeIn) {
                    alert("Time Out cannot be earlier than Time In when Date In and Date Out are the same.");
                    return false;
                }
            }

            return true;
        }

        function validateBreakHour(input) {
            var value = input.value;
            var regex = /^\d+(\.\d{1})?$/; // Allows numeric input with at most 1 decimal place

            if (!regex.test(value)) {
                alert("Please enter a valid number with at most 1 decimal place.");
                input.value = ""; // Clear the invalid input
            }
        }

        let autocomplete;

        function initializeAutocomplete() {
            // Get the TextBox's client-side ID
            const locationInput = document.getElementById('<%= txtLocation.ClientID %>');

            if (!locationInput) {
                console.error("Location input field not found.");
                return;
            }

            // Initialize Google Places Autocomplete
            autocomplete = new google.maps.places.Autocomplete(locationInput, {
                types: ['geocode'] // Restrict results to geographic locations
            });

            // Listener for place selection
            autocomplete.addListener('place_changed', function () {
                const place = autocomplete.getPlace();

                if (place && place.formatted_address) {
                    locationInput.value = place.formatted_address; // Set address in field
                } else {
                    console.error("No valid place selected.");
                }
            });
        }

        // Wait for the page to load before initializing autocomplete
        window.onload = initializeAutocomplete;
</script>
</asp:Content>
