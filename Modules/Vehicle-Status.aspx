<%@ Page Title="" Language="C#" MasterPageFile="~/ModulesPage.Master" AutoEventWireup="true" CodeBehind="Vehicle-Status.aspx.cs" Inherits="FruitUnitedMobile.Modules.Vehicle_Status" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <!-- Search Icon -->
    <link href="../CSS/Basic.css" rel="stylesheet" />
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
    <div id="GridDiv2" class="container">
        <div class="row" style="margin-top: 10px;">
            <div class="col-12" id="OutputGridDiv2" style="overflow: auto;">
                <asp:Panel ID="Panel1" runat="server" ScrollBars="Vertical" Style="max-height: 65vh; overflow: auto; width: 100%;">
                    <asp:GridView ID="DriverGrid2" runat="server" AutoGenerateColumns="false" OnRowDataBound="DriverGrid2_RowDataBound" CssClass="table table-bordered table-responsive">
                        <columns>
                            <asp:TemplateField HeaderText="Driver">
                                <itemtemplate>
                                    <asp:Label ID="DriverLabel" Text='<%# Bind("Schedule_Display_Name") %>' Font-Size="Small" runat="server"></asp:Label>
                                </itemtemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Go">
                                <itemtemplate>
                                    <asp:Label ID="GoLabel" Text='<%# Bind("Go_Seat") %>' Font-Size="Small" runat="server"></asp:Label>
                                </itemtemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Back">
                                <itemtemplate>
                                    <asp:Label ID="BackLabel" Text='<%# Bind("Back_Seat") %>' Font-Size="Small" runat="server"></asp:Label>
                                </itemtemplate>
                            </asp:TemplateField>
                        </columns>
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
