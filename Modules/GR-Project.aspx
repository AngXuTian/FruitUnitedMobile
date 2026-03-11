<%@ Page Title="" Language="C#" MasterPageFile="~/ModulesPage.Master" AutoEventWireup="true" CodeBehind="GR-Project.aspx.cs" Inherits="FruitUnitedMobile.Modules.GR_Project" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <link href="../CSS/Basic.css" rel="stylesheet" />
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
                    <asp:TextBox ID="SONoTextBox" runat="server" CssClass="form-control FilterTextBox" Placeholder="Search by SO No"></asp:TextBox>
                </div>
                <div class="col-md-4">
                    <asp:TextBox ID="ProjectNameTextBox" runat="server" CssClass="form-control FilterTextBox" Placeholder="Search by Project Name"></asp:TextBox>
                </div>
                <div class="col-md-4">
                    <asp:TextBox ID="CustomerNameTextBox" runat="server" CssClass="form-control FilterTextBox" Placeholder="Search by Customer Name"></asp:TextBox>
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
                <asp:Panel ID="GridPanel" runat="server" ScrollBars="Vertical" Style="max-height: 80vh; overflow: auto; width: 100%;">
                    <asp:GridView ID="ProjectGrid" runat="server" AutoGenerateColumns="false" OnRowDataBound="ProjectGrid_RowDataBound" OnRowCommand="ProjectGrid_RowCommand" CssClass="table table-bordered table-responsive">
                        <Columns>
                            <asp:TemplateField HeaderText="Project">
                                <ItemTemplate>
                                    <asp:Label ID="Label1" Text='<%# Bind("Project_No") %>' Font-Size="Small" Font-Bold="true" runat="server"></asp:Label>
                                    <br>
                                    <asp:Label ID="Label2" Text='<%# Bind("Project_Name") %>' Font-Size="Small" runat="server"></asp:Label>
                                    <br>
                                    <asp:Label ID="Label3" Text='<%# Bind("Customer_Name") %>' Font-Size="Small" runat="server"></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="">
                                <ItemTemplate>
                                    <div id="linkBtnDiv">
                                        <asp:LinkButton ID="ViewLocation" runat="server" CommandName="ViewLocation" CommandArgument='<%# Eval("Project_ID") %>'><i class="fa fa-external-link" aria-hidden="true"></i></asp:LinkButton>
                                    </div>
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="20%"></ItemStyle>
                            </asp:TemplateField>
                            <asp:TemplateField>
                                <ItemTemplate>
                                    <asp:Label ID="Project_ID" runat="server" Text='<%# Bind("Project_ID") %>' Visible="false"></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </asp:Panel>
                <div></div>
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
