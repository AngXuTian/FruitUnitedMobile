<%@ Page Title="SISTA - Delivery Plan" Language="C#" MasterPageFile="~/ModulesPage.Master"
    AutoEventWireup="true" CodeBehind="Delivery_Plan.aspx.cs" 
    Inherits="FruitUnitedMobile.Modules.Delivery_Plan" %>
<%@ Register Src="~/Component/Toast.ascx" TagPrefix="uc" TagName="Toast" %>
<%@ Register Src="~/Component/SearchBar.ascx" TagPrefix="uc" TagName="SearchBar" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="../CSS/Basic.css" rel="stylesheet" />
    <link href="../CSS/ButtonFooter.css" rel="stylesheet" />
    <style>
        .step-container { display: none; }
        .step-container.active { display: block; }
        .form-section {
            background: white; padding: 20px; border-radius: 8px;
            box-shadow: 0 2px 4px rgba(0,0,0,0.1); margin-bottom: 20px;
        }
        .step-title {
            color: #333; margin-bottom: 20px; padding-bottom: 10px;
            border-bottom: 2px solid #007bff;
        }
        .info-badge {
            background: #e7f3ff; padding: 10px; border-radius: 5px; margin-bottom: 15px;
        }
        .required-field::after { content: " *"; color: red; }

        /* Outlet Cards */
        .outlet-card-link { text-decoration: none; color: inherit; display: block; margin-bottom: 15px; }
        .outlet-card {
            background: white; border: 1px solid #ddd; border-radius: 8px;
            box-shadow: 0 1px 3px rgba(0,0,0,0.1); overflow: hidden; cursor: pointer;
            transition: all 0.2s ease; position: relative;
        }
        .outlet-card:hover { border-color: #007bff; box-shadow: 0 2px 6px rgba(0,123,255,0.2); }
        .outlet-card:active { transform: scale(0.98); background: #f8f9fa; }
        .outlet-header {
            background: #f8f9fa; border-bottom: 1px solid #dee2e6;
            padding: 12px 15px; display: flex; justify-content: space-between; align-items: center;
        }
        .outlet-postcode { font-weight: bold; font-size: 1.1rem; color: #333; }
        .outlet-postcode i { margin-right: 5px; color: #6c757d; }
        .outlet-number {
            background: #e9ecef; color: #495057; padding: 4px 12px;
            border-radius: 4px; font-size: 0.9rem; font-weight: bold;
        }
        .outlet-body { padding: 15px 50px 15px 15px; }
        .outlet-name { font-size: 1.2rem; font-weight: bold; color: #333; margin-bottom: 10px; }
        .info-item {
            display: flex; align-items: flex-start; margin-bottom: 8px;
            color: #666; font-size: 0.95rem;
        }
        .info-item i { margin-right: 8px; margin-top: 3px; color: #6c757d; min-width: 16px; }
        .outlet-arrow {
            position: absolute; right: 15px; top: 50%; transform: translateY(-50%);
            color: #6c757d; font-size: 1.2rem;
        }

        .step-container { padding-bottom: 70px; }

        @media (max-width: 768px) {
            .outlet-name { font-size: 1.1rem; }
            .info-item { font-size: 0.9rem; }
        }
        .ajax-loading { opacity: 0.6; pointer-events: none; }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePartialRendering="true" />
    
    <uc:Toast ID="Toast1" runat="server" />
    <div class="container" style="padding: 20px;">
        <h2 class="text-center mb-4">SISTA - Delivery Plan</h2>

        <!-- Step 1: Select Plan -->
        <div class="step-container active" id="step1">
            <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <div class="form-section">
                        <h3 class="step-title">Step 1: Select Delivery Plan & Date</h3>
                        
                        <div class="mb-3">
                            <label for="txtDeliveryDate" class="form-label required-field">Delivery Date</label>
                            <asp:TextBox ID="txtDeliveryDate" runat="server" CssClass="form-control"
                                TextMode="Date" AutoPostBack="true"
                                OnTextChanged="txtDeliveryDate_TextChanged" required="required"></asp:TextBox>
                            <small class="text-muted">Select a date within the current week</small>
                        </div>

                        <div class="mb-3">
                            <label for="ddlPlanName" class="form-label required-field">Plan Name</label>
                            <asp:DropDownList ID="ddlPlanName" runat="server" CssClass="form-select" 
                                AutoPostBack="true" OnSelectedIndexChanged="ddlPlanName_SelectedIndexChanged">
                                <asp:ListItem Value="">-- Select Plan --</asp:ListItem>
                            </asp:DropDownList>
                        </div>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>

            <div class="button-footer">
                <asp:Button ID="btnBackToHome" runat="server" Text="Back" CssClass="btn btn-secondary"
                    OnClick="btnBackToHome_Click" />
                <asp:Button ID="btnLoadPlan" runat="server" Text="Load Plan" 
                    CssClass="btn btn-primary btn-lg" OnClick="btnLoadPlan_Click" />
            </div>
        </div>

        <!-- Step 2: Display Outlets -->
        <div class="step-container" id="step2">
            <div class="form-section">
                <h3 class="step-title">Step 2: Select Outlet</h3>
                
                <div class="info-badge mb-3">
                    <strong>Delivery Date:</strong> <asp:Label ID="lblSelectedDate" runat="server"></asp:Label><br />
                    <strong>Plan:</strong> <asp:Label ID="lblSelectedPlan" runat="server"></asp:Label><br />
                    <strong>Delivery Day:</strong> <asp:Label ID="lblDeliveryDay" runat="server"></asp:Label>
                </div>

                <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <uc:SearchBar ID="SearchBar1" runat="server" 
                            Placeholder="Search by postcode or outlet name..." 
                            SearchInfoText="Type to search outlets by postcode or name"
                            SearchFields="data-postcode,data-outletname" />

                        <div data-results-container="true">
                            <asp:Repeater ID="rptOutlets" runat="server" OnItemCommand="rptOutlets_ItemCommand">
                                <ItemTemplate>
                                    <asp:LinkButton ID="lnkOutletCard" runat="server" 
                                        CommandName="SelectOutlet"
                                        CommandArgument='<%# Eval("Outlet_Profile_ID") + "|" + Eval("Delivery_Outlet_ID") %>'
                                        CssClass="outlet-card-link"
                                        data-postcode='<%# Eval("Postcode") %>'
                                        data-outletname='<%# Eval("Outlet_Name") %>'
                                        data-searchable="true">
                                        
                                        <div class="outlet-card">
                                            <div class="outlet-header">
                                                <div class="outlet-postcode">
                                                    <i class="fa fa-map-marker"></i> <%# Eval("Postcode") %>
                                                </div>
                                                <div class="outlet-number">
                                                    #<%# Eval("Outlet_Number") %>
                                                </div>
                                            </div>
                                            <div class="outlet-body">
                                                <h5 class="outlet-name"><%# Eval("Outlet_Name") %></h5>
                                                <div class="outlet-info">
                                                    <div class="info-item">
                                                        <i class="fa fa-map-marker"></i>
                                                        <span><%# Eval("Address") %></span>
                                                    </div>
                                                    <asp:Panel ID="pnlContact" runat="server" 
                                                        Visible='<%# !string.IsNullOrEmpty(Eval("Primary_Contact")?.ToString()) %>' 
                                                        CssClass="info-item">
                                                        <i class="fa fa-phone"></i>
                                                        <span><%# Eval("Primary_Contact") %></span>
                                                    </asp:Panel>
                                                </div>
                                            </div>
                                            <div class="outlet-arrow">
                                                <i class="fa fa-chevron-right"></i>
                                            </div>
                                        </div>
                                    </asp:LinkButton>
                                </ItemTemplate>
                            </asp:Repeater>
                        </div>

                        <asp:Label ID="lblNoOutlets" runat="server" 
                            CssClass="alert alert-info d-block text-center py-4" 
                            Visible="false" 
                            Text="No outlets scheduled for delivery on this date."
                            data-no-results="true"></asp:Label>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>

            <div class="button-footer">
                <asp:Button ID="btnBackToStep1" runat="server" Text="Back" 
                    CssClass="btn btn-secondary" OnClick="btnBackToStep1_Click" />
            </div>
        </div>

        <asp:HiddenField ID="hfSelectedOutletID" runat="server" />
        <asp:HiddenField ID="hfSelectedDeliveryOutletID" runat="server" />
        <asp:HiddenField ID="hfSelectedPlanID" runat="server" />
        <asp:HiddenField ID="hfSelectedDeliveryDayID" runat="server" />
    </div>

    <script type="text/javascript">
        function showStep(stepNumber) {
            document.querySelectorAll('.step-container').forEach(el => el.classList.remove('active'));
            document.getElementById('step' + stepNumber).classList.add('active');
            window.scrollTo(0, 0);
        }

        var prm = Sys.WebForms.PageRequestManager.getInstance();
        prm.add_beginRequest(() => document.querySelector('.form-section')?.classList.add('ajax-loading'));
        prm.add_endRequest(() => document.querySelector('.form-section')?.classList.remove('ajax-loading'));
    </script>
</asp:Content>