<%@ Page Title="" Language="C#" MasterPageFile="~/ModulesPage.Master" AutoEventWireup="true" CodeBehind="View-Leave.aspx.cs" Inherits="FruitUnitedMobile.Modules.View_Leave" %>

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
                    <asp:TextBox ID="employeeTextBox" runat="server" CssClass="form-control FilterTextBox" Placeholder="Search by Employee"></asp:TextBox>
                </div>
                <div class="col-md-4">
                    <asp:TextBox ID="DateFromTextBox" runat="server" CssClass="form-control FilterTextBox" Placeholder="From Date"></asp:TextBox>
                </div>
                <div class="col-md-4">
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
                    <asp:GridView ID="DataGrid" runat="server" AutoGenerateColumns="false" OnRowCommand="MyGridView_RowCommand" OnRowDataBound="Grid_RowDataBound" CssClass="table table-bordered table-responsive">
                        <Columns>
                            <asp:TemplateField HeaderText="Leave Info">
                                <ItemTemplate>
                                    <asp:Label ID="lblEmployee" Text='<%#Bind("Display_Name")%>' Font-Size="Small" Font-Bold="true" runat="server"></asp:Label>
                                    <asp:Label ID="Label1" Text='(' Font-Size="Small" runat="server"></asp:Label>
                                    <asp:Label ID="lblShowStatus" Text='<%#Bind("Status")%>' Font-Size="Small" runat="server"></asp:Label>
                                    <asp:Label ID="Label2" Text=')' Font-Size="Small" runat="server"></asp:Label>
                                    <br>
                                    <asp:Label ID="lblDate" Text='<%# Bind("Leave_Date") %>' Font-Size="Small" runat="server"></asp:Label>
                                    <asp:Label ID="lblSession" Text='<%# Bind("Leave_Session") %>' Font-Size="Small" runat="server"></asp:Label>
                                    <br>
                                    <asp:Label ID="lblType" Text='<%# Bind("Leave_Type") %>' Font-Size="Small" runat="server"></asp:Label>
                                    <asp:Literal ID="Literal6" Text="<br>" runat="server" Visible='<%# !string.IsNullOrEmpty(Eval("Remarks")?.ToString()) %>'></asp:Literal>
                                    <asp:Label ID="lblRemarks" Text='<%# Bind("Remarks") %>' Visible='<%# !string.IsNullOrEmpty(Eval("Remarks")?.ToString()) %>' Font-Size="Small" runat="server"></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Approve">
                                <ItemTemplate>
                                    <div id="approveIcon">
                                        <asp:LinkButton ID="lnkApprove" runat="server" CommandName="Approve" CommandArgument="Approve">
                                        <i class="fa fa-check" style="color:green; font-size:20px; cursor:pointer;"></i>
                                    </asp:LinkButton>
                                    </div>
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Cancel">
                                <ItemTemplate>
                                    <div id="cancelIcon">
                                        <asp:LinkButton ID="lnkCancel" runat="server" CommandName="CancelDD" CommandArgument="CancelDD">
                                        <i class="fa fa-times" style="color:red; font-size:20px; cursor:pointer;"></i>
                                    </asp:LinkButton>
                                    </div>
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
                            </asp:TemplateField>
                            <asp:TemplateField>
                                <ItemTemplate>
                                    <asp:Label ID="Leave_Application_ID" runat="server" Text='<%# Bind("Leave_Application_ID") %>' Visible="false"></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField>
                                <ItemTemplate>
                                    <asp:Label ID="lblStatus" runat="server" Text='<%# Bind("Status") %>' Visible="false"></asp:Label>
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

        $(function () {
            $("#" + '<%=DateFromTextBox.ClientID%>').datepicker(
                {
                    dateFormat: 'dd-M-yy',
                });

            $("#" + '<%=DateToTextBox.ClientID%>').datepicker(
                {
                    dateFormat: 'dd-M-yy',
                });
        });
    </script>
</asp:Content>
