<%@ Page Title="SISTA - Print Invoice" Language="C#" MasterPageFile="~/ModulesPage.Master" AutoEventWireup="true" CodeBehind="Print_Invoice.aspx.cs" Inherits="FruitUnitedMobile.Modules.Print_Invoice" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <!-- 1. html2canvas first (required dependency) -->
    <script src="https://cdnjs.cloudflare.com/ajax/libs/html2canvas/1.4.1/html2canvas.min.js"></script>
    
    <!-- 2. html2pdf after html2canvas -->
    <script src="https://cdnjs.cloudflare.com/ajax/libs/html2pdf.js/0.10.1/html2pdf.bundle.min.js"></script>
    <link href="<%= ResolveUrl("~/CSS/ButtonFooter.css") %>" rel="stylesheet" />

    <style>
    /* ────────────────────────────────────────────────
       CENTRAL CONTROLS – CHANGE THESE ONLY!
    ───────────────────────────────────────────────── */
    .invoice-box {
        /* Font family – change here to apply everywhere */
        /*--ff-main: "Times New Roman", Times, serif;*/
        --ff-main: Arial, sans-serif;
        /* --ff-main: "Courier New", Courier, monospace; */  /* ← uncomment if you want monospace back */

        /* Font sizes */
        --fs-base:      8px;      /* items, descriptions, qty, prices, amounts, SKU, totals, good-condition, footer, signature text, etc. */
        --fs-small:     8px;    /* separator lines, very small notes */
        --fs-company:   8px;      /* company address, phone, registration no., details */
        --fs-header:    8px;    /* invoice header (no/date/outlet), section titles */
        --fs-name:      8px;    /* company name – usually largest */

        /* Line heights */
        --lh-base:      1.25;
        --lh-tight:     1.15;
    }

    /* Apply main font family + basic reset */
    body,
    .invoice-box,
    .invoice-box * {
        font-family: var(--ff-main) !important;
    }

    body { 
        background-color: #fff !important; 
        margin: 0; 
        padding: 0;
    }

    .invoice-box {
        width: 48mm;
        max-width: 48mm;
        margin: 0 auto;
        padding: 1.2mm 1mm;
        color: #000;
        background: #fff;
        font-size: var(--fs-base);
        line-height: var(--lh-base);
        box-sizing: border-box;
    }

    .logo-container { text-align: center; margin-bottom: 5px; }
    .logo { max-width: 70px; height: 70px; }

    .company-info { text-align: center; margin-bottom: 6px; font-size: var(--fs-company); }
    .comp-name { font-size: var(--fs-name); display: block; }
    .comp-details { font-size: var(--fs-company); display: block; }

    .invoice-header { text-align: center; margin: 6px 0 4px 0; font-size: var(--fs-header); }
    .section-title { margin: 6px 0 2px 0; font-size: var(--fs-header); }

    .receipt-table {
        width: 100%;
        border-collapse: collapse;
        font-size: var(--fs-base);
        margin: 2px 0;
    }

    .receipt-table td {
        padding: 0.5px 1.5px;
        text-align: left;
        white-space: nowrap;
        font-size: var(--fs-base);
    }

    .receipt-table thead { display: none; }

    .receipt-table .amt,
    .receipt-table td.amt {
        text-align: right !important;
    }

    .qty  { text-align: center  !important; }
.exc  { text-align: center  !important; }
.up   { text-align: right   !important; }
.amt  { text-align: right   !important; }

    .chargeable-header-row {
        font-size: var(--fs-base);
    }

    .chargeable-header-row td {
        padding: 1px 2px;
        text-align: right;
    }

    .chargeable-header-row .header-desc {
        text-align: left;
        padding-left: 3px;
    }

    /* Added approximate column widths for better alignment (adjust as needed based on content) */
    .chargeable-header-row .header-desc { width: 25%; }
    .chargeable-header-row .sku { width: 20%; }
    .chargeable-header-row .qty { width: 10%; }
    .chargeable-header-row .exc { width: 10%; }
    .chargeable-header-row .up { width: 15%; }
    .chargeable-header-row .amt { width: 20%; text-align: right; }

    .item-desc {
        font-size: var(--fs-base);
        white-space: normal;
        word-break: break-word;
        padding: 1.5px 3px 0.5px 3px;
        line-height: var(--lh-tight);
    }

    .item-details td {
        padding: 0 1.5px 0.5px 1.5px;
        font-size: var(--fs-base);
    }

    .item-details .sku {
        font-size: var(--fs-base);
        color: #000 !important;
        padding-left: 7px;
        text-align: left;
    }

    .item-details .qty,
    .item-details .exc,
    .item-details .up,
    .item-details .amt,
    .item-details .reason {
        text-align: right;
    }

    .item-details .reason {
        text-align: left;
        padding-left: 3px;
    }

    .total-row td { 
        padding: 2px 1.5px; 
        font-size: var(--fs-base);
    }

    .total-row .label {
    padding-right: 4px !important;     /* was probably 6–8px → make smaller */
    text-align: right !important;
}

.total-row .currency {
    padding-left:  2px !important;     /* reduce left space */
    padding-right: 2px !important;     /* reduce right space */
    text-align: right !important;      /* or center if you prefer */
}

.total-row .value,
.total-row .amt {
    padding-left:  1px !important;     /* almost no space on left */
    padding-right: 2px !important;
    text-align: right !important;
}

    .separator { text-align: center; margin: 5px 0; font-size: var(--fs-small); letter-spacing: 1px; }
    .good-condition { text-align: left; font-size: var(--fs-base); margin: 6px 0 2px 0; }

    .signature-spacer {
        height: 80px;
        min-height: 80px !important;
        line-height: 1 !important;
        font-size: 1pt !important;
        margin: 0 !important;
        padding: 0 !important;
        overflow: hidden;
    }

    .filler-dots {
        height: 90px;
        min-height: 90px;
        line-height: 1.1;
        overflow: hidden;
    }

    .signature-line { 
        margin: 8px 0 4px 0; 
        font-size: var(--fs-base);
    }
    .signature-line .line { 
        width: 30mm; 
        border-bottom: 1px solid #000; 
        margin-bottom: 2px; 
    }

    .section-divider { border-top: 1px solid #000; margin: 6px 0; }
    .footer-note { text-align: center; font-size: var(--fs-base); margin-top: 10px; }

    /* Prevent unwanted breaking / slicing */
    p, div, span, td, tr, .item-desc, .item-details, .good-condition, 
    .signature-line, .signature-spacer, .total-row, .section-title {
        break-inside: avoid !important;
        page-break-inside: avoid !important;
    }

    .receipt-table {
        page-break-inside: auto !important;
    }

    .receipt-table tr {
        page-break-inside: avoid !important;
        page-break-after: auto !important;
    }

    /* PRINT OPTIMIZATION */
    @media print {
        @page { size: 48mm auto; margin: 0; }

        body { margin: 0; padding: 0; overflow: hidden; }

        #ModuleHeader, .no-print, .button-footer { display: none !important; }

        .invoice-box {
            width: 48mm !important;
            padding: 1mm 0.8mm !important;
            margin: 0 !important;
            font-size: var(--fs-base) !important;
            line-height: var(--lh-base) !important;
        }

        .logo { max-width: 70px; height: 70px; }

        .comp-name     { font-size: var(--fs-name)    !important; }
        .comp-details  { font-size: var(--fs-company) !important; }
        .company-info  { font-size: var(--fs-company) !important; }
        .invoice-header{ font-size: var(--fs-header)  !important; }
        .section-title { font-size: var(--fs-header)  !important; }

        .receipt-table,
        .receipt-table td,
        .item-desc,
        .item-details td,
        .item-details .sku,
        .chargeable-header-row,
        .total-row td,
        .good-condition,
        .footer-note,
        .signature-line {
            font-size: var(--fs-base) !important;
        }

        .separator { font-size: var(--fs-small) !important; }

        .chargeable-header-row td,
        .item-details td {
            padding: 0.4px 1.2px !important;
        }

        .section-divider, .separator { margin: 4px 0 !important; }

        /* Force signature spacer to stay visible on thermal printers */
        .signature-spacer,
        .filler-dots {
            height: 70px !important;
            min-height: 70px !important;
            display: block !important;
        }

        .filler-dots span {
            color: #f0f0f0 !important;
        }

        .receipt-table, .total-row, .signature-line { 
            page-break-inside: avoid; 
        }

        .invoice-box {
            padding-bottom: 10mm !important;
        }

        /* Reinforce font family & background */
        body,
        .invoice-box,
        .invoice-box * {
            font-family: var(--ff-main) !important;
            background: #fff !important;
            -webkit-print-color-adjust: exact;
            print-color-adjust: exact;
        }
    }

    /* PDF generation mode (applied via JS) */
    .print-mode {
        margin: 0 !important;
        padding: 0 !important;
        overflow: hidden !important;
    }

    .print-mode #ModuleHeader,
    .print-mode .no-print,
    .print-mode .button-footer {
        display: none !important;
    }

    .print-mode .invoice-box {
        width: 48mm !important;
        padding: 1mm 0.8mm !important;
        margin: 0 !important;
        font-size: var(--fs-base) !important;
        line-height: var(--lh-base) !important;
    }

    .print-mode .logo { max-width: 65px !important; }

    .print-mode .comp-name     { font-size: var(--fs-name)    !important; }
    .print-mode .comp-details  { font-size: var(--fs-company) !important; }
    .print-mode .company-info  { font-size: var(--fs-company) !important; }
    .print-mode .invoice-header{ font-size: var(--fs-header)  !important; }
    .print-mode .section-title { font-size: var(--fs-header)  !important; }

    .print-mode .receipt-table,
    .print-mode .receipt-table td,
    .print-mode .item-desc,
    .print-mode .item-details td,
    .print-mode .item-details .sku { font-size: var(--fs-base) !important; }

    .print-mode .chargeable-header-row td,
    .print-mode .item-details td {
        padding: 0.4px 1.2px !important;
    }

    .print-mode .section-divider,
    .print-mode .separator { margin: 4px 0 !important; }

    .print-mode .signature-spacer,
    .print-mode .filler-dots {
        height: 70px !important;
        min-height: 70px !important;
    }

    .print-mode .filler-dots span {
        color: #f0f0f0 !important;
    }

    .print-mode .receipt-table,
    .print-mode .total-row,
    .print-mode .signature-line {
        page-break-inside: avoid !important;
    }

    /* Reinforce font family in PDF mode */
    .print-mode body,
    .print-mode .invoice-box,
    .print-mode .invoice-box * {
        font-family: var(--ff-main) !important;
        background: #fff !important;
    }
</style>
    
    <script type="text/javascript">
        //function downloadPDF() {
        //    const element = document.getElementById('printArea');

        //    var opt = {
        //        margin: 0,
        //        filename: 'receipt.pdf',

        //        image: {
        //            type: 'jpeg',
        //            quality: 1   // highest quality
        //        },

        //        html2canvas: {
        //            scale: 6,          // IMPORTANT: increase DPI
        //            useCORS: true,
        //            logging: false
        //        },

        //        jsPDF: {
        //            unit: 'mm',
        //            format: [58, element.scrollHeight * 0.2645],
        //            orientation: 'portrait'
        //        },

        //        pagebreak: {
        //            mode: ['avoid-all', 'css', 'legacy']
        //        }
        //    };

        //    html2pdf().set(opt).from(element).save();
        //}

        function downloadPDF() {
            const element = document.getElementById('printArea');
            const invoiceNo = document.getElementById('invoice-number-for-filename').innerText.trim() || 'receipt';

            var opt = {
                margin: 0,
                filename: `${invoiceNo}.pdf`,

                image: {
                    type: 'jpeg',
                    quality: 1   // highest quality
                },

                html2canvas: {
                    scale: 6,          // keep for DPI, but safe with pagination
                    useCORS: true,
                    logging: false
                },

                jsPDF: {
                    unit: 'mm',
                    format: [58, 297],  // fixed width 58mm, height 297mm (adjust if needed, e.g., 200 or 400 for shorter/taller pages)
                    orientation: 'portrait'
                },

                pagebreak: {
                    mode: ['avoid-all', 'css', 'legacy']
                }
            };

            html2pdf().set(opt).from(element).save();
        }


        function printInvoice() {
            const buttonFooter = document.querySelector('.button-footer');
            buttonFooter.style.display = 'none';
            window.print();
            buttonFooter.style.display = 'flex';
        }
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <!-- Hidden element for PDF filename -->
<div id="invoice-number-for-filename" style="display:none;">
    <asp:Literal ID="litInvoiceNoForFilename" runat="server" />
</div>
    <div class="invoice-box" id="printArea">
        <div class="logo-container">
            <asp:Image ID="imgLogo" runat="server" CssClass="logo" Visible="true" />
        </div>

        <div class="company-info">
            <span class="comp-name"><asp:Literal ID="litCompName" runat="server" /></span>
            <span class="comp-details">Registration NO: <asp:Literal ID="litCoReg" runat="server" /></span>
            <span class="comp-details"><asp:Literal ID="litCompAddr" runat="server" /></span>
            <span class="comp-details">TEL: <asp:Literal ID="litCompPhone" runat="server" /></span>
            <span class="comp-details">Vender Code: <asp:Literal ID="litOutletCode" runat="server" /></span>
        </div>

        <div class="invoice-header">
            <asp:Literal ID="litInvoiceHeader" runat="server" Mode="PassThrough" />
        </div>
        <div class="section-divider"></div>

        <%--<div class="section-title">CHARGEABLE</div>--%>

        <table class="receipt-table">
            <!-- Header row inside table for perfect alignment -->
            <tr class="chargeable-header-row">
                <td class="header-desc">CHARGEABLE</td>
                <td class="sku"></td>
                <td class="qty">Qty</td>
                <td class="exc">EXC</td>
                <td class="up">U/P</td>
                <td class="amt">Amt</td>
            </tr>

            <tbody>
                <asp:Literal ID="litChargeableRows" runat="server" />
            </tbody>
        </table>

        <table class="receipt-table">
            <tbody>
                <asp:Literal ID="litTotals" runat="server" />
            </tbody>
        </table>

        <div class="separator">################################</div>

        <div class="good-condition">
    Received in good condition.
</div>

<!-- Forced feed area – adjust number of lines as needed (try 8–15) -->
<div style="font-size: 3pt; line-height: 1.1; color: #000000; letter-spacing: 1px; margin: 0; padding: 0; text-align: right;">
    .<br>
    .<br>
    .<br>
    .<br>
    .<br>
    .<br>
    .<br>
    .<br>
    .<br>
    .<br>
    .<br>
    .<br>
    .<br>
</div>

<div class="signature-line">
    <div class="line"></div>
    <div>Received by:</div>
</div>

        <div class="section-divider"></div>
        <div class="section-title">EXCHANGE</div>
        <table class="receipt-table">
            <thead>
                <tr>
                    <th class="desc">Description</th>
                    <th class="sku"></th>
                    <th class="qty">QTY</th>
                    <th class="amt">Reason</th>
                </tr>
            </thead>
            <tbody>
                <asp:Literal ID="litExchangeRows" runat="server" />
            </tbody>
        </table>

        <div class="section-divider"></div>

        <div class="section-title">TOTAL DELIVERED</div>
        <table class="receipt-table">
            <thead>
                <tr>
                    <th class="desc">Product</th>
                    <th class="sku"></th>
                    <th class="qty">QTY</th>
                </tr>
            </thead>
            <tbody>
                <asp:Literal ID="litDeliveredRows" runat="server" />
            </tbody>
        </table>

        <div class="footer-note">
            <p>E.& O.E.<br />*** THANK YOU ***</p>
        </div>
    </div>

    <div class="button-footer no-print">
        <asp:Button ID="btnHome" runat="server" Text="Home" CssClass="btn btn-secondary  no-print" OnClick="btnHome_Click" />
        <button type="button" class="btn btn-info text-white  no-print" onclick="downloadPDF()">PDF</button>
        <button type="button" class="btn btn-primary  no-print" onclick="printInvoice()">PRINT</button>
    </div>
</asp:Content>