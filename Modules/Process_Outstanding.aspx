<%@ Page Title="SISTA - Process Outstanding" Language="C#" MasterPageFile="~/ModulesPage.Master"
    AutoEventWireup="true" CodeBehind="Process_Outstanding.aspx.cs"
    Inherits="FruitUnitedMobile.Modules.Process_Outstanding" %>
<%@ Register Src="~/Component/ProductImage.ascx" TagPrefix="uc" TagName="ProductImage" %>
<%@ Register Src="~/Component/Toast.ascx" TagPrefix="uc" TagName="Toast" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
     <link href="<%= ResolveUrl("~/CSS/ButtonFooter.css") %>" rel="stylesheet" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <style>
        .container { 
            max-width: 720px; 
            margin: 0 auto; 
            padding: 16px 12px; 
            padding-bottom: 100px;
        }
        h2 { 
            font-weight: 700; 
            font-size: 1.8rem; 
            margin-bottom: 1.5rem; 
            text-align: center; 
            color: #222; 
        }
        .info-badge {
            background: #e7f3ff; 
            padding: 12px 16px; 
            border-radius: 8px; 
            margin-bottom: 24px;
            font-size: 1rem; 
            font-weight: 600; 
            color: #004085; 
            text-align: center;
        }
        .product-card {
            display: flex; 
            align-items: center; 
            justify-content: space-between;
            background: #fff; 
            border: 1px solid #ddd; 
            border-radius: 10px;
            padding: 12px 16px; 
            margin-bottom: 16px; 
            box-shadow: 0 1px 6px rgba(0,0,0,0.07);
            gap: 12px; 
            transition: all 0.3s ease; 
            cursor: pointer; 
            flex-wrap: nowrap;
            position: relative;
        }
        .product-card:active { 
            background: #f0f8ff; 
        }

        /* Highlight problematic products */
        .product-card.outstanding-error {
            border: 2px solid #dc3545 !important;
            background: #fff5f5 !important;
            box-shadow: 0 2px 8px rgba(220, 53, 69, 0.3) !important;
            animation: shake 0.5s;
        }

        @keyframes shake {
            0%, 100% { transform: translateX(0); }
            25% { transform: translateX(-5px); }
            75% { transform: translateX(5px); }
        }

        .outstanding-warning-badge {
            position: absolute;
            top: 8px;
            right: 8px;
            background: #dc3545;
            color: white;
            padding: 4px 8px;
            border-radius: 4px;
            font-size: 0.75rem;
            font-weight: 600;
            z-index: 10;
        }

        .product-image { 
            flex-shrink: 0; 
            width: 70px; 
            height: 70px; 
            object-fit: contain;
            border-radius: 8px; 
            border: 1px solid #ccc; 
            background: #fefefe; 
        }
        .product-info { 
            flex: 1; 
            min-width: 0; 
        }
        .product-abbreviation { 
            font-weight: 700; 
            font-size: 1.3rem; 
            color: #222;
            white-space: nowrap; 
            overflow: hidden; 
            text-overflow: ellipsis; 
        }
        .product-qty { 
            font-size: 0.95rem; 
            color: #555; 
            font-weight: 600; 
        }
        .outstanding-toggle {
            font-weight: 700; 
            font-size: 1.3rem; 
            min-width: 70px; 
            text-align: right;
            transition: color 0.2s ease;
        }
        .outstanding-toggle.is-yes {
            color: #28a745;
        }
        .outstanding-toggle.is-no {
            color: #d32f2f;
        }
        .empty-state { 
            text-align: center; 
            padding: 48px 24px; 
            color: #6c757d; 
            font-size: 1.1rem; 
        }
        
        @media (max-width: 480px) {
            .container {
                padding: 12px 8px;
                padding-bottom: 100px;
            }
            .product-card {
                padding: 10px 12px;
            }
            .product-image { 
                width: 60px; 
                height: 60px; 
            }
            .product-abbreviation { 
                font-size: 1.15rem; 
            }
            .outstanding-toggle { 
                font-size: 1.1rem; 
                min-width: 60px;
            }
            .outstanding-warning-badge {
                font-size: 0.7rem;
                padding: 3px 6px;
            }
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <uc:Toast ID="Toast1" runat="server" />
    <div class="container">
        <h2>Process Outstanding</h2>
        <div class="info-badge">
            Tap item to toggle <strong>Outstanding: YES / NO</strong>
        </div>

        <asp:Panel ID="pnlOutstandingItems" runat="server">
            <asp:Repeater ID="rptOutstandingItems" runat="server">
                <ItemTemplate>
                    <div class="product-card" data-productid='<%# Eval("Product_Profile_ID") %>'>
                        <asp:HiddenField ID="hfIsOutstanding" runat="server" 
                            Value='<%# Convert.ToBoolean(Eval("IsOutstanding")) ? "1" : "0" %>' />

                        <uc:ProductImage runat="server"
                            ProductId='<%# Eval("Product_Profile_ID") %>'
                            FileName='<%# Eval("Filename") %>'
                            Abbreviation='<%# Eval("Abbreviation") %>'
                            ImageSize="60" />

                        <div class="product-info">
                            <div class="product-abbreviation"><%# Eval("Abbreviation") %></div>
                            <div class="product-qty">
                                Qty: <%# Eval("Return_Quantity") %>
                                <small class="text-muted">(<%# GetUOM(Eval("Product_Profile_ID")) %>)</small>
                            </div>
                        </div>

                        <div class="outstanding-toggle <%# Convert.ToBoolean(Eval("IsOutstanding")) ? "is-yes" : "is-no" %>">
                            <asp:Label ID="lblToggleState" runat="server" 
                                Text='<%# Convert.ToBoolean(Eval("IsOutstanding")) ? "YES" : "NO" %>'>
                            </asp:Label>
                        </div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </asp:Panel>

        <asp:Panel ID="pnlEmpty" runat="server" Visible="false" CssClass="empty-state">
            <p>No return items to process.<br>You can proceed.</p>
        </asp:Panel>

        <div class="button-footer">
            <asp:Button ID="btnBack" runat="server" Text="Back" CssClass="btn btn-secondary" OnClick="btnBack_Click" />
            <asp:Button ID="btnNext" runat="server" Text="Next" CssClass="btn btn-primary" OnClick="btnNext_Click" />
        </div>
    </div>

    <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
    <script type="text/javascript">
        $(function () {
            $('.product-card').on('click', function () {
                var $card = $(this);
                $card.removeClass('outstanding-error');
                var badge = $card.find('.outstanding-warning-badge');
                if (badge.length) badge.remove();

                var $hf = $card.find('input[type=hidden]');
                var $toggle = $card.find('.outstanding-toggle');
                var current = $hf.val();
                var newVal = current === "1" ? "0" : "1";

                $hf.val(newVal);
                $toggle.text(newVal === "1" ? "YES" : "NO");

                if (newVal === "1") {
                    $toggle.removeClass('is-no').addClass('is-yes');
                } else {
                    $toggle.removeClass('is-yes').addClass('is-no');
                }

                $card.addClass('bg-light');
                setTimeout(function () {
                    $card.removeClass('bg-light');
                }, 200);
            });
        });

        function highlightProblematicProducts() {
            $('.product-card').each(function () {
                $(this).removeClass('outstanding-error');
                $(this).find('.outstanding-warning-badge').remove();
            });

            var problematicIds = <%= Session["ProblematicProducts"] != null ? 
                Newtonsoft.Json.JsonConvert.SerializeObject(Session["ProblematicProducts"]) : "[]" %>;

            if (problematicIds.length === 0) return;

            problematicIds.forEach(function (productId, index) {
                var $card = $('[data-productid="' + productId + '"]');
                if ($card.length) {
                    $card.addClass('outstanding-error');
                    if (index === 0) {
                        setTimeout(function () {
                            $card[0].scrollIntoView({ behavior: 'smooth', block: 'center' });
                        }, 300);
                    }
                }
            });
        }

        $(window).on('load', function () {
            var problematicIds = <%= Session["ProblematicProducts"] != null ? 
                Newtonsoft.Json.JsonConvert.SerializeObject(Session["ProblematicProducts"]) : "[]" %>;
            if (problematicIds.length > 0) {
                highlightProblematicProducts();
            }
        });
    </script>
</asp:Content>