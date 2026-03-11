<%@ Page Title="" Language="C#" MasterPageFile="~/ModulesPage.Master" AutoEventWireup="true" CodeBehind="Leave-Approval.aspx.cs" Inherits="FruitUnitedMobile.Modules.Leave_Approval" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
        <link href="../CSS/Basic.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="GridDiv" class="container">
        <div class="row" style="margin-top: 10px;">
            <div class="col-12" id="OutputGridDiv" style="overflow: auto;">
                <asp:Panel ID="GridPanel" runat="server" ScrollBars="Vertical" Style="max-height: 65vh; overflow: auto; width: 100%;">
                    <asp:GridView ID="DataGrid" runat="server" AutoGenerateColumns="false" OnRowCommand="MyGridView_RowCommand" CssClass="table table-bordered table-responsive">
                        <columns>
                            <asp:TemplateField HeaderText="Leave Info">
                                <itemtemplate>
                                    <asp:Label ID="Label1" Text='<%# Bind("Display_Name") %>' Font-Size="Small" Font-Bold="true" runat="server"></asp:Label>
                                    <br>
                                    <asp:Label ID="Label2" Text='<%# Bind("Leave_Date") %>     ' Font-Size="Small" runat="server"></asp:Label>
                                    <asp:Label ID="Label4" Text='<%# Bind("Leave_Session") %>' Font-Size="Small" runat="server"></asp:Label>
                                    <br>
                                    <asp:Label ID="Label3" Text='<%# Bind("Leave_Type") %>' Font-Size="Small" runat="server"></asp:Label>
                                    <asp:Literal ID="Literal6" Text="<br>" runat="server" Visible='<%# !string.IsNullOrEmpty(Eval("Remarks")?.ToString()) %>'></asp:Literal>
                                    <asp:Label ID="Label6" Text='<%# Bind("Remarks") %>' Visible='<%# !string.IsNullOrEmpty(Eval("Remarks")?.ToString()) %>' Font-Size="Small" runat="server"></asp:Label>
                                </itemtemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Approval">
                                <itemtemplate>
                                    <div id="approvalIcon">
                                        <asp:LinkButton ID="lnkApprove" runat="server" CommandName="Approve" CommandArgument="Approve">
                                            <i class="fa fa-check" style="color: green; font-size: 20px; cursor: pointer;"></i>
                                        </asp:LinkButton>
                                    </div>
                                </itemtemplate>
                                <itemstyle horizontalalign="Center" verticalalign="Middle"></itemstyle>
                            </asp:TemplateField>
                            <asp:TemplateField>
                                <itemtemplate>
                                    <asp:Label ID="Leave_Application_ID" runat="server" Text='<%# Bind("Leave_Application_ID") %>' Visible="false"></asp:Label>
                                </itemtemplate>
                            </asp:TemplateField>
                        </columns>
                    </asp:GridView>
                </asp:Panel>
                <div></div>
            </div>
        </div>
    </div>
</asp:Content>
