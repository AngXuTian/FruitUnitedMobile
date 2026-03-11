<%@ Page Title="" Language="C#" MasterPageFile="~/ModulesPage.Master" AutoEventWireup="true" CodeBehind="View-Request.aspx.cs" Inherits="FruitUnitedMobile.Modules.View_Request" %>

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
                <asp:Repeater ID="rptTransportRequests" runat="server">
                    <ItemTemplate>
                        <div style="border: 2px solid black; width: 95%; background-color: white; margin-bottom: 20px; padding: 10px; border-radius: 10px;">
                            <div style="display: flex; margin-bottom: 8px;">
                                <div style="display: flex;" class="col-3">
                                    <label>Date:</label>
                                </div>
                                <div style="display: flex;" class="col-9">
                                    <label><%# Eval("Request_Date", "{0:dd-MMM-yyyy}") %></label>
                                </div>
                            </div>
                            <div style="display: flex; margin-bottom: 8px;">
                                <div style="display: flex;" class="col-3">
                                    <label>Time:</label>
                                </div>
                                <div style="display: flex;" class="col-9">
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
                                <div style="display: flex;" class="col-9">
                                    <label><%# Eval("No_of_Seat") %> Material</label>
                                </div>
                            </div>
                            <div style="display: flex; flex-wrap: wrap; margin-bottom: 8px;">
                                <div style="display: flex;" class="col-3">
                                    <div></div>
                                </div>
                                <div style="display: flex;" class="col-9">
                                    <label><%# Eval("Material_Seat") %> Manpower/Tool</label>
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
                                    <label><%# Eval("Vehicle_No") %></label>
                                </div>
                            </div>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
                <br />
                <!--<div class="container" style="border: 2px solid black; width: 95%; overflow-x: hidden; background-color: white;">
                    <table style="width: 100%; border-collapse: collapse;">
                        
                        <tr>
                            <td class="label-column" style="font-weight: bold;">Date:</td>
                            <td class="label-column">
                                <asp:Label runat="server" ID="txtBackDate"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td class="label-column" style="font-weight: bold;">Time:</td>
                            <td class="label-column">
                                <asp:Label runat="server" ID="txtBackTime"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td class="label-column" style="font-weight: bold;">Project:</td>
                            <td class="label-column">
                                <asp:Label runat="server" ID="txtBackProject"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td class="label-column" style="font-weight: bold;">No of Seat(s):</td>
                            <td class="label-column">
                                <asp:Label runat="server" ID="txtBackSeat"></asp:Label>
                            </td>
                            <td class="label-column" style="font-weight: bold;">Material:</td>
                            <td class="label-column">
                                <asp:Label runat="server" ID="txtBackMat"></asp:Label>
                            </td>
                            <td class="label-column" style="font-weight: bold;">Manpower/Tool</td>
                            <td class="label-column">
                                <asp:Label runat="server" ID="txtBackMan"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td class="label-column" style="font-weight: bold;">Total Seat(s):</td>
                            <td class="label-column">
                                <asp:Label runat="server" ID="txtBackTotal"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td class="label-column" style="font-weight: bold;">Remarks:</td>
                            <td class="label-column">
                                <asp:Label runat="server" ID="txtBackRemark"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td class="label-column" style="font-weight: bold;">Lorry:</td>
                            <td class="label-column">
                                <asp:Label runat="server" ID="txtBackLorry"></asp:Label>
                            </td>
                        </tr>
                    </table>-->

            </div>
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
                <asp:Button ID="btnSubmit" runat="server" Text="Back" OnClientClick="window.location.href='Driver-Approval.aspx'; return false;" CssClass="btn btn-secondary" Style="padding: 10px 20px; margin-right: 10px; background-color: green" />
            </div>
        </div>
    </div>
    <script>

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
    </script>
</asp:Content>
