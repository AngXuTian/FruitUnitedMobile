<%@ Page Title="" Language="C#" MasterPageFile="~/ModulesPage.Master" AutoEventWireup="true" CodeBehind="Worker-Timesheet-History.aspx.cs" Inherits="FruitUnitedMobile.Modules.Worker_Timesheet_History" %>

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
                    <asp:TextBox ID="SONoTextBox" runat="server" CssClass="form-control FilterTextBox" Placeholder="Search by SO No"></asp:TextBox>
                </div>
                <div class="col-md-3">
                    <asp:TextBox ID="ProjectNameTextBox" runat="server" CssClass="form-control FilterTextBox" Placeholder="Search by Project Name"></asp:TextBox>
                </div>
                <div class="col-md-3">
                    <asp:TextBox ID="DateFromTextBox" runat="server" CssClass="form-control FilterTextBox" Placeholder="From Date"></asp:TextBox>
                </div>
                <div class="col-md-3">
                    <asp:TextBox ID="DateToTextBox" runat="server" CssClass="form-control FilterTextBox" Placeholder="To Date"></asp:TextBox>
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
                    <asp:GridView ID="InvoiceGrid" runat="server" AutoGenerateColumns="false" OnRowDataBound="InvoiceGrid_RowDataBound" CssClass="table table-bordered table-responsive">
                        <Columns>
                            <asp:TemplateField HeaderText="Date In">
                                <ItemTemplate>
                                    <asp:Label ID="LabelDateIn" Text='<%# Bind("Date_In") %>' Font-Size="Small" runat="server"></asp:Label>
                                    <asp:HiddenField ID="LabelType" Value='<%# Bind("Type") %>' runat="server"></asp:HiddenField>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Time In">
                                <ItemTemplate>
                                    <asp:Label ID="LabelTimeIn" Text='<%# Bind("Time_In") %>' Font-Size="Small" runat="server"></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Date Out">
                                <ItemTemplate>
                                    <asp:Label ID="LabelDateOut" Text='<%# Bind("Date_Out") %>' Font-Size="Small" runat="server"></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Time Out">
                                <ItemTemplate>
                                    <asp:Label ID="LabelTimeOut" Text='<%# Bind("Time_Out") %>' Font-Size="Small" runat="server"></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Project">
                                <ItemTemplate>
                                    <asp:Label ID="LabelProjectName" Text='<%# Bind("Project_Name") %>' Font-Size="Small" runat="server"></asp:Label>
                                    <br>
                                    <asp:Label ID="LabelPrjectNo" Text='<%# Bind("Project_No") %>' Font-Size="Small" runat="server"></asp:Label>
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


        $(function () {
            $("#" + '<%=DateToTextBox.ClientID%>').datepicker(
                {
                    dateFormat: 'dd-M-yy',
                });
            $("#" + '<%=DateFromTextBox.ClientID%>').datepicker(
                {
                    dateFormat: 'dd-M-yy',
                });
        });

    </script>
</asp:Content>
