<%@ Page Title="" Language="C#" MasterPageFile="~/ModulesPage.Master" AutoEventWireup="true" CodeBehind="Equipment-Maintenance.aspx.cs" Inherits="FruitUnitedMobile.Modules.Equipment_Maintenance" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
        <link href="../CSS/Basic.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
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
                <div class="col-md-4">
                    <asp:TextBox ID="EquipmentNameTextBox" runat="server" CssClass="form-control" Placeholder="Search by Equipment Name"></asp:TextBox>
                </div> 
            </div>
            <div class="row">
                <div class="col-md-12" style="text-align: right;">
                    <asp:Button ID="SearchButton" runat="server" Text="Search" CssClass="btn btn-primary" OnClick="SearchButton_Click" />
                </div>
            </div>
        </div>
    </div>
    <div id="GridDiv" class="container">
        <div class="row" style="margin-top: 10px;">
            <div class="col-12" id="OutputGridDiv" style="overflow: auto;">
                <asp:Panel ID="GridPanel" runat="server" ScrollBars="Vertical" Style="max-height: 65vh; overflow: auto; width: 100%;">
                    <asp:GridView ID="FruitUnitedMobileGrid" runat="server" AutoGenerateColumns="false" OnRowDataBound="InvoiceGrid_RowDataBound" CssClass="table table-bordered table-responsive">
                        <Columns>
                            <asp:TemplateField HeaderText="Equipment Name">
                                <ItemTemplate>
                                    <asp:Label ID="Label1" Text='<%# Bind("Equipment_Name") %>' Font-Size="Small" runat="server"></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                             <asp:TemplateField HeaderText="Expiry Date">
                                <ItemTemplate>
                                    <asp:Label ID="Label1" Text='<%# Bind("Expiry_Date") %>' Font-Size="Small" runat="server"></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                             <asp:TemplateField HeaderText="Remaining Day(s)">
                                <ItemTemplate>
                                    <asp:Label ID="Label1" Text='<%# Bind("Remaining_Days") %>' Font-Size="Small" runat="server"></asp:Label>
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
