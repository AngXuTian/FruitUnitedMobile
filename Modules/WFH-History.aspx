<%@ Page Title="" Language="C#" MasterPageFile="~/ModulesPage.Master" AutoEventWireup="true" CodeBehind="WFH-History.aspx.cs" Inherits="FruitUnitedMobile.Modules.WFH_History" %>

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
        <asp:Label ID="ww" runat="server" />
        <div id="search-container" class="row" style="display: none;">
            <div class="row mb-3">
                <div class="col-md-3">
                    <asp:TextBox ID="DateFromTextBox" runat="server" CssClass="form-control FilterTextBox" Placeholder="Search by From Date"></asp:TextBox>
                </div>
                <div class="col-md-3">
                    <asp:TextBox ID="DateToTextBox" runat="server" CssClass="form-control FilterTextBox" Placeholder="Search by To Date"></asp:TextBox>
                </div>
                <div class="col-md-3">
                    <asp:TextBox ID="EmployeeTextBox" runat="server" CssClass="form-control FilterTextBox" Placeholder="Search by Employee"></asp:TextBox>
                </div>
                <div class="col-md-3">
                    <asp:TextBox ID="DepartmentTextBox" runat="server" CssClass="form-control FilterTextBox" Placeholder="Search by Department"></asp:TextBox>
                </div>
            </div>
            <div class="row">
                <div class="col-md-12" style="text-align: right; padding-top: 2px">
                    <asp:Button ID="SearchButton" runat="server" Text="Search" CssClass="btn btn-primary" OnClick="SearchButton_Click" />
                </div>
            </div>
        </div>
    </div>
    <div id="GridDiv" class="container">
        <div class="row" style="margin-top: 10px;">
            <div class="col-12" id="OutputGridDiv" style="overflow: auto;">
                <asp:Panel ID="GridPanel" runat="server" ScrollBars="Vertical" Style="max-height: 65vh; overflow: auto; width: 100%;">
                    <asp:GridView ID="WFHHistoryGrid" runat="server" AutoGenerateColumns="false" OnRowDataBound="WFHHistoryGrid_RowDataBound" OnRowCommand="WFHHistoryGrid_RowCommand" CssClass="table table-bordered table-responsive">
                        <Columns>
                            <asp:TemplateField HeaderText="Date" ItemStyle-CssClass="col-2">
                                <ItemTemplate>
                                    <asp:Label ID="LabelDate" Text='<%# Bind("WFH_Date") %>' Font-Size="Small" runat="server"></asp:Label>
                                    <asp:HiddenField ID="StatusHF" Value='<%# Bind("Status") %>' runat="server"></asp:HiddenField>
                                    <asp:HiddenField ID="Employee_Profile_IDHF" Value='<%# Bind("Employee_Profile_ID") %>' runat="server"></asp:HiddenField>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Employee" ItemStyle-CssClass="col-2">
                                <ItemTemplate>
                                    <asp:Label ID="LabelEmployee" Text='<%# Bind("Display_Name") %>' Font-Size="Small" runat="server"></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Department" ItemStyle-CssClass="col-2">
                                <ItemTemplate>
                                    <asp:Label ID="LabelDepartment" Text='<%# Bind("Department") %>' Font-Size="Small" runat="server"></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Remarks" ItemStyle-CssClass="col-5">
                                <ItemTemplate>
                                    <asp:Label ID="LabelRemarks" Text='<%# Bind("Remarks") %>' Font-Size="Small" runat="server"></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Cancel" ItemStyle-HorizontalAlign="Center" ItemStyle-CssClass="col-2">
                                <ItemTemplate>
                                    <div id="linkBtnDiv">
                                        <asp:LinkButton ID="ViewFile" runat="server" CommandName="CancelWFH" CommandArgument='<%# Eval("WFH_ID") %>'><i class="fa fa-times" aria-hidden="true"></i></asp:LinkButton>
                                    </div>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </asp:Panel>
            </div>
        </div>
    </div>
    <link rel="stylesheet" href="https://code.jquery.com/ui/1.14.1/themes/base/jquery-ui.css">
    <script src="https://code.jquery.com/jquery-3.7.1.js"></script>
    <script src="https://code.jquery.com/ui/1.14.1/jquery-ui.js"></script>
    <link href="https://cdn.jsdelivr.net/npm/select2@4.1.0-rc.0/dist/css/select2.min.css" rel="stylesheet" />
    <script src="https://cdn.jsdelivr.net/npm/select2@4.1.0-rc.0/dist/js/select2.min.js"></script>
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
            var dateTo = $("#" + '<%=DateToTextBox.ClientID%>');
            var dateFrom = $("#" + '<%=DateFromTextBox.ClientID%>');

            // Initialize datepickers
            dateTo.datepicker({
                dateFormat: 'dd-M-yy'
            });

            dateFrom.datepicker({
                dateFormat: 'dd-M-yy'
            });
            /*
            // Log values on change
            dateTo.on("change", function () {
                console.log("Date To:", $(this).val());
            });

            dateFrom.on("change", function () {
                console.log("Date From:", $(this).val());
            });
            */
        });
    </script>
</asp:Content>
