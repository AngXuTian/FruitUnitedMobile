<%@ Page Title="" Language="C#" MasterPageFile="~/ModulesPage.Master" AutoEventWireup="true" CodeBehind="Equipment-Rental.aspx.cs" Inherits="FruitUnitedMobile.Modules.Equipment_Rental" %>

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

        .inline {
            display: inline-block; /* Or a specific width like 300px */
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
    <script async src="https://maps.googleapis.com/maps/api/js?key=AIzaSyBOnnUkYXXbbl-H5536NLpV_zPDSWt8oqk&callback=console.debug&libraries=maps,marker&v=beta"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div style="font-family: Arial, sans-serif;">
        <div class="container mt-4 parent">
            <div class="form-container">
                <table style="width: 100%;">
                    <!-- Type DDL-->
                    <tr>
                        <td class="TitleLabel">
                            <label for="ddlType">Type</label></td>
                        <td>
                            <asp:DropDownList ID="ddlType" AutoPostBack="true" runat="server" CssClass="form-control dropdown-mobile" OnSelectedIndexChanged="TypeListDropDown_Change">
                                <asp:ListItem Text="-- Please Select --" Value="0" />
                            </asp:DropDownList>
                            <span id="ddlTypeError" style="color: red; display: none;">Please select Type.</span>
                        </td>
                    </tr>
                    <!-- Equipment DDL-->
                    <tr class="split-row">
                        <td class="TitleLabel">
                            <label for="ddlEquipment">Equipment</label></td>
                        <td>
                            <asp:DropDownList ID="ddlEquipment" AutoPostBack="true" runat="server" CssClass="form-control dropdown-mobile" OnSelectedIndexChanged="EquipmentListDropDown_Change">
                                <asp:ListItem Text="-- Please Select --" Value="0" />
                            </asp:DropDownList>
                            <span id="ddlEquipmentError" style="color: red; display: none;">Please select Equipment.</span>
                        </td>
                    </tr>
                    <!-- Location DDL -->
                    <tr class="split-row">
                        <td class="TitleLabel">
                            <label for="ddlLocation">Location</label>
                        </td>
                        <td>
                            <asp:DropDownList ID="ddlLocation" runat="server" CssClass="form-control dropdown-mobile">
                                <asp:ListItem Text="-- Please Select --" Value="0" />
                            </asp:DropDownList>
                            <span id="ddlLocationError" style="color: red; display: none;">Please select Location.</span>
                        </td>
                    </tr>
                    <!-- Movement DDL -->
                    <tr class="split-row">
                        <td class="TitleLabel">
                            <label for="ddlMovement">Movement</label>
                        </td>
                        <td>
                            <asp:DropDownList ID="ddlMovement" AutoPostBack="true" runat="server" CssClass="form-control dropdown-mobile" OnSelectedIndexChanged="MovementListDropDown_Change">
                                <asp:ListItem Text="-- Please Select --" Value="0" />
                            </asp:DropDownList>
                            <span id="ddlMovementError" style="color: red; display: none;">Please select Movement.</span>
                        </td>
                    </tr>
                    <!-- Project DDL -->
                    <tr class="split-row" id="divProject" runat="server" cssclass="form-control dropdown-mobile" style="display: none;">
                        <td class="TitleLabel">
                            <label for="ddlProject">Project</label>
                        </td>
                        <td>
                            <asp:DropDownList ID="ddlProject" runat="server" CssClass="form-control dropdown-mobile">
                                <asp:ListItem Text="-- Please Select --" Value="0" />
                            </asp:DropDownList>
                            <span id="ddlProjectError" style="color: red; display: none;">Please select Project.</span>
                        </td>
                    </tr>
                    <!-- Checklist Section -->
                    <tr>
                        <td colspan="2">
                            <div id="GridDiv" class="container">
                                <div class="row">
                                    <div class="col-12" id="OutputGridDiv" style="overflow: auto;">
                                        <asp:Panel ID="GridPanel" runat="server" ScrollBars="Vertical" Style="max-height: 40vh; overflow: auto; width: 100%;">
                                            <asp:GridView ID="ChecklistGrid" runat="server" AutoGenerateColumns="false" ShowHeader="false" OnRowDataBound="ChecklistGrid_RowDataBound" CssClass="table table-bordered table-responsive">
                                                <Columns>
                                                    <asp:TemplateField>
                                                        <ItemTemplate>
                                                            <asp:Label ID="SNTxt" Text='<%# Bind("S_N") %>' Font-Size="Small" runat="server"></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField>
                                                        <ItemTemplate>
                                                            <asp:Label ID="ChecklistTxt" Text='<%# Bind("Checklist") %>' Font-Size="Small" runat="server"></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField>
                                                        <ItemTemplate>
                                                            <asp:CheckBox ID="ChecklistCB" runat="server" TextAlign="Right" />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                </Columns>
                                            </asp:GridView>
                                        </asp:Panel>
                                    </div>
                                </div>
                            </div>
                            <span id="ChecklistGridError" style="color: red; display: none;">Please check at least 1 option.</span>
                        </td>
                    </tr>
                    <!-- Remark Section -->
                    <tr>
                        <td class="TitleLabel">
                            <label for="txtRemark">Remark</label></td>
                        <td>
                            <asp:TextBox ID="txtRemark" runat="server" CssClass="form-control input-control" TextMode="MultiLine" placeholder="Remarks..."></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <label for="txtLocation">Current Location</label></td>
                        <td>
                            <div id="map"></div>
                            <div style="display: none;">
                                <asp:TextBox ID="Latitude" runat="server" CssClass="form-control input-control" Enabled="true"></asp:TextBox>
                                <asp:TextBox ID="Longitude" runat="server" CssClass="form-control input-control" Enabled="true"></asp:TextBox>
                            </div>
                        </td>
                    </tr>
                    <!-- Expiry Date Section -->
                    <tr id="divExpiryDate" runat="server" style="display: none;">
                        <td class="TitleLabel">
                            <label for="txtExpiryDate">Expiry Date</label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtExpiryDate" runat="server" CssClass="form-control input-control" placeholder="Please Select Expiry Date" Width="100%"></asp:TextBox>
                        </td>
                    </tr>
                    <!-- Image Upload Section -->
                    <tr id="ImageUpload1DTR">
                        <td class="TitleLabel">
                            <label for="ImageUpload1">Upload Image *</label>
                        </td>
                        <td>
                            <div class="inline" style="width: 80%;">
                                <asp:FileUpload ID="ImageUpload1" runat="server" CssClass="form-control" Style="width: 100%; margin-bottom: 10px;" accept="image/*" capture="camera" />
                            </div>
                            <div class="inline">
                                <button onclick="ClearImageUpload1();return false;" id="BtnClearImageUpload1" class="btn btn-mini" title="Search">
                                    <i class="fa fa-times"></i>
                                </button>
                            </div>
                            <span id="fileUploadError" style="color: red; display: none;">Please upload an image file.</span>
                        </td>
                    </tr>
                    <!-- Image Upload Section -->
                    <tr id="ImageUpload2DTR" style="display: none;">
                        <td class="TitleLabel">
                            <label for="ImageUpload2"></label>
                        </td>
                        <td>
                            <div class="inline" style="width: 80%;">
                                <asp:FileUpload ID="ImageUpload2" runat="server" CssClass="form-control" Style="width: 100%; margin-bottom: 10px;" accept="image/*" capture="camera" />
                            </div>
                            <div class="inline">
                                <button onclick="ClearImageUpload2();return false;" id="BtnClearImageUpload2" class="btn btn-mini" title="Search">
                                    <i class="fa fa-times"></i>
                                </button>
                            </div>
                        </td>
                    </tr>
                    <!-- Image Upload Section -->
                    <tr id="ImageUpload3DTR" style="display: none;">
                        <td class="TitleLabel">
                            <label for="ImageUpload3"></label>
                        </td>
                        <td>
                            <div class="inline" style="width: 80%;">
                                <asp:FileUpload ID="ImageUpload3" runat="server" CssClass="form-control" Style="width: 100%; margin-bottom: 10px;" accept="image/*" capture="camera" />
                            </div>
                            <div class="inline">
                                <button onclick="ClearImageUpload3();return false;" id="BtnClearImageUpload3" class="btn btn-mini" title="Search">
                                    <i class="fa fa-times"></i>
                                </button>
                            </div>
                        </td>
                    </tr>
                    <!-- File Upload Section -->
                    <tr id="divFileUpload1" runat="server" style="display: none;">
                        <td class="TitleLabel">
                            <label for="fileUpload1">Upload File</label>
                        </td>
                        <td>
                            <div class="inline" style="width: 80%;">
                                <asp:FileUpload ID="fileUpload1" runat="server" CssClass="form-control" Style="width: 100%; margin-bottom: 10px;" Multiple="Multiple" />
                            </div>
                            <div class="inline">
                                <button onclick="ClearImageUpload4();return false;" id="BtnClearImageUpload4" class="btn btn-mini" title="Search">
                                    <i class="fa fa-times"></i>
                                </button>
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
                <%--<div>
                    <asp:Button ID="btnToProject" runat="server" Text="Project" CausesValidation="false"  OnClick="btnToProject_Click" CssClass="btn btn-secondary" style="padding: 10px 20px;" />
                    <asp:Button ID="btnToScope" runat="server" Text="Scope" CausesValidation="false"  OnClick="btnToScope_Click" CssClass="btn btn-secondary" style="padding: 10px 20px;" />
                    <asp:Button ID="btnToTask" runat="server" Text="Task" CausesValidation="false"  OnClick="btnToTask_Click" CssClass="btn btn-secondary" style="padding: 10px 20px;" />
                </div>--%>
                <div>
                </div>
                <div>
                    <asp:Button ID="btnSubmit" runat="server" Text="-------" OnClientClick="return validateForm();this.disabled=true;" OnClick="btnSubmit_Click" Enabled="false" ForeColor="White" CssClass="btn" Style="padding: 10px 20px; margin-right: 10px;" />
                </div>
            </div>
        </div>
    </div>
    <link rel="stylesheet" href="https://code.jquery.com/ui/1.14.1/themes/base/jquery-ui.css">
    <script src="https://code.jquery.com/ui/1.14.1/jquery-ui.js"></script>
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

        window.onload = initMap;
    </script>
    <script>
        $(document).ready(function () {
            // Initialize Select2 on the dropdowns
            $('#<%= ddlType.ClientID %>').select2();
            $('#<%= ddlEquipment.ClientID %>').select2();
            $('#<%= ddlLocation.ClientID %>').select2();
            $('#<%= ddlMovement.ClientID %>').select2();
            $('#<%= ddlProject.ClientID %>').select2({
                formatResult: formatDesign,
                formatSelection: formatDesign
            });
            $("#" + '<%=txtExpiryDate.ClientID%>').datepicker(
                {
                    dateFormat: 'dd-M-yy',
                });

            const fileInput1 = document.getElementById('<%= ImageUpload1.ClientID %>');
            const fileDisplayArea2 = document.getElementById('ImageUpload2DTR');
            const fileInput2 = document.getElementById('<%= ImageUpload2.ClientID %>');
            const fileDisplayArea3 = document.getElementById('ImageUpload3DTR');

            fileInput1.addEventListener('change', function (e) {
                const file = e.target.files[0];
                if (file) {
                    fileDisplayArea2.style.display = '';
                } else {
                    fileDisplayArea2.style.display = 'none';
                }
            });

            fileInput2.addEventListener('change', function (e) {
                const file = e.target.files[0];
                if (file) {
                    fileDisplayArea3.style.display = '';
                } else {
                    fileDisplayArea3.style.display = 'none';
                }
            });
        });

        function formatDesign(item) {
            var selectionText = item.text.split("|");
            var $returnString = selectionText[0] + "</br>" + selectionText[1];
            return $returnString;
        };

        function validateFileUpload() {
            const fileInput = document.getElementById('<%= ImageUpload1.ClientID %>');
            const errorSpan = document.getElementById('fileUploadError');

            if (!fileInput.value) {
                errorSpan.style.display = 'inline';
                return false;
            } else {
                errorSpan.style.display = 'none';
                return true;
            }
        }

        function validateForm() {
            const ddlType = validateddlType();
            const ddlEquipment = validateddlEquipment();
            const ddlLocation = validateddlLocation();
            const ddlMovement = validateddlMovement();
            const ddlProject = validateddlProject();
            const CheckList = validatechecklist();
            const fileValid = validateFileUpload();

            return ddlType && ddlEquipment && ddlLocation && ddlMovement && ddlProject && CheckList && fileValid;
        }

        function validatechecklist() {
            const errorSpan = document.getElementById('ChecklistGridError');
            var el = document.getElementsByTagName("input");
            var gvID = document.getElementById('<%=ChecklistGrid.ClientID %>');

            for (var i = 0; i < el.length; i++) {
                if (el[i].type == "checkbox") {
                    if (el[i].checked) {
                        errorSpan.style.display = 'none';
                        return true;
                    }
                }
            }
            errorSpan.style.display = 'inline';
            return false;
        }

        function validateddlType() {
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

        function validateddlEquipment() {
            const ddl = document.getElementById('<%= ddlEquipment.ClientID %>');
            const errorSpan = document.getElementById('ddlEquipmentError');
            if (ddl.value === "0") {
                errorSpan.style.display = 'inline';
                return false;
            } else {
                errorSpan.style.display = 'none';
                return true;
            }
        }

        function validateddlLocation() {
            const ddl = document.getElementById('<%= ddlLocation.ClientID %>');
            const errorSpan = document.getElementById('ddlLocationError');
            if (ddl.value === "0") {
                errorSpan.style.display = 'inline';
                return false;
            } else {
                errorSpan.style.display = 'none';
                return true;
            }
        }

        function validateddlMovement() {
            const ddl = document.getElementById('<%= ddlMovement.ClientID %>');
            const errorSpan = document.getElementById('ddlMovementError');
            if (ddl.value === "0") {
                errorSpan.style.display = 'inline';
                return false;
            } else {
                errorSpan.style.display = 'none';
                return true;
            }
        }

        function validateddlProject() {
            const ddlMovement = document.getElementById('<%= ddlMovement.ClientID %>');
            const ddl = document.getElementById('<%= ddlProject.ClientID %>');
            const errorSpan = document.getElementById('ddlProjectError');
            if (ddlMovement.value !== "Project") {
                return true;
            }
            else if (ddl.value === "0") {
                errorSpan.style.display = 'inline';
                return false;
            } else {
                errorSpan.style.display = 'none';
                return true;
            }
        }

        function ClearImageUpload1() {
            const fileInput = document.getElementById('<%= ImageUpload1.ClientID %>');
            fileInput.value = '';
            const fileInput1 = document.getElementById('<%= ImageUpload2.ClientID %>');
            fileInput1.value = '';
            const fileInput2 = document.getElementById('<%= ImageUpload3.ClientID %>');
            fileInput2.value = '';
            const fileDisplayArea2 = document.getElementById('ImageUpload2DTR');
            const fileDisplayArea3 = document.getElementById('ImageUpload3DTR');
            fileDisplayArea2.style.display = 'none';
            fileDisplayArea3.style.display = 'none';
        }

        function ClearImageUpload2() {
            const fileInput1 = document.getElementById('<%= ImageUpload2.ClientID %>');
            fileInput1.value = '';
            const fileInput2 = document.getElementById('<%= ImageUpload3.ClientID %>');
            fileInput2.value = '';
            const fileDisplayArea3 = document.getElementById('ImageUpload3DTR');
            fileDisplayArea3.style.display = 'none';
        }

        function ClearImageUpload3() {
            const fileInput = document.getElementById('<%= ImageUpload3.ClientID %>');
            fileInput.value = '';
        }

        function ClearImageUpload4() {
            const fileInput = document.getElementById('<%= fileUpload1.ClientID %>');
             fileInput.value = '';
         }
    </script>
</asp:Content>
