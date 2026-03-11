<%@ Page Title="SISTA - Process Return" Language="C#" MasterPageFile="~/ModulesPage.Master"
    AutoEventWireup="true" CodeBehind="Process_Return.aspx.cs"
    Inherits="FruitUnitedMobile.Modules.Process_Return" %>
<%@ Register Src="~/Component/Toast.ascx" TagPrefix="uc" TagName="Toast" %>
<%@ Register Src="~/Component/SearchBar.ascx" TagPrefix="uc" TagName="SearchBar" %>
<%@ Register Src="~/Component/ProductImage.ascx" TagPrefix="uc" TagName="ProductImage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="<%= ResolveUrl("~/CSS/ButtonFooter.css") %>" rel="stylesheet" />
    <style>
        .container {
            max-width: 720px;
            margin: 0 auto;
            padding: 16px 12px;
            width: 100%;
            padding-bottom: 160px !important;
        }
        h2 { font-weight: 700; font-size: 1.8rem; margin-bottom: 1.5rem; text-align: center; color: #222; }

        /* Return Item Card */
        .return-item {
            border: 2px solid #007bff;
            background: #fff;
            border-radius: 12px;
            padding: 14px;
            margin-bottom: 16px;
            box-shadow: 0 2px 8px rgba(0,0,0,0.1);
        }
        .return-item-header {
            display: flex;
            justify-content: space-between;
            align-items: center;
            margin-bottom: 12px;
            padding-bottom: 10px;
            border-bottom: 1px solid #007bff;
        }
        .return-item-product {
            display: flex;
            align-items: center;
            gap: 12px;
            font-weight: 700;
            font-size: 1.1rem;
        }
        .return-item-product img {
            width: 50px;
            height: 50px;
            object-fit: contain;
            border-radius: 8px;
            border: 1px solid #ddd;
        }
        .btn-remove-return {
            background: #dc3545;
            color: white;
            border: none;
            border-radius: 50%;
            width: 32px;
            height: 32px;
            cursor: pointer;
            display: flex;
            align-items: center;
            justify-content: center;
        }

        /* REASON */
        .return-detail-reason {
            flex: 1;
            min-width: 140px;
        }
        .return-detail-label {
            display: block;
            margin-bottom: 6px;
            font-weight: 600;
            color: #555;
            font-size: 0.9rem;
        }
        .list-reason-ddl {
            width: 100%;
            padding: 8px;
            border: 1px solid #ced4da;
            border-radius: 6px;
        }

        /* + / - BUTTONS — EXACTLY SAME AS DELIVERY (using your classes) */
        .qty-control {
            display: flex;
            align-items: center;
            gap: 8px;
        }
        .qty-btn {
    width: 36px;
    height: 36px;
    font-size: 1.5rem;
    font-weight: 700;
    border-radius: 6px;
    border: 1px solid #ccc;
    background: #f8f9fa;
    cursor: pointer;
    display: flex;
    justify-content: center;
    align-items: center;
    color: inherit !important;           /* ← THIS LINE KILLS THE BLUE */
    text-decoration: none !important;    /* ← Remove underline */
    transition: all 0.2s;
}
        .qty-btn:active {
            background: #e2e6ea;
            transform: scale(0.95);
        }
        .qty-input {
            width: 60px;
            height: 36px;
            border-radius: 6px;
            border: 1px solid #ccc;
            font-size: 1.1rem;
            font-weight: 700;
            text-align: center;
        }

        /* Layout */
        .return-detail-row-flex {
            display: flex;
            flex-wrap: wrap;
            gap: 16px;
            align-items: end;
        }

        /* Sticky Footer */
        .fixed-bottom-footer {
            position: fixed;
            bottom: 0; left: 0; right: 0;
            z-index: 1030;
            pointer-events: none;
            display: flex;
            flex-direction: column;
            background: #fff;
            box-shadow: 0 -4px 16px rgba(0,0,0,0.08);
        }
        .add-product-footer {
            padding: 8px 16px 4px;
            text-align: center;
            pointer-events: auto;
        }
        .btn-add-product-sticky {
            background: #007bff;
            color: white;
            border: none;
            padding: 6px 8px;
            font-size: clamp(0.85rem, 3.5vw, 1.1rem);
            font-weight: 700;
            border-radius: 12px;
            cursor: pointer;
            width: 88%;
            max-width: 360px;
            box-shadow: 0 4px 12px rgba(0,123,255,0.3);
            transition: all 0.2s;
            display: inline-flex;
            align-items: center;
            justify-content: center;
            gap: 10px;
        }
        .btn-add-product-sticky i { font-size: 1.3rem; }
        .btn-add-product-sticky:active { transform: scale(0.97); }
        .fixed-bottom-footer  {
            position: relative !important;
            border-top: 1px solid #dee2e6 !important;
            box-shadow: none !important;
            pointer-events: auto;
            padding: 10px 16px !important;
            padding-bottom: calc(12px + env(safe-area-inset-bottom)) !important;
        }

        /* FULLSCREEN MODAL ON MOBILE */
        #returnModal .modal-dialog { margin: 0 auto; }
        @media (max-width: 576px) {
            #returnModal .modal-dialog {
                margin: 0;
                max-width: 100vw;
                height: 100vh;
            }
            #returnModal .modal-content {
                height: 100vh;
                border-radius: 0;
                border: none;
                display: flex;
                flex-direction: column;
            }
            #returnModal .modal-header {
                position: sticky;
                top: 0;
                background: #fff;
                z-index: 10;
                border-bottom: 1px solid #dee2e6;
                flex-shrink: 0;
                padding: 16px;
            }
            #returnModal .modal-title {
                font-size: 1.5rem;
                font-weight: 700;
                text-align: center;
                width: 100%;
            }
            #returnModal .modal-body {
                flex: 1;
                overflow-y: auto;
                -webkit-overflow-scrolling: touch;
                padding: 20px 16px;
            }
            #returnModal .modal-footer {
                position: sticky;
                bottom: 0;
                background: #fff;
                border-top: 1px solid #dee2e6;
                padding: 16px 20px calc(20px + env(safe-area-inset-bottom));
                flex-shrink: 0;
                z-index: 10;
            }
            #returnModal .modal-footer .btn {
                width: 100%;
                max-width: 340px;
                margin: 0 auto;
                min-height: 56px;
                font-size: 1.1rem;
                border-radius: 12px;
                font-weight: 600;
            }
        }
        @media (min-width: 577px) {
            #returnModal .modal-header { padding: 20px 24px; }
            #returnModal .modal-title { font-size: 1.6rem; }
            #returnModal .modal-body { padding: 20px 24px; }
            #returnModal .modal-footer { padding: 16px 24px; }
        }

        /* Search Bar */
        .modal-search-container {
            position: sticky;
            top: 0;
            background: #fff;
            z-index: 9;
            padding: 12px 16px 8px;
            border-bottom: 1px solid #eee;
        }
        .modal-search-input {
            width: 100%;
            padding: 12px 16px;
            border: 2px solid #ddd;
            border-radius: 12px;
            font-size: 1rem;
            outline: none;
        }
        .modal-search-input:focus {
            border-color: #007bff;
            box-shadow: 0 0 0 3px rgba(0,123,255,0.1);
        }

        /* Product List in Modal */
.modal-body .product-select {
    display: grid;
    grid-template-columns: 70px 1fr;
    grid-template-areas: "img abbr";
    align-items: center;
    gap: 14px;
    padding: 14px 16px;
    border-radius: 12px;
    margin-bottom: 10px;
    background: #fff;
    cursor: pointer;
    transition: all 0.2s;
    border: 2px solid;
}
.modal-body .product-select:hover {
    background: #f0f8ff;
    border-color: #007bff;
}
.modal-body .product-select.selected {
    background: #d4edff;
    border-color: #007bff;
}

/* Image — fixed 60×60px, perfectly centered */
.modal-body .product-select img {
    grid-area: img;
    width: 60px !important;
    height: 60px !important;
    object-fit: contain;
    border-radius: 8px;
    background: #f9f9f9;
    padding: 6px;
    box-sizing: border-box;
    justify-self: center;
}

/* Text — takes remaining space */
.modal-body .product-abbreviation {
    grid-area: abbr;
    font-weight: 600;
    font-size: 1.12rem;
    color: #1a1a1a;
}

.input-grid {
    display: flex;
    /*flex-wrap: wrap;*/
    gap: 10px;
    justify-content: center;
    margin: 0 auto 20px auto;
    max-width: 560px;
    width: 100%;
}

.input-grid > div {
    flex: 1;
    min-width: 180px;
    max-width: 260px;
}

.input-group-label {
    display: block;
    font-weight: 600;
    color: #555;
    margin-bottom: 8px;
    font-size: 0.95rem;
}
        .error-message { color: #dc3545; font-size: 0.8rem; margin-top: 4px; display: none; }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <uc:Toast ID="Toast1" runat="server" />
    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>

    <div class="container">
        <h2>SISTA - Process Return</h2>
        <uc:SearchBar ID="SearchBar1" runat="server" 
            Placeholder="Search product by abbreviation..." 
            SearchInfoText="Type to filter products"
            SearchFields="data-abbreviation" />

        <asp:UpdatePanel ID="upReturnList" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <asp:Panel ID="pnlReturnItems" runat="server">
                    <asp:Repeater ID="rptReturnItems" runat="server" OnItemDataBound="rptReturnItems_ItemDataBound">
                        <ItemTemplate>
                            <div class="return-item" data-productid='<%# Eval("Product_Profile_ID") %>' data-abbreviation='<%# Eval("Abbreviation")?.ToString().ToLower() %>' data-searchable="true">
                                <asp:HiddenField ID="hfListProdID" runat="server" Value='<%# Eval("Product_Profile_ID") %>' />
                                <div class="return-item-header">
                                    <div class="return-item-product">
                                        <uc:ProductImage runat="server"
    ProductId='<%# Eval("Product_Profile_ID") %>'
    FileName='<%# Eval("Filename") %>'
    Abbreviation='<%# Eval("Abbreviation") %>'
    ImageSize="60" />
                                        <div class="product-abbreviation"><%# Eval("Abbreviation") %></div>
                                    </div>
                                    <asp:Button ID="btnRemoveReturn" runat="server" Text="X" CssClass="btn-remove-return"
                                        CommandArgument='<%# Eval("Product_Profile_ID") %>' OnClick="btnRemoveReturn_Click" />
                                </div>

                                <div class="return-detail-row-flex">
                                    <div class="return-detail-reason">
                                        <span class="return-detail-label">Reason:</span>
                                        <asp:DropDownList ID="ddlListReason" runat="server" CssClass="list-reason-ddl"
                                            AutoPostBack="true" OnSelectedIndexChanged="ddlListReason_SelectedIndexChanged">
                                        </asp:DropDownList>
                                    </div>

                                    <div class="return-detail-qty">
                                        <span class="return-detail-label">Qty:</span>
                                        <div class="qty-control">
                                            <asp:LinkButton ID="btnDec" runat="server" CssClass="qty-btn"
                                                CommandName="Decrease" CommandArgument='<%# Eval("Product_Profile_ID") %>'
                                                OnCommand="UpdateReturnItem_Command">−</asp:LinkButton>

                                            <asp:TextBox ID="txtListQty" runat="server" CssClass="qty-input"
                                                Text='<%# Eval("Return_Quantity") %>' AutoPostBack="true"
                                                OnTextChanged="txtListQty_TextChanged"></asp:TextBox>

                                            <asp:LinkButton ID="btnInc" runat="server" CssClass="qty-btn"
                                                CommandName="Increase" CommandArgument='<%# Eval("Product_Profile_ID") %>'
                                                OnCommand="UpdateReturnItem_Command">+</asp:LinkButton>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>

                    <asp:Label ID="lblNoReturns" runat="server" CssClass="text-muted text-center d-block py-5"
                        Text="No return items added yet." Visible="false"></asp:Label>
                </asp:Panel>
            </ContentTemplate>
        </asp:UpdatePanel>

        <asp:HiddenField ID="hfSelectedProductID" runat="server" />
    </div>

    <!-- MODAL WITH SEARCH -->
    <div class="modal fade" id="returnModal" tabindex="-1" aria-labelledby="returnModalLabel" aria-hidden="true">
        <div class="modal-dialog modal-lg modal-dialog-scrollable">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title" id="returnModalLabel">Add Return Product</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
                </div>

                <uc:SearchBar ID="SearchBar2" runat="server" 
            Placeholder="Search product by abbreviation..." 
            SearchInfoText="Type to filter products"
            SearchFields="data-abbreviation" />

                <div class="modal-body">

                    <asp:UpdatePanel ID="upModalProducts" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <asp:Repeater ID="rptAvailableProducts" runat="server">
                                <ItemTemplate>
    <div class="product-select"
         data-abbreviation='<%# Eval("Abbreviation")?.ToString().ToLower() %>'
         data-productid='<%# Eval("Product_Profile_ID") %>'
         data-searchable="true"
         onclick="selectReturnProduct(this, '<%# Eval("Product_Profile_ID") %>')">

        <!-- Fixed 60×60 image -->
        <uc:ProductImage runat="server"
            ProductId='<%# Eval("Product_Profile_ID") %>'
            FileName='<%# Eval("Filename") %>'
            Abbreviation='<%# Eval("Abbreviation") %>'
            ImageSize="60" />

        <!-- Clean text -->
        <div class="product-abbreviation"><%# Eval("Abbreviation") %></div>
    </div>
</ItemTemplate>
                            </asp:Repeater>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>

                <div class="modal-footer">
                    <div class="input-grid">
                        <div>
                            <label class="input-group-label">Quantity</label>
                            <div class="qty-control">
                                <button type="button" class="qty-btn" onclick="changeReturnQty(-1)">−</button>
                                <asp:TextBox ID="txtReturnQty" runat="server" CssClass="qty-input" Text="1" />
                                <button type="button" class="qty-btn" onclick="changeReturnQty(1)">+</button>
                            </div>
                            <span id="qtyErrorMsg" class="error-message">Minimum 1</span>
                        </div>
                        <div>
                            <label class="input-group-label">Reason</label>
                            <asp:DropDownList ID="ddlExchangeReason" runat="server" CssClass="form-select">
                                <asp:ListItem Value="">-- Select Reason --</asp:ListItem>
                            </asp:DropDownList>
                            <span id="reasonErrorMsg" class="error-message">Required</span>
                        </div>
                    </div>

                    <asp:Button ID="btnConfirmReturn" runat="server" Text="Add Return Item"
                        CssClass="btn btn-primary w-100 mt-3" style="height:56px;font-size:1.1rem;"
                        OnClick="btnConfirmReturn_Click" />
                </div>
            </div>
        </div>
    </div>

    <!-- STICKY FOOTER -->

        <div class="button-footer d-flex justify-content-between px-3 py-2 bg-white">
            <asp:Button ID="btnBack" runat="server" Text="Back" CssClass="btn btn-secondary flex-fill me-2" OnClick="btnBack_Click" />
            <button type="button" class="btn-add-product-sticky" onclick="showReturnProductModal()">
                <%--<i class="fa fa-plus-circle"></i>--%>
                Add Return
            </button>
            <asp:Button ID="btnNext" runat="server" Text="Next" CssClass="btn btn-primary flex-fill ms-2"
                OnClick="btnNext_Click" OnClientClick="return saveProductData();" />
        </div>

    <script>
        function showReturnProductModal() {
            var modal = new bootstrap.Modal(document.getElementById('returnModal'), {backdrop: false});
            modal.show();
            document.querySelector('.modal-search-input').value = '';
            filterReturnProducts('');
        }
        function selectReturnProduct(el, id) {
            document.querySelectorAll('.product-select').forEach(p => p.classList.remove('selected'));
            el.classList.add('selected');
            document.getElementById('<%= hfSelectedProductID.ClientID %>').value = id;
            hideReturnModalErrors();
        }
        function changeReturnQty(delta) {
            var input = document.getElementById('<%= txtReturnQty.ClientID %>');
            var val = parseInt(input.value) || 1;
            val = Math.max(1, val + delta);
            input.value = val;
            hideReturnModalErrors();
        }
        function hideReturnModalErrors() {
            document.getElementById('qtyErrorMsg').style.display = 'none';
            document.getElementById('reasonErrorMsg').style.display = 'none';
        }
        function filterReturnProducts(text) {
            const items = document.querySelectorAll('#returnModal .product-select');
            items.forEach(item => {
                const abbr = item.getAttribute('data-abbreviation') || '';
                item.style.display = abbr.includes(text.toLowerCase()) ? 'flex' : 'none';
            });
        }
        document.addEventListener('DOMContentLoaded', () => {
            var header = document.getElementById('ModuleHeader');
            var modal = document.getElementById('returnModal');
            modal.addEventListener('show.bs.modal', () => header && (header.style.visibility = 'hidden'));
            modal.addEventListener('hidden.bs.modal', () => header && (header.style.visibility = 'visible'));
        });
    </script>
</asp:Content>