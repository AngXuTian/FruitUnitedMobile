<%@ Page Title="" Language="C#" MasterPageFile="~/ModulesPage.Master" AutoEventWireup="true" CodeBehind="Driver-Approval.aspx.cs" Inherits="FruitUnitedMobile.Modules.Driver_Approval" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <link href="../CSS/Basic.css" rel="stylesheet" />
    <!-- Search Icon -->
    <div class="container">
        <div class="row">
            <div class="col-md-12 text-right" style="text-align: right;">
                <asp:LinkButton ID="SearchToggleButton" runat="server" CssClass="btn btn-link" OnClientClick="toggleSearch(); return false;">
                    <i id="search-icon" style="color: black;" class="fa fa-search-plus" aria-hidden="true"></i>
                </asp:LinkButton>
            </div>
        </div>
    </div>
    <!-- Search Fields -->
    <div class="container">
        <div id="search-container" class="row" style="display: none;">
            <div class="row">
                <div class="col-md-3" style="display: inline-block">
                    <asp:TextBox ID="DateSearch" runat="server" CssClass="form-control FilterTextBox" Placeholder="Date Search"></asp:TextBox>
                </div>
                <div class="col-md-1" style="text-align: right; display: inline-block">
                    <asp:Button ID="SearchButton" runat="server" Text="Search" CssClass="btn btn-primary" OnClick="SearchButton_Click" />
                </div>
            </div>
            <div class="row">
            </div>
        </div>
    </div>
    <div id="GridDiv" class="container">
        <div class="row" style="margin-top: 10px;">
            <div class="col-12" id="OutputGridDiv" style="overflow: auto;">
                <asp:Panel ID="GridPanel" runat="server" ScrollBars="Vertical" Style="max-height: 65vh; overflow: auto; width: 100%;">
                    <asp:GridView ID="DriverGrid" runat="server" AutoGenerateColumns="false" OnRowCommand="DriverGrid_RowCommand" OnRowDataBound="DriverGrid_RowDataBound" CssClass="table table-bordered table-responsive">
                        <Columns>
                            <asp:TemplateField HeaderText="Project">
                                <ItemTemplate>
                                    <asp:Label ID="LabelProject" Text='<%# Bind("Project") %>' Font-Size="Small" runat="server"></asp:Label>
                                </ItemTemplate>
                                <ItemStyle Width="30%" />
                                <HeaderStyle Width="30%" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Date">
                                <ItemTemplate>
                                    <asp:Label ID="LabelDate" Text='<%# Bind("Request_Date") %>' Font-Size="Small" runat="server"></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="No Of Seats">
                                <ItemTemplate>
                                    <asp:Label ID="LabelSeats" Text='<%# Bind("No_of_Seat") %>' Font-Size="Small" runat="server"></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Time">
                                <ItemTemplate>
                                    <asp:Label ID="LabelTime" Text='<%# Bind("Request_Time") %>' Font-Size="Small" runat="server"></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="View" HeaderStyle-HorizontalAlign="Center">
                                <ItemTemplate>
                                    <asp:LinkButton
                                        ID="lnkView"
                                        runat="server"
                                        CommandName="ViewRequest"
                                        CommandArgument='<%# Eval("Transport_Request_ID") %>'><i class="fa fa-file-o" aria-hidden="true"></i></asp:LinkButton>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Driver">
                                <ItemTemplate>
                                    <asp:DropDownList ID="ddlDriver" runat="server" CssClass="form-control select2-dropdown" Width="100%" AutoPostBack="True" OnSelectedIndexChanged="DDLDriver_SelectedIndexChanged"></asp:DropDownList>
                                </ItemTemplate>
                                <ItemStyle Width="30%" />
                                <HeaderStyle Width="30%" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Request ID" Visible="false">
                                <ItemTemplate>
                                    <asp:Label ID="Transport_Request_ID" Text='<%# Bind("Transport_Request_ID") %>' Font-Size="Small" runat="server"></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Request ID" Visible="false">
                                <ItemTemplate>
                                    <asp:Label ID="Go_Back" Text='<%# Bind("Go_Back") %>' Font-Size="Small" runat="server"></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </asp:Panel>
                <asp:HiddenField ID="hfSelectedRequestID" runat="server" />
                <asp:HiddenField ID="hfSelectedDriverID" runat="server" />
                <asp:HiddenField ID="hfSelections" runat="server" />
                <div class="d-flex justify-content-between align-items-center mt-3">
                    <div>
                        <span style="display: inline-block; width: 20px; height: 20px; background-color: lawngreen; border: 1px solid #ccc; margin-right: 5px;"></span>
                        <span>Go</span>
                        <span style="display: inline-block; width: 20px; height: 20px; background-color: darkorange; border: 1px solid #ccc; margin-left: 20px; margin-right: 5px;"></span>
                        <span>Back</span>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <div id="GridDiv2" class="container">
        <div class="row" style="margin-top: 10px;">
            <div class="col-12" id="OutputGridDiv2" style="overflow: auto;">
                <asp:Panel ID="Panel1" runat="server" ScrollBars="Vertical" Style="max-height: 65vh; overflow: auto; width: 100%;">
                    <asp:GridView ID="DriverGrid2" runat="server" AutoGenerateColumns="false" OnRowDataBound="DriverGrid2_RowDataBound" CssClass="table table-bordered table-responsive">
                        <Columns>
                            <asp:TemplateField HeaderText="Driver">
                                <ItemTemplate>
                                    <asp:Label ID="DriverLabel" Text='<%# Bind("Schedule_Display_Name") %>' Font-Size="Small" runat="server"></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Date">
                                <ItemTemplate>
                                    <asp:Label ID="DateLabel" Text='<%# Bind("Request_Date") %>' Font-Size="Small" runat="server"></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Go">
                                <ItemTemplate>
                                    <asp:Label ID="GoLabel" Text='<%# Bind("Go_Seat") %>' Font-Size="Small" runat="server"></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Back">
                                <ItemTemplate>
                                    <asp:Label ID="BackLabel" Text='<%# Bind("Back_Seat") %>' Font-Size="Small" runat="server"></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </asp:Panel>
                <div></div>
            </div>
        </div>
    </div>

    <link rel="stylesheet" href="https://code.jquery.com/ui/1.14.1/themes/base/jquery-ui.css">
    <script src="https://code.jquery.com/jquery-3.7.1.js"></script>
    <script src="https://code.jquery.com/ui/1.14.1/jquery-ui.js"></script>
    <link href="https://cdn.jsdelivr.net/npm/select2@4.1.0-rc.0/dist/css/select2.min.css" rel="stylesheet" />
    <script src="https://cdn.jsdelivr.net/npm/select2@4.1.0-rc.0/dist/js/select2.min.js"></script>
    <script>

        $(document).ready(function () {
            $('.select2-dropdown').select2();
        });

        let driverSelections = {};

        document.addEventListener("DOMContentLoaded", function () {
            document.querySelectorAll('#<%= DriverGrid.ClientID %> select').forEach(function (ddl) {
                ddl.addEventListener('change', function () {
                    let driverId = this.value;
                    let requestId = this.getAttribute('data-requestid');

                    driverSelections[requestId] = driverId;

                    // Save to hidden field
                    document.getElementById('<%= hfSelections.ClientID %>').value = JSON.stringify(driverSelections);
                    console.log(JSON.stringify(driverSelections));
                });
            });
        });

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
            $("#" + '<%=DateSearch.ClientID%>').datepicker(
                {
                    dateFormat: 'dd-M-yy',
                    minDate: 0 
                });
        });

        var dateSearchElem = document.getElementById('<%= DateSearch.ClientID %>');

    </script>
</asp:Content>
