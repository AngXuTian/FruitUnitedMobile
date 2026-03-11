<%@ Page Title="SISTA - Delivery Plan Detail" Language="C#" MasterPageFile="~/ModulesPage.Master"
    AutoEventWireup="true"
    CodeBehind="Delivery_Plan_Detail.aspx.cs"
    Inherits="FruitUnitedMobile.Modules.Delivery_Plan_Detail" %>
<%@ Register Src="~/Component/SearchBar.ascx" TagPrefix="uc" TagName="SearchBar" %>
<%@ Register Src="~/Component/ProductImage.ascx" TagPrefix="uc" TagName="ProductImage" %>
<%@ Register Src="~/Component/Toast.ascx" TagPrefix="uc" TagName="Toast" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="<%= ResolveUrl("~/CSS/ButtonFooter.css") %>" rel="stylesheet" />
    <link href="<%= ResolveUrl("~/CSS/SearchFix.css") %>" rel="stylesheet" />

    <style>
        .products-container {
            display: grid;
            grid-template-columns: repeat(2, 1fr);
            gap: 14px;
            margin-top: 20px;
        }

        .products-container > *:only-child {
            /*grid-column: 1 / -1;*/
            grid-column: 1;
            /*justify-self: center;*/
            /*max-width: 320px;*/
        }

        .products-container > *:nth-last-child(odd):nth-child(odd):not(:nth-child(2)) {
           /* grid-column: 1 / -1;*/
            grid-column: 1;
            /*justify-self: center;*/
            /*max-width: 320px;*/
        }

        .product-card {
            display: flex;
            align-items: center;
            background: #fff;
            border: 1px solid #ddd;
            border-radius: 8px;
            padding: 12px;
            box-shadow: 0 1px 4px rgba(0,0,0,0.1);
            gap: 12px;
        }

        /*.product-card img {
            width: 50px;
            height: 50px;
            object-fit: contain;
            border-radius: 4px;
            flex-shrink: 0;
        }*/

        .product-text {
            flex: 1;
            min-width: 0;
        }

        .abbreviation {
            font-size: 0.8rem;
            font-weight: 500;
            white-space: nowrap;
            overflow: hidden;
            text-overflow: ellipsis;
            color: #212529;
        }

        .quantity {
            font-size: 0.7rem;
            color: #555;
        }

        .container {
            padding-bottom: 110px !important;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <uc:Toast ID="Toast1" runat="server" />
    <div class="container" style="padding: 20px;">
        <h2 class="text-center mb-4">SISTA - Delivery Plan Detail</h2>

        <div>
            <strong>PostCode:</strong>
            <asp:Label ID="lblPostCode" runat="server"></asp:Label><br />
            <asp:Label ID="lblOutletName" runat="server"></asp:Label><br />
            <asp:Label ID="lblAddress" runat="server"></asp:Label><br />

        </div>
        <hr />
        <h4>Products</h4>
        <uc:SearchBar ID="SearchBar1" runat="server" 
    Placeholder="Search product by abbreviation..." 
    SearchInfoText="Type to filter products"
    SearchFields="data-abbreviation" />

        <!-- PRODUCT GRID -->
        <div class="products-container" data-results-container="true">
            <asp:Repeater ID="rpProducts" runat="server">
                <ItemTemplate>
                    <div class="product-card" 
                         data-abbreviation='<%# Eval("Abbreviation")?.ToString().ToLower() %>'
                         data-searchable="true">
                        <uc:ProductImage runat="server"
    ProductId='<%# Eval("Product_Profile_ID") %>'
    FileName='<%# Eval("Filename") %>'
    Abbreviation='<%# Eval("Abbreviation") %>'
    ImageSize="60" />
                        <div class="product-text">
                            <div class="abbreviation"><%# Eval("Abbreviation") %></div>
                            <div class="quantity">Qty: <strong><%# Eval("Quantity") %></strong></div>
                        </div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </div>


        <asp:Label ID="lblNoProducts" runat="server" CssClass="alert alert-warning d-block"
            Visible="false" Text="No products found for this delivery."></asp:Label>

        <div class="button-footer">
            <asp:Button ID="btnBack" runat="server" Text="Back"
                CssClass="btn btn-secondary" OnClick="btnBack_Click" />
            <asp:Button ID="btnPrepareInvoice" runat="server" Text="Prepare Invoice"
                CssClass="btn btn-primary" OnClick="btnPrepareInvoice_Click" />
        </div>
    </div>
</asp:Content>