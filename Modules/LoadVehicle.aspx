<%@ Page Title="Load / Complete Delivery" Language="C#" MasterPageFile="~/ModulesPage.Master"
    AutoEventWireup="true"
    CodeBehind="LoadVehicle.aspx.cs"  
    Inherits="FruitUnitedMobile.Modules.LoadVehicle" %>

<%@ Register Src="~/Component/Toast.ascx" TagPrefix="uc" TagName="Toast" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="<%= ResolveUrl("~/CSS/ButtonFooter.css") %>" rel="stylesheet" />
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <uc:Toast ID="Toast1" runat="server" />

    <div class="container" style="padding: 20px; padding-bottom: 100px;">
        <h2>Vehicle Operations</h2>
        <hr />
        
        <h4 ID="h4StatusHeader" runat="server" class="mb-3 text-primary"></h4>

        <div style="margin-bottom: 15px;" class="table-responsive">
            <asp:GridView ID="gvLoadingItems" runat="server" 
                AutoGenerateColumns="true" 
                CssClass="table table-bordered table-sm">
                <EmptyDataTemplate>
                    <div class="alert alert-light text-center">
                        No products currently loaded for this vehicle.
                    </div>
                </EmptyDataTemplate>
            </asp:GridView>
        </div>
        
        <div class="button-footer d-flex justify-content-between px-3 py-2 bg-white border-top">
            <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="~/Modules/Menu.aspx" CssClass="btn btn-secondary">
               Back
            </asp:HyperLink>

            <div class="d-flex gap-2">
                <%-- Visible when status is 'Ready' --%>
                <asp:Button ID="btnLoadVehicle" runat="server" 
                    Text="Confirm Load" 
                    CssClass="btn btn-success" 
                    OnClick="btnLoadVehicle_Click" Visible="false" />

                <%-- Visible when status is 'Loaded' --%>
                <asp:Button ID="btnTriggerComplete" runat="server" 
                    Text="Complete Delivery" 
                    CssClass="btn btn-primary" 
                    OnClientClick="return confirmCompletion();" Visible="false" />
                
                <%-- Hidden button to trigger real PostBack --%>
                <asp:Button ID="btnActualComplete" runat="server" 
                    OnClick="btnCompleteDelivery_Click" style="display:none;" />
            </div>
        </div>
    </div>

    <script type="text/javascript">
        var vehicleHasBalance = false;

        function setHasBalance(val) {
            vehicleHasBalance = val;
        }

        function confirmCompletion() {
            var title = "Confirm Completion?";
            var text = vehicleHasBalance ?
                "Selected vehicle is still with on hand balance. Click OK to proceed completion. Click Cancel to cancel completion action." :
                "Are you sure you want to mark this delivery as done?";

            Swal.fire({
                title: title,
                text: text,
                icon: vehicleHasBalance ? 'warning' : 'question',
                showCancelButton: true,
                confirmButtonColor: '#3085d6',
                cancelButtonColor: '#d33',
                confirmButtonText: 'Yes, Complete',
                cancelButtonText: 'Cancel'
            }).then((result) => {
                if (result.isConfirmed) {
                    document.getElementById('<%= btnActualComplete.ClientID %>').click();
                }
            });
            return false;
        }
    </script>
</asp:Content>