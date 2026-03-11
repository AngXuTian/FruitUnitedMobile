<%@ Page Title="" Language="C#" MasterPageFile="~/ModulesPage.Master" AutoEventWireup="true" CodeBehind="Submitted-Task.aspx.cs" Inherits="FruitUnitedMobile.Modules.Submitted_Task" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .table tbody tr:hover {
    background-color: #f1f1f1;
}
.container .label-column {
        vertical-align: top;
    }

    @media (max-width: 768px) {
        .container .label-column {
            vertical-align: top; /* Align top in mobile view */
        }
    }

    </style>
        <link href="../CSS/Basic.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
  <div class="container" style="border:5px solid black; width:95%; overflow-x: hidden; background-color: white;">
    <table style="width: 100%; border-collapse: collapse;">
        <tr>
            <td class="label-column" style="font-weight: bold;">Project:</td>
            <td class="label-column"><asp:Label runat="server" ID="txtProject"></asp:Label></td>
        </tr>
        <tr>
            <td class="label-column" style="font-weight: bold;">From:</td>
            <td class="label-column"><asp:Label runat="server" ID="txtFrom"></asp:Label></td>
        </tr>
        <tr>
            <td class="label-column" style="font-weight: bold;">To:</td>
            <td class="label-column"><asp:Label runat="server" ID="txtTo"></asp:Label></td>
        </tr>
        <tr>
            <td class="label-column" style="font-weight: bold;">Scope:</td>
            <td class="label-column"><asp:Label runat="server" ID="txtScope"></asp:Label></td>
        </tr>
         <tr>
            <td class="label-column" style="font-weight: bold;">Scope Info:</td>
            <td class="label-column"><asp:Label runat="server" ID="txtScopeInfo"></asp:Label></td>
        </tr>
    </table>
</div>

       <!-- Search Icon -->
<%--<div class="container">
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
        <div class="col-md-4">
            <asp:TextBox ID="SONoTextBox" runat="server" CssClass="form-control" Placeholder="Search by Task"></asp:TextBox>
        </div>
    </div>
    <div class="row">
        <div class="col-md-12" style="text-align: right;">
            <asp:Button ID="SearchButton" runat="server" Text="Search" CssClass="btn btn-primary" OnClick="SearchButton_Click" />
        </div>
    </div>
</div>
</div>--%>

    <div id="GridDiv" class="container">
        <div class="row" style="margin-top: 10px;">
            <div class="col-12" id="OutputGridDiv" style="overflow: auto;">
                <asp:Panel ID="GridPanel" runat="server" ScrollBars="Vertical" Style="max-height: 65vh; overflow: auto; width: 100%;">
                <asp:GridView ID="InvoiceGrid" runat="server" AutoGenerateColumns="false" OnRowCommand="MyGridView_RowCommand" OnRowDataBound="InvoiceGrid_RowDataBound" GridLines="Both" CssClass="table table-bordered table-responsive">
                    <Columns>
                        <asp:TemplateField HeaderText="Completion Info">
                            <ItemTemplate>
                                <asp:Label ID="Label1" Text='<%# Bind("Task") %>' Visible='<%# !string.IsNullOrEmpty(Eval("Task")?.ToString()) %>' Font-Size="Small" runat="server"></asp:Label>
                                 <asp:Literal ID="Literal1" Text="<br>" runat="server" Visible='<%# !string.IsNullOrEmpty(Eval("Task")?.ToString()) %>'></asp:Literal>
                                
                                <asp:Label ID="Label6" Text='<%# "Task Info: "+Eval("Task_Info") %>' Visible='<%# !string.IsNullOrEmpty(Eval("Task_Info")?.ToString()) %>' Font-Size="Small" runat="server"></asp:Label>
                                  <asp:Literal ID="Literal2" Text="<br>" runat="server" Visible='<%# !string.IsNullOrEmpty(Eval("Task_Info")?.ToString()) %>'></asp:Literal>
                                
                                <asp:Label ID="Label2" Text='<%# "Qty: " + Eval("Pending_Qty") %>' Visible='<%# !string.IsNullOrEmpty(Eval("Pending_Qty")?.ToString()) %>' Font-Size="Small" runat="server"></asp:Label>
                                 <asp:Literal ID="Literal7" Text="<br>" runat="server"></asp:Literal>
                                <asp:Label ID="Label4" Text='<%# "By: " + Eval("Completed_By") %>' Visible='<%# !string.IsNullOrEmpty(Eval("Completed_By")?.ToString()) %>' Font-Size="Small" runat="server"></asp:Label>
                                <asp:Label ID="Label8" Text='<%# "   On: " + Eval("Completion_Date") %>' Visible='<%# !string.IsNullOrEmpty(Eval("Completion_Date")?.ToString()) %>' Font-Size="Small" runat="server"></asp:Label>
                                  <asp:Literal ID="Literal3" Text="<br>" runat="server" Visible='<%# !string.IsNullOrEmpty(Eval("Completed_By")?.ToString()) %>'></asp:Literal>
                                
                                <asp:Label ID="Label3" Text='<%# "Remark: " + Eval("Remark") %>' Visible='<%# !string.IsNullOrEmpty(Eval("Remark")?.ToString()) %>' Font-Size="Small" runat="server"></asp:Label>
                                <asp:Label ID="Label5" Text='<%# "Exceeded Reason: " + Eval("Exceeded_Reason") %>' Visible='<%# !string.IsNullOrEmpty(Eval("Exceeded_Reason")?.ToString()) %>' Font-Size="Small" runat="server"></asp:Label>
                                 <asp:Literal ID="Literal4" Text="<br>" runat="server" Visible='<%# !string.IsNullOrEmpty(Eval("Exceeded_Reason")?.ToString()) %>'></asp:Literal>

                                </ItemTemplate>
                        </asp:TemplateField>
                       
                        <asp:TemplateField HeaderText="Info">
                            <ItemTemplate>
                                <div id="linkBtnInfo">
                                    <asp:LinkButton ID="viewInfo" runat="server" Text="View"><i class="fa fa-search" aria-hidden="true"></i></asp:LinkButton>
                                </div>
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
                        </asp:TemplateField>

                         <asp:TemplateField HeaderText="Image">
                            <ItemTemplate>
                                <div id="linkBtnImage">
                                    <asp:LinkButton ID="viewImage" runat="server"><i class="fa fa-search" aria-hidden="true"></i>
</asp:LinkButton>
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
                        </asp:TemplateField>

                         <asp:TemplateField HeaderText="Delete">
                            <ItemTemplate>
                                <div id="verifyIcon">
                                    <asp:LinkButton ID="lnkVerify" runat="server" CommandName="Verify" CommandArgument="Verify">
                                        <i class="fa fa-times" style="color:red; font-size:20px; cursor:pointer;"></i>
                                    </asp:LinkButton>
                                </div>
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:Label ID="Project_Task_Completion_ID" runat="server" Text='<%# Bind("Project_Task_Completion_ID") %>' Visible="false"></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Project_Scope_Task_ID">
                            <ItemTemplate>
                                <asp:Label ID="Project_Scope_Task_ID" runat="server" Text='<%# Bind("Project_Scope_Task_ID") %>' ></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
                    </asp:Panel>
            </div>
        </div>

         <br><br><br><br>
        <div style="position: fixed; bottom: 0; left: 0; width: 100vw; background-color: white; border-top: 1px solid #ccc; padding: 10px;">
            <div style="display: flex; justify-content: space-between; align-items: center;">
                <div>
                    <asp:Button ID="btnToProject" runat="server" Text="Project" CausesValidation="false"  OnClick="btnToProject_Click" CssClass="btn btn-secondary" style="padding: 10px 20px;" />
                    <asp:Button ID="btnToScope" runat="server" Text="Scope" CausesValidation="false"  OnClick="btnToScope_Click" CssClass="btn btn-secondary" style="padding: 10px 20px;" />
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
        window.onpageshow = function (event) {
            if (event.persisted || (window.performance && window.performance.navigation.type === 2)) {
                window.location.reload();
            }
        };


    </script>
</asp:Content>
