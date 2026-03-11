<%@ Page Title="" Language="C#" MasterPageFile="~/ModulesPage.Master" AutoEventWireup="true" CodeBehind="Knowledge-Base.aspx.cs" Inherits="FruitUnitedMobile.Modules.Knowledge_Base" %>

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
                <div class="col-md-3">
                    <asp:TextBox ID="DepartmentTextBox" runat="server" CssClass="form-control FilterTextBox" Placeholder="Search by Department"></asp:TextBox>
                </div>
                <div class="col-md-3">
                    <asp:TextBox ID="CategoryTextBox" runat="server" CssClass="form-control FilterTextBox" Placeholder="Search by Category"></asp:TextBox>
                </div>
                <div class="col-md-3">
                    <asp:TextBox ID="TitleTextBox" runat="server" CssClass="form-control FilterTextBox" Placeholder="Search by Title"></asp:TextBox>
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
    <div id="GridDiv" class="container">
        <div class="row" style="margin-top: 10px;">
            <div class="col-12" id="OutputGridDiv" style="overflow: auto;">
                <asp:Panel ID="GridPanel" runat="server" ScrollBars="Vertical" Style="max-height: 85vh; overflow: auto; width: 100%;">
                    <asp:GridView ID="KnowledgeBaseGrid" runat="server" AutoGenerateColumns="false" OnRowCommand="KnowledgeBaseGrid_RowCommand" OnRowDataBound="KnowledgeBaseGrid_RowDataBound" CssClass="table table-bordered table-responsive">
                        <Columns>
                            <asp:TemplateField HeaderText="Department">
                                <ItemTemplate>
                                    <asp:Label ID="LabelDeparment" Text='<%# Bind("Department") %>' Font-Size="Small" runat="server"></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Category">
                                <ItemTemplate>
                                    <asp:Label ID="LabelCategory" Text='<%# Bind("Category") %>' Font-Size="Small" runat="server"></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Title">
                                <ItemTemplate>
                                    <asp:Label ID="LabelTitle" Text='<%# Bind("Title") %>' Font-Size="Small" runat="server"></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Download">
                                <ItemTemplate>
                                    <div id="linkBtnDiv">
                                        <asp:LinkButton ID="ViewFile" runat="server" CommandName="ViewFile" CommandArgument='<%# Eval("Knowledge_Base_ID") %>'><i class="fa fa-file-o" aria-hidden="true"></i></asp:LinkButton>
                                    </div>
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="10%"></ItemStyle>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Upload" Visible="false">
                                <ItemTemplate>
                                    <div id="linkBtnDiv">
                                        <asp:LinkButton ID="UploadFile" runat="server" CommandName="UploadFile" CommandArgument='<%# Eval("Knowledge_Base_ID") %>'><i class="fa fa-upload" aria-hidden="true"></i></asp:LinkButton>
                                    </div>
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="10%"></ItemStyle>
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
