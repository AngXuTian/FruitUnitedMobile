<%@ Page Title="" Language="C#" MasterPageFile="~/ModulesPage.Master" AutoEventWireup="true" CodeBehind="Submitted-Request.aspx.cs" Inherits="FruitUnitedMobile.Modules.Submitted_Request" %>

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
            font-size: clamp(14px, 2vw, 20px);
            padding-left: 5px;
        }

        .TimeSpan {
            font-size: clamp(14px, 2vw, 20px);
            padding-left: 5px;
            padding-right: 5px;
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
    <script src="https://code.jquery.com/ui/1.13.2/jquery-ui.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/select2@4.1.0-rc.0/dist/js/select2.min.js"></script>
    <link rel="stylesheet" href="https://code.jquery.com/ui/1.13.2/themes/base/jquery-ui.css">
    <script async src="https://maps.googleapis.com/maps/api/js?key=AIzaSyBqFdHdfUPT7hxBKU_ekm0gGXCE4NK_SMA&callback=console.debug&libraries=maps,marker&v=beta"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div style="font-family: Arial, sans-serif;">
        <div class="container mt-4">
            <div class="form-container">
                <!-- Search Icon -->
                <div class="container">
                    <div class="row">
                        <div class="col-md-12 text-right" style="text-align: right;">
                            <asp:LinkButton ID="SearchToggleButton" runat="server" CssClass="btn btn-link" OnClientClick="toggleSearch(); return false;">
             <i id="search-icon" style="color:black;" class="fa fa-search-plus" aria-hidden="true"></i>
         </asp:LinkButton>
                        </div>
                    </div>
                </div>
                <!-- Search Fields -->
                <div class="container">
                    <div id="search-container" class="row" style="display: none;">
                        <div class="row mb-3">
                            <div class="col-md-3">
                                <asp:TextBox ID="DateFromTextBox" runat="server" CssClass="form-control FilterTextBox" Placeholder="Search by From Date"></asp:TextBox>
                            </div>
                            <div class="col-md-3">
                                <asp:TextBox ID="DateToTextBox" runat="server" CssClass="form-control FilterTextBox" Placeholder="Search by To Date"></asp:TextBox>
                            </div>
                            <div class="col-md-3">
                                <asp:TextBox ID="ProjectTextBox" Width="100%" runat="server" CssClass="form-control FilterTextBox" Placeholder="Search by Project"></asp:TextBox>
                            </div>
                            <div class="col-md-3">
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-12" style="text-align: right;">
                                <asp:Button ID="SearchButton" runat="server" Text="Search" CssClass="btn btn-primary" OnClick="SearchButton_Click" />
                            </div>
                        </div>
                    </div>
                </div>
                <asp:Label runat="server" ID="ErrorMessage" />
                <div id="GridDiv" class="container" style="padding-top: 10px;">
                    <asp:Repeater ID="rptTransportRequests" runat="server" OnItemDataBound="rptTransportRequests_ItemDataBound">
                        <ItemTemplate>
                            <div style="border: 2px solid black; width: 95%; background-color: white; margin-bottom: 20px; padding: 10px; border-radius: 10px;">
                                <div style="display: flex; margin-bottom: 8px;">
                                    <div style="display: flex;" class="col-3">
                                        <label>Date:</label>
                                    </div>
                                    <div style="display: flex;" class="col-9">
                                        <asp:Label runat="server" ID="DateLabel" CssClass="TimeSpan" Text='<%# Eval("Request_Date", "{0:dd-MMM-yyyy}") %>'></asp:Label>
                                    </div>
                                </div>
                                <div style="display: flex; margin-bottom: 8px;">
                                    <div style="display: flex;" class="col-3">
                                        <label>Time:</label>
                                    </div>
                                    <div style="display: flex;" class="col-9">
                                        <asp:Label runat="server" ID="Status" Text='<%# Eval("Status") %>'></asp:Label>
                                        <asp:Label runat="server" ID="GoBack" Text='<%# Eval("Go_Back") %>'></asp:Label>
                                        <asp:Label runat="server" ID="tdTime" CssClass="TimeSpan" Text='<%# Eval("Request_Time", "{0:hh\\:mm}") %>'></asp:Label>
                                    </div>
                                </div>
                                <div style="display: flex; margin-bottom: 8px;">
                                    <div style="display: flex;" class="col-3">
                                        <label>Requestor:</label>
                                    </div>
                                    <div style="display: flex;" class="col-9">
                                        <label><%# Eval("Requestor") %></label>
                                    </div>
                                </div>
                                <div style="display: flex; margin-bottom: 8px;">
                                    <div style="display: flex;" class="col-3">
                                        <label>Project:</label>
                                    </div>
                                    <div style="display: flex;" class="col-9">
                                        <label><%# Eval("Project") %></label>
                                    </div>
                                </div>
                                <div style="display: flex; flex-wrap: wrap; margin-bottom: 8px;">
                                    <div style="display: flex;" class="col-3">
                                        <label>No of Seat(s):</label>
                                    </div>
                                    <div class="col-9">
                                        <div>
                                            <label><%# Eval("No_of_Seat") %> Manpower</label>
                                        </div>
                                        <div>
                                            <label><%# Eval("Material_Seat") %> Material/Tool</label>
                                        </div>
                                    </div>
                                </div>
                                <div style="display: flex; margin-bottom: 8px;">
                                    <div style="display: flex;" class="col-3">
                                        <label>Total Seat(s):</label>
                                    </div>
                                    <div style="display: flex;" class="col-9">
                                        <label><%# Convert.ToInt32(Eval("No_of_Seat")) + Convert.ToInt32(Eval("Material_Seat")) %></label>
                                    </div>
                                </div>
                                <div style="display: flex; margin-bottom: 8px;">
                                    <div style="display: flex;" class="col-3">
                                        <label>Transport Unavailable:</label>
                                    </div>
                                    <div style="display: flex;" class="col-9">
                                        <label><%# Eval("Transport_Mode") %></label>
                                    </div>
                                </div>
                                <div style="display: flex; margin-bottom: 8px;">
                                    <div style="display: flex;" class="col-3">
                                        <label>Remarks:</label>
                                    </div>
                                    <div style="display: flex;" class="col-9">
                                        <label><%# Eval("Remarks") %></label>
                                    </div>
                                </div>
                                <div style="display: flex; margin-bottom: 8px;">
                                    <div style="display: flex;" class="col-3">
                                        <label>Lorry:</label>
                                    </div>
                                    <div style="display: flex;" class="col-9">
                                        <label><%# Eval("Driver")%></label>
                                    </div>
                                </div>
                                <div style="margin-top: 15px;">
                                    <asp:Button ID="btnCancel" runat="server" Text="Cancel"
                                        CssClass="btn btn-danger"
                                        OnClick="btnCancel_Click"
                                        CommandArgument='<%# Eval("Transport_Request_ID") %>'
                                        data-request-date='<%# Eval("Request_Date", "{0:yyyy-MM-dd}") %>' />
                                </div>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
                <br />
            </div>
        </div>
    </div>
    <!-- Bottom Buttons Section -->


    <script>

        window.onload = function () {
            const today = new Date().setHours(0, 0, 0, 0);

            document.querySelectorAll("[data-request-date]").forEach(btn => {
                const dateStr = btn.getAttribute("data-request-date");
                const requestDate = new Date(dateStr).setHours(0, 0, 0, 0);

                if (requestDate < today) {
                    btn.disabled = true;
                    btn.style.display = "none";
                    btn.classList.remove("btn-danger");
                    btn.classList.add("btn-secondary");
                }
            });
        };


        function toggleSearch() {
            var searchContainer = document.getElementById("search-container");
            var searchIcon = document.getElementById("search-icon");

            // Check if the search container is visible
            if (searchContainer.style.display === "none" || searchContainer.style.display === "") {
                // Show search fields
                searchContainer.style.display = "block";
                searchIcon.className = "fa fa-search-minus"; // Change icon to minus
            } else {
                // Hide search fields
                searchContainer.style.display = "none";
                searchIcon.className = "fa fa-search-plus"; // Change icon to plus
            }
        }


        $(function () {
            var dateTo = $("#" + '<%=DateToTextBox.ClientID%>');
            var dateFrom = $("#" + '<%=DateFromTextBox.ClientID%>');

            // Initialize datepickers
            dateTo.datepicker({
                dateFormat: 'dd-M-yy'
            });

            dateFrom.datepicker({
                dateFormat: 'dd-M-yy'
            });

            // Log values on change
            dateTo.on("change", function () {
                console.log("Date To:", $(this).val());
            });

            dateFrom.on("change", function () {
                console.log("Date From:", $(this).val());
            });

            // Optional: log initial values on page load
            console.log("Initial Date To:", dateTo.val());
            console.log("Initial Date From:", dateFrom.val());
        });

    </script>
</asp:Content>
