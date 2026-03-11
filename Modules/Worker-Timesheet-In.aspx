<%@ Page Title="" Language="C#" MasterPageFile="~/ModulesPage.Master" AutoEventWireup="true" CodeBehind="Worker-Timesheet-In.aspx.cs" Inherits="FruitUnitedMobile.Modules.Worker_Timesheet_In" %>

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

        #map {
            height: 400px;
            width: 100%;
        }
    </style>
    <link href="../CSS/Basic.css" rel="stylesheet" />
    <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
    <link href="https://cdn.jsdelivr.net/npm/select2@4.1.0-rc.0/dist/css/select2.min.css" rel="stylesheet" />
    <script src="https://cdn.jsdelivr.net/npm/select2@4.1.0-rc.0/dist/js/select2.min.js"></script>
    <script async src="https://maps.googleapis.com/maps/api/js?key=AIzaSyBqFdHdfUPT7hxBKU_ekm0gGXCE4NK_SMA&callback=console.debug&libraries=maps,marker&v=beta"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div style="font-family: Arial, sans-serif;">
        <div class="container mt-4 parent">
            <div class="form-container">
                <table>
                    <!-- Shift -->
                    <tr>
                        <td>
                            <label for="shiftType">Shift Type</label></td>
                        <td>
                            <asp:DropDownList ID="ddlShiftType" runat="server" CssClass="form-control dropdown-mobile">
                                <asp:ListItem Text="-- Please Select --" Value="0" />
                                <asp:ListItem Text="Normal Shift" Value="Normal Shift" />
                                <asp:ListItem Text="Flexible Shift" Value="Flexible Shift" />
                                <asp:ListItem Text="Special Normal" Value="Special Normal" />
                            </asp:DropDownList>
                            <span id="ddlShiftError" style="color: red; display: none;">Please select Shift Type.</span>

                        </td>
                    </tr>
                    <!-- Subcon -->
                    <tr>
                        <td>
                            <label for="ddlSubcon">Type</label></td>
                        <td>
                            <asp:DropDownList ID="ddlType" runat="server" CssClass="form-control dropdown-mobile">
                                <asp:ListItem Text="-- Please Select --" Value="0" />
                                <asp:ListItem Text="Project" Value="Project" />
                                <asp:ListItem Text="Attend SIC" Value="Attend SIC" />
                                <asp:ListItem Text="Training" Value="Training" />
                                <asp:ListItem Text="Office" Value="Office" />
                            </asp:DropDownList>
                            <span id="ddlTypeError" style="color: red; display: none;">Please select Type.</span>

                            <%--         <asp:RadioButton ID="rbOffice" GroupName="TypeGroup" Text="Office" runat="server" Checked="true" OnClick="toggleDropdown()" />
                            <asp:RadioButton ID="rbProject" GroupName="TypeGroup" Text="Project" runat="server" OnClick="toggleDropdown()" />--%>
                        </td>
                    </tr>
                    <!-- Project -->
                    <tr>
                        <td>
                            <label for="ddlProject">Project</label></td>
                        <td>
                            <asp:DropDownList ID="ddlProject" runat="server" AutoPostBack="true" CssClass="form-control dropdown-mobile" OnSelectedIndexChanged="ProjectDropDown_Change">
                                <asp:ListItem Text="-- Please Select --" Value="0" />
                            </asp:DropDownList>
                            <span id="ddlProjectError" style="color: red; display: none;">Please select Project.</span>
                        </td>
                    </tr>
                    <!-- Clock Type -->
                    <tr>
                        <td>
                            <label for="ddlClockType">Clock Type</label></td>
                        <td>
                            <asp:DropDownList ID="ddlClockType" runat="server" CssClass="form-control dropdown-mobile">
                                <asp:ListItem Text="-- Please Select --" Value="0" />
                                <asp:ListItem Text="Normal-In" Value="Normal-In" />
                                <asp:ListItem Text="Project-In" Value="Project-In" />
                                <asp:ListItem Text="OT-In" Value="OT-In" />
                            </asp:DropDownList>
                            <span id="ddlError" style="color: red; display: none;">Please select Clock Type.</span>
                        </td>
                    </tr>
                    <!-- Date In and Time In -->
                    <tr class="split-row">
                        <td>
                            <label for="txtDateIn">Time</label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtTimeIn" runat="server" CssClass="form-control input-control" TextMode="Time" placeholder="Time"></asp:TextBox>
                            <span id="timeInError" style="color: red; display: none;">Time In field cannot be empty.</span>
                        </td>
                    </tr>
                    <!-- Remark -->
                    <tr>
                        <td>
                            <label for="txtRemark">Remark</label></td>
                        <td>
                            <asp:TextBox ID="txtRemark" runat="server" CssClass="form-control input-control" TextMode="MultiLine" placeholder="Remarks..."></asp:TextBox>
                        </td>
                    </tr>
                    <!-- Location -->
                    <tr>
                        <td>
                            <label for="txtLocation">Location</label></td>
                        <td>
                            <div id="map"></div>
                            <div style="display: none;">
                                <asp:TextBox ID="Latitude" runat="server" CssClass="form-control input-control" Enabled="true"></asp:TextBox>
                                <asp:TextBox ID="Longitude" runat="server" CssClass="form-control input-control" Enabled="true"></asp:TextBox>
                                <asp:TextBox ID="Distance" runat="server" CssClass="form-control input-control" Enabled="true"></asp:TextBox>
                                <asp:TextBox ID="PostCode" runat="server" CssClass="form-control input-control" Enabled="false"></asp:TextBox>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <label for="fileUpload1">Upload Image *</label>
                        </td>
                        <td>
                            <asp:FileUpload ID="fileUpload1" runat="server" CssClass="form-control" Style="width: 100%; margin-bottom: 10px;" accept="image/*" />
                            <span id="fileUploadError" style="color: red; display: none;">Please upload an image file.</span>
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
                    <asp:Button ID="btnSubmit" runat="server" Text="Check In" OnClientClick="return validateForm();this.disabled=true;" OnClick="btnSubmit_Click" CssClass="btn btn-success" Style="padding: 10px 20px; margin-right: 10px;" />
                </div>
            </div>
        </div>
    </div>
    <script>
        function initMap() {
            // Default map center
            const defaultPos = { lat: -34.397, lng: 150.644 };
            const map = new google.maps.Map(document.getElementById("map"), {
                zoom: 15,
                center: defaultPos,
            });
            // Try HTML5 geolocation
            if (navigator.geolocation) {
                navigator.geolocation.getCurrentPosition(
                    (position) => {
                        const lat = position.coords.latitude;
                        const lng = position.coords.longitude;
                        document.getElementById('<%=Latitude.ClientID %>').value = lat;
                        document.getElementById('<%=Longitude.ClientID %>').value = lng;
                        const pos = { lat, lng };
                        // Add marker at current location
                        new google.maps.Marker({
                            position: pos,
                            map: map,
                            title: "You are here!",
                        });

                        var geocoder = new google.maps.Geocoder();
                        var address = document.getElementById('<%=PostCode.ClientID %>').value + " Singapore";

                        geocoder.geocode({ 'address': address }, function (results, status) {

                            if (status == google.maps.GeocoderStatus.OK) {
                                const latitude = results[0].geometry.location.lat();
                                const longitude = results[0].geometry.location.lng();
                                if (document.getElementById('<%=PostCode.ClientID %>').value) {
                                    document.getElementById('<%=Distance.ClientID %>').value = getDistanceFromLatLonInKm(lat, lng, latitude, longitude);
                                }
                                else {
                                    document.getElementById('<%=Distance.ClientID %>').value = 0;
                                }
                            }
                        });

                        // Center map on current location
                        map.setCenter(pos);
                    },
                    () => {
                        handleLocationError(true, map.getCenter());
                    }
                );
            } else {
                // Browser doesn't support Geolocation
                handleLocationError(false, map.getCenter());
            }
        }

        function getDistanceFromLatLonInKm(lat1, lon1, lat2, lon2) {
            var R = 6371; // Radius of the earth in km
            var dLat = deg2rad(lat2 - lat1);  // deg2rad below
            var dLon = deg2rad(lon2 - lon1);
            var a =
                Math.sin(dLat / 2) * Math.sin(dLat / 2) +
                Math.cos(deg2rad(lat1)) * Math.cos(deg2rad(lat2)) *
                Math.sin(dLon / 2) * Math.sin(dLon / 2)
                ;
            var c = 2 * Math.atan2(Math.sqrt(a), Math.sqrt(1 - a));
            var d = R * c; // Distance in km
            return d;
        }

        function deg2rad(deg) {
            return deg * (Math.PI / 180)
        }

        function handleLocationError(browserHasGeolocation, pos) {
            alert(
                browserHasGeolocation
                    ? "Error: The Geolocation service failed."
                    : "Error: Your browser doesn't support geolocation."
            );
        }

        // Initialize the map after the page loads
        window.onload = initMap;
    </script>
    <script>
        $(document).ready(function () {
            // Initialize Select2 on the dropdowns
            $('#<%= ddlProject.ClientID %>').select2();
            $('#<%= ddlShiftType.ClientID %>').select2();
            $('#<%= ddlClockType.ClientID %>').select2();
            $('#<%= ddlType.ClientID %>').select2();


            $("#<%= ddlType.ClientID %>").change(function () {
                if ($(this).val() === "Office") {
                    $("#<%= ddlProject.ClientID %>").prop("disabled", true);
                } else {
                    $("#<%= ddlProject.ClientID %>").prop("disabled", false);
                }
            });

        });

        function validateAndDisable(btn) {
            if (validateForm()) {
                btn.disabled = true;
                return true;
            }
            return false;
        }

        function toggleWorkerDropdown() {
            var ddlType = document.getElementById("<%= ddlType.ClientID %>");
            var ddlProject = document.getElementById("<%= ddlProject.ClientID %>");

            // Enable or disable ddlProject based on the selected value of ddlType
            if (ddlType.value === "Office") {
                ddlProject.disabled = true; // Disable ddlProject
            } else {
                ddlProject.disabled = false; // Enable ddlProject
            }
        }

        function validateForm() {
            const ddlValid = validateDropDownList();
            const ddlTypeValid = validateTypeList();
            const ddlShiftValid = validateShiftDropDownList();
            const timeInValid = validateTimeIn();
            const fileValid = validateFileUpload();

            const ddl = document.getElementById('<%= ddlType.ClientID %>');

            if (ddl.value === "Project" || ddl.value === "Attend SIC") {
                const ddlProjectValid = validateProjectDropDownList();
                return ddlShiftValid && ddlTypeValid && ddlProjectValid && ddlValid && timeInValid && fileValid;
            } else {
                const errorSpan = document.getElementById('ddlProjectError');
                errorSpan.style.display = 'none';
                return ddlShiftValid && ddlTypeValid && ddlValid && timeInValid && fileValid;
            }
            return fales;
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

        function validateTypeList() {
            const ddl = document.getElementById('<%= ddlType.ClientID %>');
            const errorSpan = document.getElementById('ddlTypeError');

            if (ddl.value === "0") {
                errorSpan.style.display = 'inline';
                return false;
            } else {
                errorSpan.style.display = 'none';
                return true;
            }
        }

        function validateShiftDropDownList() {
            const ddl = document.getElementById('<%= ddlShiftType.ClientID %>');
            const errorSpan = document.getElementById('ddlShiftError');

            if (ddl.value === "0") {
                errorSpan.style.display = 'inline';
                return false;
            } else {
                errorSpan.style.display = 'none';
                return true;
            }
        }

        function validateProjectDropDownList() {
            const ddl = document.getElementById('<%= ddlProject.ClientID %>');
            const errorSpan = document.getElementById('ddlProjectError');

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
