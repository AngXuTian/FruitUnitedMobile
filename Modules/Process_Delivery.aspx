<%@ Page Title="SISTA - Process Delivery" Language="C#" MasterPageFile="~/ModulesPage.Master"
    AutoEventWireup="true" CodeBehind="Process_Delivery.aspx.cs"
    Inherits="FruitUnitedMobile.Modules.Process_Delivery" %>
<%@ Register Src="~/Component/Toast.ascx" TagPrefix="uc" TagName="Toast" %>
<%@ Register Src="~/Component/SearchBar.ascx" TagPrefix="uc" TagName="SearchBar" %>
<%@ Register Src="~/Component/ProductImage.ascx" TagPrefix="uc" TagName="ProductImage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="<%= ResolveUrl("~/CSS/ButtonFooter.css") %>" rel="stylesheet" />
    <link href="<%= ResolveUrl("~/CSS/SearchFix.css") %>" rel="stylesheet" />
    <meta name="viewport" content="width=device-width, initial-scale=1, maximum-scale=1, user-scalable=no" />
   <style>
    .container { 
        max-width: 720px; 
        margin: 0 auto; 
        padding: 16px 12px; 
        width: 100%; 
        padding-bottom: 160px !important;
    }
    h2 { font-weight: 700; font-size: 1.8rem; margin-bottom: 1.5rem; text-align: center; color: #222; }
    h5 { font-weight: 600; font-size: 1.2rem; margin-bottom: 1rem; color: #333; text-align: center; }

    .product-card {
        display: flex; 
        flex-wrap: wrap;
        align-items: center; 
        justify-content: space-between;
        background: #fff; border: 1px solid #ddd; border-radius: 10px;
        padding: 12px 48px; 
        margin-bottom: 16px;
        box-shadow: 0 1px 6px rgba(0,0,0,0.07); 
        position: relative; 
        gap: 10px;
        transition: all 0.3s ease;
    }
    .product-image { flex-shrink: 0; width: 80px; height: 80px; object-fit: contain; border-radius: 8px; border: 1px solid #ccc; background: #fefefe; }
    .product-abbreviation { flex: 1; font-weight: 700; font-size: 1.25rem; color: #222; white-space: nowrap; overflow: hidden; text-overflow: ellipsis; }
    .qty-control { display: flex; align-items: center; gap: 8px; }
    .qty-btn { width: 36px; height: 36px; font-size: 1.5rem; font-weight: 700; border-radius: 6px; border: 1px solid #ccc; background: #f8f9fa; cursor: pointer; display: flex; justify-content: center; align-items: center; }
    .qty-btn:active { background: #e2e6ea; transform: scale(0.95); }
    .qty-input { width: 60px; height: 36px; border-radius: 6px; border: 1px solid #ccc; font-size: 1.1rem; font-weight: 700; text-align: center; }
    .remove-btn {
        position: absolute; top: 8px; right: 8px; width: 28px; height: 28px;
        border-radius: 50%; background: #dc3545; border: none; color: #fff;
        cursor: pointer; display: flex; justify-content: center;
        align-items: center; transition: background 0.3s ease; z-index: 1;
        font-size: 1rem;
    }
    .remove-btn:hover { background: #c82333; }

    .empty-state { text-align: center; padding: 48px 24px; color: #6c757d; font-size: 1.1rem; }
    .empty-state i { font-size: 3.5rem; margin-bottom: 18px; opacity: 0.5; }

    /* Stock insufficient highlighting */
    .product-card.stock-insufficient {
        border: 2px solid #dc3545 !important;
        background: #fff5f5 !important;
        box-shadow: 0 2px 8px rgba(220, 53, 69, 0.2) !important;
    }
    .stock-warning {
        width: 100%;
        margin-top: 15px;
        background: #dc3545;
        color: white;
        padding: 10px;
        border-radius: 6px;
        font-size: 0.9rem;
        font-weight: 600;
        text-align: center;
        box-shadow: inset 0 0 5px rgba(0,0,0,0.1);
    }

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
        box-shadow: 0 4px 10px rgba(0,123,255,0.3);
        transition: all 0.2s;
        display: inline-flex;
        align-items: center;
        justify-content: center;
        gap: 8px;
    }
    .fixed-bottom-footer {
        position: relative !important;
        border-top: 1px solid #dee2e6 !important;
        box-shadow: none !important;
        pointer-events: auto;
        padding: 4px 8px !important;
        padding-bottom: calc(12px + env(safe-area-inset-bottom)) !important;
    }

    @media (max-width: 480px) {
        .container { padding: 12px 8px; }
        h2 { font-size: 1.5rem; }
        #<%= pnlProducts.ClientID %> .product-card { display: grid; grid-template-columns: auto 1fr auto; gap: 10px; padding: 12px; padding-top: 40px; }
        #<%= pnlProducts.ClientID %> .product-image { width: 60px; height: 60px; }
        .stock-warning { grid-column: 1 / span 3; margin-top: 10px; }
    }
    @media (max-width: 360px) {
        #<%= pnlProducts.ClientID %> .product-image { width: 55px; height: 55px; }
    }
    @media (min-width: 481px) and (max-width: 768px) {
        .container { padding: 16px 20px; }
        .product-image { width: 90px; height: 90px; }
    }

    #addProductModal .modal-dialog { margin: 0 auto; }
    @media (max-width: 576px) {
        #addProductModal .modal-dialog { margin: 0; max-width: 100vw; height: 100vh; }
        #addProductModal .modal-content { height: 100vh; border-radius: 0; border: none; display: flex; flex-direction: column; }
        #addProductModal .modal-header { position: sticky; top: 0; background: #fff; z-index: 10; border-bottom: 1px solid #dee2e6; flex-shrink: 0; padding: 16px 16px 12px; }
        #addProductModal .modal-title { font-size: 1.5rem; font-weight: 700; margin-bottom: 12px; }
        #addProductModal .modal-footer { position: sticky; bottom: 0; background: #fff; border-top: 1px solid #dee2e6; padding: 16px 20px calc(20px + env(safe-area-inset-bottom)); flex-shrink: 0; z-index: 10; }
        #addProductModal .modal-body { flex: 1; overflow-y: auto; -webkit-overflow-scrolling: touch; padding: 16px 16px 20px; }
    }
</style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <uc:Toast ID="Toast1" runat="server" />
    <div class="container">

        <h2>Process Delivery</h2>
        <h5>Delivery Items (<asp:Label ID="lblItemCount" runat="server" Text="0"></asp:Label>)</h5>

        <uc:SearchBar ID="SearchBar1" runat="server" 
            Placeholder="Search product by abbreviation..." 
            SearchInfoText="Type to filter products"
            SearchFields="data-abbreviation" />

        <asp:Panel ID="pnlProducts" runat="server">
            <asp:Repeater ID="rptDeliveryItems" runat="server">
                <ItemTemplate>
                    <div class="product-card" 
                         data-productid='<%# Eval("Product_Profile_ID") %>' 
                         data-abbreviation='<%# Eval("Abbreviation")?.ToString().ToLower() %>' 
                         data-searchable="true">

                        <!-- Client-side remove button (no postback) -->
                        <button type="button" 
                                class="remove-btn" 
                                onclick="removeProductCard('<%# Eval("Product_Profile_ID") %>')">
                            X
                        </button>

                        <uc:ProductImage runat="server"
                            ProductId='<%# Eval("Product_Profile_ID") %>'
                            FileName='<%# Eval("Filename") %>'
                            Abbreviation='<%# Eval("Abbreviation") %>'
                            ImageSize="60" />

                        <div class="product-abbreviation"><%# Eval("Abbreviation") %></div>

                        <div class="qty-control">
                            <button type="button"
                                    class="qty-btn"
                                    onclick="changeQty('<%# Eval("Product_Profile_ID") %>', -1)">−</button>

                            <input type="number"
                                   class="qty-input"
                                   id="qty_<%# Eval("Product_Profile_ID") %>"
                                   value='<%# Eval("Quantity") %>'
                                   min="0"
                                   oninput="validateQtyInput(this)" />

                            <button type="button"
                                    class="qty-btn"
                                    onclick="changeQty('<%# Eval("Product_Profile_ID") %>', 1)">+</button>
                        </div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </asp:Panel>

        <asp:Panel ID="pnlEmpty" runat="server" Visible="false" CssClass="empty-state">
            <i class="fa fa-inbox"></i>
            <p>No products in delivery list</p>
        </asp:Panel>

        <asp:HiddenField ID="hfProductData" runat="server" />
        <asp:HiddenField ID="hfStockIssues" runat="server" />
    </div>


        <div class="button-footer d-flex justify-content-between px-3 py-2 bg-white">
            <asp:Button ID="btnBack" runat="server" Text="Back"
                CssClass="btn btn-secondary flex-fill me-2" OnClick="btnBack_Click" />
            <button type="button" class="btn-add-product-sticky" onclick="showAddProductModal()">
                Add Product
            </button>
            <asp:Button ID="btnNext" runat="server" Text="Next"
                CssClass="btn btn-primary flex-fill ms-2" 
                OnClick="btnNext_Click" OnClientClick="return saveProductData();" />
        </div>


    <!-- Add Product Modal (unchanged) -->
    <div class="modal fade" id="addProductModal" tabindex="-1" aria-labelledby="addProductModalLabel" aria-hidden="true">
        <div class="modal-dialog modal-lg modal-dialog-scrollable">
            <div class="modal-content">
                <div class="modal-header flex-column align-items-stretch pb-2">
                    <h5 class="modal-title text-center mb-3" id="addProductModalLabel"><strong>Add Product</strong></h5>
                    <div class="px-3 w-100">
                        <uc:SearchBar ID="SearchBar2" runat="server"
                            Placeholder="Search product by abbreviation..."
                            SearchInfoText="Type to filter products"
                            SearchFields="data-abbreviation" />
                    </div>
                </div>

                <div class="modal-body pt-3">
                    <p class="text-muted small text-center mb-3">Select products from your available inventory</p>
                    <div data-results-container="true">
                        <asp:Repeater ID="rpAvailableProducts" runat="server" OnItemCommand="rpAvailableProducts_ItemCommand">
                            <ItemTemplate>
                                <div class="product-card mb-3" data-modal-productid='<%# Eval("Product_Profile_ID") %>'
                                     data-abbreviation='<%# Eval("Abbreviation")?.ToString().ToLower() %>' data-searchable="true">
                                    <uc:ProductImage runat="server"
                                        ProductId='<%# Eval("Product_Profile_ID") %>'
                                        FileName='<%# Eval("Filename") %>'
                                        Abbreviation='<%# Eval("Abbreviation") %>'
                                        ImageSize="60" />
                                    <div class="product-abbreviation"><%# Eval("Abbreviation") %></div>
                                    <asp:Button ID="btnAddProduct" runat="server" Text="+" 
                                        CssClass="btn btn-primary add-product-btn"
                                        CommandName="AddProduct"
                                        CommandArgument='<%# Eval("Product_Profile_ID") %>' />
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>
                    </div>
                </div>

                <div class="modal-footer justify-content-center border-0 pt-3">
                    <button type="button" class="btn btn-secondary px-5" data-bs-dismiss="modal">Close</button>
                </div>
            </div>
        </div>
    </div>

    <script type="text/javascript">
        function highlightInsufficientStock() {
            var cards = document.querySelectorAll('.product-card');
            cards.forEach(function (card) {
                card.classList.remove('stock-insufficient');
                var warning = card.querySelector('.stock-warning');
                if (warning) warning.remove();
            });

            var hfStockIssues = document.getElementById('<%= hfStockIssues.ClientID %>');
            if (!hfStockIssues || !hfStockIssues.value) return;

            try {
                var stockIssues = JSON.parse(hfStockIssues.value);
                var pnl = document.getElementById('<%= pnlProducts.ClientID %>');

                stockIssues.forEach(function (issue) {
                    var card = pnl.querySelector('[data-productid="' + issue.ProductId + '"]');
                    if (card) {
                        card.classList.add('stock-insufficient');

                        var warning = document.createElement('div');
                        warning.className = 'stock-warning';
                        warning.textContent = 'Entered: ' + issue.Requested + ', Loaded: ' + issue.Available + ', Shortage: ' + (issue.Requested - issue.Available);
                        card.appendChild(warning);

                        if (stockIssues[0].ProductId === issue.ProductId) {
                            card.scrollIntoView({ behavior: 'smooth', block: 'center' });
                        }
                    }
                });
            } catch (e) {
                console.error('Error highlighting stock issues:', e);
            }
        }

        function removeProductCard(productId) {
            if (confirm('Remove this product from delivery?')) {
                var pnl = document.getElementById('<%= pnlProducts.ClientID %>');
                var card = pnl.querySelector('[data-productid="' + productId + '"]');
                if (card) {
                    card.remove();
                    updateHiddenField();
                    updateItemCount();
                    highlightInsufficientStock();

                    var remaining = pnl.querySelectorAll('.product-card');
                    if (remaining.length === 0) {
                        document.getElementById('<%= pnlEmpty.ClientID %>').style.display = 'block';
                        pnl.style.display = 'none';
                    }
                }
            }
        }

        function changeQty(productId, change) {
            var input = document.getElementById('qty_' + productId);
            var currentQty = parseInt(input.value) || 0;
            var newQty = Math.max(0, currentQty + change);
            input.value = newQty;
            updateHiddenField();
            updateItemCount();
        }

        function validateQtyInput(input) {
            if (input.value === '' || isNaN(input.value) || input.value < 0) {
                input.value = 0;
            }
            updateHiddenField();
            updateItemCount();
        }

        function updateItemCount() {
            var count = 0;
            var pnl = document.getElementById('<%= pnlProducts.ClientID %>');
            if (pnl) {
                pnl.querySelectorAll('.qty-input').forEach(function (input) {
                    if (parseInt(input.value) > 0) count++;
                });
            }
            var label = document.querySelector('[id$="lblItemCount"]');
            if (label) label.textContent = count;
        }

        function updateHiddenField() {
            var products = [];
            var pnl = document.getElementById('<%= pnlProducts.ClientID %>');
            if (pnl) {
                pnl.querySelectorAll('.qty-input').forEach(function (input) {
                    var prodId = input.id.replace('qty_', '');
                    var qty = parseInt(input.value) || 0;
                    if (qty > 0) {
                        products.push({ productId: prodId, qty: qty });
                    }
                });
            }
            var hf = document.getElementById('<%= hfProductData.ClientID %>');
            if (hf) hf.value = JSON.stringify(products);
        }

        function showAddProductModal() {
            var modal = new bootstrap.Modal(document.getElementById('addProductModal'), { backdrop: false });
            modal.show();
        }

        function saveProductData() {
            updateHiddenField();
            var data = JSON.parse(document.getElementById('<%= hfProductData.ClientID %>').value || '[]');
            if (data.length === 0) {
                alert('Please add at least one product with quantity > 0');
                return false;
            }
            return true;
        }

        // Run on every page load/refresh/postback
        window.addEventListener('load', function () {
            updateHiddenField();
            updateItemCount();
            highlightInsufficientStock();
        });

        document.addEventListener('DOMContentLoaded', function () {
            var modals = document.querySelectorAll('.modal');
            var header = document.getElementById('ModuleHeader');
            modals.forEach(function (modal) {
                modal.addEventListener('show.bs.modal', function () {
                    if (header) header.style.visibility = 'hidden';
                });
                modal.addEventListener('hidden.bs.modal', function () {
                    if (header) header.style.visibility = 'visible';
                });
            });
        });
    </script>
</asp:Content>