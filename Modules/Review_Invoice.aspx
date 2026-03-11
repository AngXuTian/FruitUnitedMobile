<%@ Page Title="SISTA - Review Invoice" Language="C#" MasterPageFile="~/ModulesPage.Master"
    AutoEventWireup="true" CodeBehind="Review_Invoice.aspx.cs"
    Inherits="FruitUnitedMobile.Modules.Review_Invoice" %>
<%@ Register Src="~/Component/Toast.ascx" TagPrefix="uc" TagName="Toast" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="<%= ResolveUrl("~/CSS/ButtonFooter.css") %>" rel="stylesheet" />
    <style>
        body {
            background: #f8f9fa;
            font-size: 14px;
        }
        .card {
            box-shadow: 0 2px 10px rgba(0,0,0,0.1);
            border-radius: 8px;
            margin-bottom: 15px;
            border: none;
        }
        .card-header {
            padding: 12px 15px;
        }
        .card-header h3 {
            font-size: 1.25rem;
            margin: 0;
        }
        .card-body {
            padding: 15px;
        }
        .alert-info {
            font-size: 0.9rem;
            padding: 12px;
            margin-bottom: 15px;
        }
        .alert-info strong {
            display: inline-block;
            min-width: 100px;
        }
        h4 {
            font-size: 1.1rem;
            margin-top: 20px;
            margin-bottom: 12px;
            color: #1e40af;
        }
        .table-responsive {
            margin-bottom: 15px;
            overflow-x: auto;
            -webkit-overflow-scrolling: touch;
        }
        table {
            font-size: 0.85rem;
            margin-bottom: 0;
        }
        th {
            background: #1e40af;
            color: white;
            padding: 8px 6px;
            white-space: nowrap;
            font-weight: 600;
        }
        td {
            padding: 8px 6px;
            vertical-align: middle;
        }
        .total-row {
            font-weight: bold;
            background: #dbeafe !important;
        }
        .grand-total {
            background: #1e40af !important;
            color: white !important;
            font-size: 1rem;
        }
        .grand-total td {
            padding: 12px 8px;
        }
        .badge-pending {
            background: #dc2626;
            color: white;
            padding: 4px 8px;
            border-radius: 4px;
            font-size: 0.8rem;
        }
        .table-light { background-color: #f8f9fa; }
        .table-primary thead { background: #60a5fa !important; }
        .table-danger thead { background: #f87171 !important; }
        .table-dark thead { background: #374151 !important; }
        td:nth-child(2), td:nth-child(4), td:nth-child(5),
        th:nth-child(2), th:nth-child(4), th:nth-child(5) {
            text-align: right;
        }

        #qrModal .modal-header {
    padding-top: 2.5rem !important;     /* 40px - feel free to increase to 3rem or 3.5rem */
    padding-bottom: 1rem !important;
}

        @media (max-width: 576px) {
    #qrModal .modal-header {
        padding-top: 2.5rem !important;   /* even more on mobile if needed */
        padding-bottom: 1.25rem !important;
    }
}
        @media (max-width: 576px) {
            td:first-child {
                max-width: 120px;
                word-wrap: break-word;
            }
        }

        /* Add this to match mobile full-screen behavior for QR modal */
        @media (max-width: 576px) {
            #qrModal .modal-dialog { margin: 0; max-width: 100vw; height: 100vh; }
            #qrModal .modal-content { height: 100vh; border-radius: 0; border: none; display: flex; flex-direction: column; }
            #qrModal .modal-header { position: sticky; top: 0; background: #fff; z-index: 10; border-bottom: 1px solid #dee2e6; flex-shrink: 0; padding: 16px 16px 12px; }
            #qrModal .modal-title {font-size: 1.5rem;font-weight: 700;color: #000; text-align: center; width: 100%;}
            #qrModal .modal-footer { position: sticky; bottom: 0; background: #fff; border-top: 1px solid #dee2e6; padding: 16px 20px calc(20px + env(safe-area-inset-bottom)); flex-shrink: 0; z-index: 10; }
            #qrModal .modal-body { flex: 1; overflow-y: auto; -webkit-overflow-scrolling: touch; padding: 16px 16px 20px; }
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <uc:Toast ID="Toast1" runat="server" />
    <asp:ScriptManager ID="ScriptManager1" runat="server" />
    <asp:HiddenField ID="hdnProposedInvoiceNo" runat="server" />

    <!-- Main Invoice Card -->
    <div class="card">
        <div class="card-header bg-primary text-white text-center">
            <h3>Review Invoice</h3>
        </div>
        <div class="card-body">

            <!-- Header Info -->
            <div class="alert alert-info">
                <div><strong>Outlet:</strong> <%= OutletDisplayName %></div>
                <div><strong>Delivery Date:</strong> <%= DeliveryDateDisplay %></div>
                <div class="mt-2">
                    <strong>Invoice No:</strong>
                    <asp:Label ID="lblInvoiceNo" runat="server" CssClass="badge bg-success text-white px-3 py-2 fs-6">
                        Generating...
                    </asp:Label>
                </div>
            </div>

            <!-- Chargeable Items -->
            <h4>Chargeable Items</h4>
            <div class="table-responsive">
                <table class="table table-bordered table-hover">
                    <thead>
                        <tr>
                            <th>Product</th>
                            <th>Qty</th>
                            <th>UOM</th>
                            <th>Price</th>
                            <th>Total</th>
                        </tr>
                    </thead>
                    <tbody><%= ChargeableItemsHtml %></tbody>
                    <tfoot>
                        <tr class="total-row">
                            <td colspan="4" class="text-end">Sub Total:</td>
                            <td>$<%= SubTotal.ToString("N2") %></td>
                        </tr>
                        <tr class="total-row">
                            <td colspan="4" class="text-end">Tax (8%):</td>
                            <td>$<%= TaxAmount.ToString("N2") %></td>
                        </tr>
                        <tr class="grand-total">
                            <td colspan="4" class="text-end"><strong>Grand Total:</strong></td>
                            <td><strong>$<%= GrandTotal.ToString("N2") %></strong></td>
                        </tr>
                    </tfoot>
                </table>
            </div>

            <!-- Exchange Items -->
            <h4>Exchange Items</h4>
            <div class="table-responsive">
                <table class="table table-bordered table-light">
                    <thead class="table-primary">
                        <tr><th>Product</th><th>Qty</th><th>UOM</th><th>Reason</th></tr>
                    </thead>
                    <tbody><%= ExchangeItemsHtml %></tbody>
                </table>
            </div>

            <!-- Outstanding Items -->
            <h4>Outstanding Items</h4>
            <div class="table-responsive">
                <table class="table table-bordered table-light">
                    <thead class="table-danger">
                        <tr><th>Product</th><th>Qty</th><th>UOM</th><th>Reason</th></tr>
                    </thead>
                    <tbody><%= OutstandingItemsHtml %></tbody>
                </table>
            </div>

            <!-- Total Delivered -->
            <h4>Total Delivered (For Reference)</h4>
            <div class="table-responsive">
                <table class="table table-bordered table-warning">
                    <thead class="table-dark">
                        <tr><th>Product</th><th>Qty</th><th>UOM</th></tr>
                    </thead>
                    <tbody><%= DeliveredItemsHtml %></tbody>
                </table>
            </div>

            <!-- BUTTONS -->
            <div class="button-footer force-single-row">
                <asp:Button ID="btnBack" runat="server"
                    Text="Back"
                    CssClass="btn btn-secondary"
                    OnClick="btnBack_Click" />

                <asp:Button ID="btnQrCode" runat="server"
                    Text="QR Code"
                    CssClass="btn btn-primary"
                    OnClick="btnQRCode_Click" />

                <asp:Button ID="btnIssue" runat="server"
                    Text="Issue Invoice"
                    CssClass="btn btn-primary"
                    OnClick="btnIssue_Click" />
            </div>

            <!-- QR Code Modal -->
<div class="modal fade" id="qrModal" tabindex="-1" aria-labelledby="qrModalLabel" aria-hidden="true">
    <div class="modal-dialog modal-dialog-centered modal-lg">
        <div class="modal-content border-0 shadow-lg rounded-4 overflow-hidden bg-white">
            
            <!-- Header - Bigger, cleaner, better aligned -->
            <div class="modal-header border-0 bg-light d-flex justify-content-center align-items-center py-4 px-4 px-md-5">
                <h3 class="modal-title fw-bold text-dark mb-0" id="qrModalLabel" style="font-size: 1.85rem; letter-spacing: -0.5px;">
                    Scan to Process Invoice
                </h3>
                <button type="button" class="btn-close position-absolute end-0 me-4" 
                        data-bs-dismiss="modal" aria-label="Close"></button>
            </div>

            <!-- Body - maximum focus on QR -->
            <div class="modal-body text-center py-5 px-4 px-md-5 bg-white">
                <pre>
<%: Session["QR_Code"] == null 
        ? "QR data not available in session." 
        : string.Join("\n\n", (List<string>)Session["QR_Code"]) %>
</pre>

                <!-- Optional instruction -->
                <p class="text-muted mb-5 fs-5 fw-light">
                    Use your scanner app or camera to scan this QR code
                </p>

                <!-- QR container - generous quiet zone -->
                <div class="qr-container mx-auto p-5 bg-white rounded-3 shadow-sm border border-2 border-light-subtle" 
                     style="max-width: 480px;">
                    <asp:Image ID="imgQR" runat="server" CssClass="img-fluid" 
                               Style="width: 100%; max-width: 420px; height: auto;" 
                               AlternateText="Invoice QR Code" Visible="false" />
                </div>

                <!-- Page navigation -->
                <div id="divPageInfo" runat="server" visible="false" class="mt-5 pt-3">
                    <p class="fs-5 fw-semibold mb-3 text-secondary">
                        Page <asp:Label ID="lblPageInfo" runat="server" Text="1 / 1" />
                    </p>
                    <div class="btn-group mx-auto">
                        <asp:Button ID="btnPrev" runat="server" Text="Previous" 
                                    CssClass="btn btn-outline-secondary btn-lg px-5 py-3" OnClick="btnPrev_Click" />
                        <asp:Button ID="btnNext" runat="server" Text="Next" 
                                    CssClass="btn btn-outline-secondary btn-lg px-5 py-3" OnClick="btnNext_Click" />
                    </div>
                </div>
            </div>

            <!-- Footer -->
            <div class="modal-footer border-0 justify-content-center py-4 bg-light">
                <button type="button" class="btn btn-secondary px-5 py-3 fw-medium" data-bs-dismiss="modal">
                    Close
                </button>
            </div>
        </div>
    </div>
</div>

        </div>
    </div>

    <!-- JavaScript for hiding/showing header -->
    <script type="text/javascript">
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