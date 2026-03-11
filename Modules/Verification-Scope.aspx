<%@ Page Title="" Language="C#" MasterPageFile="~/ModulesPage.Master" AutoEventWireup="true" CodeBehind="Verification-Scope.aspx.cs" Inherits="FruitUnitedMobile.Modules.Verification_Scope" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
        <link href="../CSS/Basic.css" rel="stylesheet" />
    <%--<div class="container">
     <div style="margin-bottom: 20px;">
            <asp:Label runat="server" Text="Project:" AssociatedControlID="txtProject"></asp:Label>
            <asp:TextBox ID="txtProject" runat="server" Text="" ReadOnly="true" CssClass="form-control" style="width: 100%; margin-bottom: 10px;"></asp:TextBox>
        </div>
    </div>--%>
    <div class="container" style="border:5px solid black; width:95%; overflow-x: hidden; background-color: white;">
     <div>
            <asp:Label runat="server" Font-Bold="true" Text="Project:" AssociatedControlID="txtProject"></asp:Label>
            <asp:Label runat="server" ID="txtProject"></asp:Label>
            <%--<asp:TextBox ID="txtProject" runat="server" Text="" ReadOnly="true" CssClass="form-control" style="width: 100%; margin-bottom: 10px;"></asp:TextBox>--%>
        </div>
    </div>
    <!-- Search Icon -->
    <div class="container">
    <div class="row">
        <div class="col-md-12 text-right" style="text-align:right;">
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
                <asp:DropDownList ID="ddlFromBuilding" runat="server" CssClass="form-control" AppendDataBoundItems="true">
                </asp:DropDownList>
            </div>
            <div class="col-md-3">
                <asp:DropDownList ID="ddlFromFloor" runat="server" CssClass="form-control" AppendDataBoundItems="true">
                </asp:DropDownList>
            </div>
            <div class="col-md-3">
                <asp:DropDownList ID="ddlFromLocation" runat="server" CssClass="form-control" AppendDataBoundItems="true">
                </asp:DropDownList>
            </div>
            <div class="col-md-3">
                <asp:DropDownList ID="ddlToBuilding" runat="server" CssClass="form-control" AppendDataBoundItems="true">
                </asp:DropDownList>
            </div>
        </div>
        <div class="row mb-3">
            <div class="col-md-3">
                <asp:DropDownList ID="ddlToFloor" runat="server" CssClass="form-control" AppendDataBoundItems="true">
                </asp:DropDownList>
            </div>
            <div class="col-md-3">
                <asp:DropDownList ID="ddlToLocation" runat="server" CssClass="form-control" AppendDataBoundItems="true">
                </asp:DropDownList>
            </div>
            <div class="col-md-3">
                <asp:DropDownList ID="ddlSystem" runat="server" CssClass="form-control" AppendDataBoundItems="true">
                </asp:DropDownList>
            </div>
            <div class="col-md-3">
                <asp:DropDownList ID="ddlType" runat="server" CssClass="form-control" AppendDataBoundItems="true">
                </asp:DropDownList>
            </div>
        </div>
        <div class="col-md-12" style="text-align: right;">
            <asp:Button ID="SearchButton" runat="server" Text="Search" CssClass="btn btn-primary" OnClick="SearchButton_Click" />
        </div>
    </div>
    </div>

    


    <div id="GridDiv" class="container">
        <div class="row" style="margin-top: 10px;">
            <div class="col-12" id="OutputGridDiv" style="overflow: auto;">
                <asp:Panel ID="GridPanel" runat="server" ScrollBars="Vertical" Style="max-height: 65vh; overflow: auto; width: 100%;">
                <asp:GridView ID="InvoiceGrid" runat="server" AutoGenerateColumns="false" OnRowDataBound="InvoiceGrid_RowDataBound" CssClass="table table-bordered table-responsive">
                    <Columns>
                        <asp:TemplateField HeaderText="Scope of Work">
                            <ItemTemplate>
                                <asp:Label ID="Label1" Text='<%# Bind("Scope_of_Work") %>'  Font-Size="Small" Font-Bold="true" runat="server" Visible='<%# !string.IsNullOrEmpty(Eval("Scope_of_Work")?.ToString()) %>'></asp:Label>
                                <asp:Literal ID="Literal1" Text="<br>" runat="server" Visible='<%# !string.IsNullOrEmpty(Eval("Scope_of_Work")?.ToString()) %>'></asp:Literal>
                                
                                <asp:Label ID="Label11" Text='<%# Bind("Scope_Info") %>' Visible='<%# !string.IsNullOrEmpty(Eval("Scope_Info")?.ToString()) %>' Font-Size="Small" Font-Bold="true" runat="server"></asp:Label>
                                <asp:Literal ID="Literal2" Text="<br>" runat="server" Visible='<%# !string.IsNullOrEmpty(Eval("Scope_Info")?.ToString()) %>'></asp:Literal>
                                
                                <asp:Label ID="Label2" Text='<%# Bind("System_Type") %>' Visible='<%# !string.IsNullOrEmpty(Eval("System_Type")?.ToString()) %>' Font-Size="Small" runat="server"></asp:Label>
                                <asp:Literal ID="Literal3" Text="<br>" runat="server" Visible='<%# !string.IsNullOrEmpty(Eval("System_Type")?.ToString()) %>'></asp:Literal>
                                
                                <asp:Label ID="Label4" Text='<%# Bind("Type") %>' Visible='<%# !string.IsNullOrEmpty(Eval("Type")?.ToString()) %>' Font-Size="Small" runat="server"></asp:Label>
                                 <asp:Literal ID="Literal4" Text="<br>" runat="server" Visible='<%# !string.IsNullOrEmpty(Eval("Type")?.ToString()) %>'></asp:Literal>
                                
                                <asp:Label ID="Label3" Text='<%# Bind("Height") %>' Visible='<%# !string.IsNullOrEmpty(Eval("Height")?.ToString()) %>' Font-Size="Small" runat="server"></asp:Label>
                                <asp:Label ID="Label5" Text='<%# Bind("Ladder") %>' Visible='<%# !string.IsNullOrEmpty(Eval("Ladder")?.ToString()) %>' Font-Size="Small" runat="server"></asp:Label>
                                <asp:Literal ID="Literal5" Text="<br>" runat="server" Visible='<%# !string.IsNullOrEmpty(Eval("Height")?.ToString())  %>'></asp:Literal>
                                
                                <asp:Label ID="Label6" Text='<%# Bind("From_Location") %>' Visible='<%# !string.IsNullOrEmpty(Eval("From_Location")?.ToString()) %>' Font-Size="Small" runat="server"></asp:Label>
                                <asp:Literal ID="Literal6" Text="<br>" runat="server" Visible='<%# !string.IsNullOrEmpty(Eval("From_Location")?.ToString()) %>'></asp:Literal>
                                
                                <asp:Label ID="Label7" Text='<%# Bind("To_Location") %>' Visible='<%# !string.IsNullOrEmpty(Eval("To_Location")?.ToString()) %>' Font-Size="Small" runat="server"></asp:Label>
                                <asp:Literal ID="Literal7" Text="<br>" runat="server" Visible='<%# !string.IsNullOrEmpty(Eval("To_Location")?.ToString()) %>'></asp:Literal>
                                
                                <asp:Label ID="Label8" Text='<%# Bind("Label_Convention") %>' Visible='<%# !string.IsNullOrEmpty(Eval("Label_Convention")?.ToString()) %>' Font-Size="Small" runat="server"></asp:Label>
                                </ItemTemplate>
                        </asp:TemplateField>
                       
                        <asp:TemplateField HeaderText="Qty">
                            <ItemTemplate>
                                <div id="linkBtnDiv">
                                    <asp:Label ID="Label10" Text='<%# Bind("Qty") %>' Font-Size="Small" runat="server"></asp:Label>                                </div>
                                </div>
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
                        </asp:TemplateField>

                         <asp:TemplateField HeaderText="(%)">
                            <ItemTemplate>
                                <div id="linkBtnDiv">
                                    <asp:Label ID="Label9" Text='<%# Bind("Completion_Percent") %>' Font-Size="Small" runat="server"></asp:Label>                                </div>
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
                        </asp:TemplateField>

                         <asp:TemplateField HeaderText="Task">
                            <ItemTemplate>
                                <div id="linkBtnDiv">
                                    <asp:LinkButton ID="ViewInvoice" runat="server"><i class="fa fa-search" aria-hidden="true"></i>
</asp:LinkButton>
                                </div>
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
                        </asp:TemplateField>

                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:Label ID="Project_Scope_ID" runat="server" Text='<%# Bind("Project_Scope_ID") %>' Visible="false"></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
                    </asp:Panel>
                <div></div>
            </div>
        </div>

         <br><br><br><br>
    <div style="position: fixed; bottom: 0; left: 0; width: 100vw; background-color: white; border-top: 1px solid #ccc; padding: 10px;">
            <div style="display: flex; justify-content: space-between; align-items: center;">
                <div>
                    <asp:Button ID="btnToProject" runat="server" Text="Project" CausesValidation="false"  OnClick="btnToProject_Click" CssClass="btn btn-secondary" style="padding: 10px 20px;" />
                </div>
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
