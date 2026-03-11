<%@ Page Title="" EnableEventValidation="false" Language="C#" MasterPageFile="~/ModulesPage.Master" AutoEventWireup="true" CodeBehind="GR-MaterialList.aspx.cs" Inherits="FruitUnitedMobile.Modules.GR_MaterialList" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        table {
            width: 100%;
        }

        td {
            padding: 10px;
            vertical-align: middle;
        }

        .label {
            color: black;
            font-weight: bold;
        }

        .not-mandatory {
            color: red;
            font-size: 12px;
        }

        .form-control {
            width: 100%; /* Or a specific width like 300px */
        }

        .form-control2 {
            width: 200px;
        }

        .dropdown-input {
            width: 100%;
            cursor: pointer;
        }

        .dropdown-panel {
            position: absolute;
            width: 100%;
            border: 1px solid #ccc;
            background-color: white;
            z-index: 1000;
            padding: 5px;
        }

        .CheckBoxListItem input[type="checkbox"] {
            margin-right: 8px;
        }

        .myButton {
            display: inline-block;
            background-color: #4CAF50; /* Green */
            color: white;
            padding: 8px 16px;
            font-size: 14px;
            border: none;
            border-radius: 6px;
            text-decoration: none;
            cursor: pointer;
            transition: background-color 0.3s ease;
        }

        .displaynone {
            display: none;
        }
    </style>
    <link href="../CSS/Basic.css" rel="stylesheet" />
    <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
    <link href="https://cdn.jsdelivr.net/npm/select2@4.1.0-rc.0/dist/css/select2.min.css" rel="stylesheet" />
    <script src="https://cdn.jsdelivr.net/npm/select2@4.1.0-rc.0/dist/js/select2.min.js"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:Label ID="ErrorMessage" runat="server"></asp:Label>
    <div style="font-family: Arial, sans-serif;">
        <div class="container mt-4 parent" style="height: 100%;">
            <div class="form-container" style="padding-bottom: 80px; overflow-x: hidden;">
                <div class="container TitleDiv">
                    <div>
                        <asp:Label runat="server" Text="Project : " Font-Bold="true" AssociatedControlID="ProjectTitle"></asp:Label>
                        <asp:Label runat="server" ID="ProjectTitle" Font-Bold="true"></asp:Label>
                    </div>
                </div>
                <table style="width: 100%;">
                    <tr>
                        <td class="TitleLabel">
                            <label class="label" for="ddlCategory">Category*</label>
                        </td>
                        <td>
                            <asp:DropDownList ID="ddlCategory" CssClass="form-control dropdown-mobile" runat="server"></asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td class="TitleLabel">
                            <label class="label" for="ContentPlaceHolder1_ddlBrand">Brand*</label>
                        </td>
                        <td>
                            <asp:DropDownList ID="ddlBrand" CssClass="form-control dropdown-mobile" onchange="LoadddlPart(this)" runat="server"></asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td class="TitleLabel">
                            <label class="label" for="ContentPlaceHolder1_ddlPart">Part*</label>
                        </td>
                        <td>
                            <asp:DropDownList ID="ddlPart" CssClass="form-control dropdown-mobile" onchange="LoadddlSerialNo(this);LoadUOMFromPart(this);" runat="server"></asp:DropDownList>
                            <asp:HiddenField ID="Material_Profile_ID_TB" runat="server"></asp:HiddenField>
                        </td>
                    </tr>
                    <tr>
                        <td class="TitleLabel">
                            <label class="label" for="ContentPlaceHolder1_ddlSN">Serial Number</label>
                        </td>
                        <td>
                            <asp:DropDownList ID="ddlSN" CssClass="form-control dropdown-mobile" onchange="SerialNo_OnChange(this);" runat="server"></asp:DropDownList>
                            <asp:HiddenField ID="Inventory_Ledger_ID_TB" runat="server"></asp:HiddenField>
                        </td>
                    </tr>
                    <tr>
                        <td class="TitleLabel">
                            <label class="label" for="QtyTB">Qty</label>
                        </td>
                        <td>
                            <div style="display: inline-block; width: 35%">
                                <asp:TextBox ID="QtyTB" CssClass="form-control" runat="server"></asp:TextBox>
                            </div>
                            <div style="display: inline-block; width: 35%; height: 100%">
                                <asp:Label ID="UOMLabel" runat="server"></asp:Label>
                            </div>
                            <div style="display: inline-block; width: 20%; height: 100%">
                                <asp:Button ID="btnAdd" runat="server" Text="Add" OnClick="btnAdd_Click" CssClass="btn btn-success" Style="padding: 10px 20px; margin-right: 10px;" />
                            </div>
                        </td>
                    </tr>
                </table>
                <div id="OutputGridDiv" style="overflow: auto;">
                    <asp:Panel ID="GridPanel" runat="server" ScrollBars="Vertical" Style="max-height: 65vh; overflow: auto; width: 100%;">
                        <asp:GridView ID="MaterialListGrid" runat="server" OnRowDataBound="MaterialListGrid_RowDataBound" OnRowCommand="MaterialListGrid_RowCommand" AutoGenerateColumns="false" CssClass="table table-bordered table-responsive">
                            <Columns>
                                <asp:TemplateField HeaderText="Item">
                                    <ItemTemplate>
                                        <%# Container.DataItemIndex + 1 %>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Project">
                                    <ItemTemplate>
                                        <asp:Label ID="Label1" Text='<%# Bind("Part_No") %>' Font-Size="Small" Font-Bold="true" runat="server"></asp:Label>
                                        <asp:HiddenField ID="Material_Profile_ID_HF" Value='<%# Bind("Material_Profile_ID") %>' runat="server"></asp:HiddenField>
                                        <br>
                                        <asp:Label ID="Label2" Text='<%# Bind("Part_Description") %>' Font-Size="Small" runat="server"></asp:Label>
                                        <br>
                                        <asp:Label ID="Label3" Text='<%# Bind("Serial_No") %>' Font-Size="Small" runat="server"></asp:Label>
                                        <asp:HiddenField ID="Inventory_Ledger_ID_HF" Value='<%# Bind("Inventory_Ledger_ID") %>' runat="server"></asp:HiddenField>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Qty">
                                    <ItemTemplate>
                                        <asp:Label ID="QtyTxt" Text='<%# Bind("Qty") %>' Font-Size="Small" runat="server"></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Action">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="lnkDelete" runat="server" CommandName="DeleteRow" CommandArgument='<%# Eval("Material_Profile_ID") %>' ToolTip="Delete"><i class="fa fa-times" aria-hidden="true"></i></asp:LinkButton>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </asp:Panel>
                </div>
                <table style="width: 100%;">
                    <tr>
                        <td class="TitleLabel">
                            <label class="label" for="ddlCategory">Remark</label>
                        </td>
                        <td>
                            <asp:TextBox ID="Remark_TB" TextMode="MultiLine" CssClass="form-control dropdown-mobile" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <label class="label" for="fileUpload1">Upload Image</label>
                        </td>
                        <td style="display: flex; justify-content: center; align-items: center;">
                            <asp:FileUpload ID="fileUpload1" runat="server" CssClass="form-control" Style="width: 100%;" accept="image/*" />
                            <span id="fileUploadError" style="color: red; display: none;">Please upload an image file.</span>
                        </td>
                    </tr>
                </table>
            </div>
        </div>
        <div style="position: fixed; bottom: 0; left: 0; width: 100vw; background-color: white; border-top: 1px solid #ccc; padding: 10px;">
            <div style="display: flex; justify-content: space-between; align-items: center;">
                <div>
                </div>
                <div>
                    <asp:Button ID="btnSubmit" runat="server" Text="Submit" Visible="false" OnClientClick="showLoader()" OnClick="btnSubmit_Click" CssClass="btn btn-success" Style="padding: 10px 20px; margin-right: 10px;" />
                </div>
            </div>
        </div>
    </div>
    <script>
        $(document).ready(function () {
            $('#<%= ddlCategory.ClientID %>').select2();
            $('#<%= ddlBrand.ClientID %>').select2();
            $('#<%= ddlPart.ClientID %>').select2();
            $('#<%= ddlSN.ClientID %>').select2();

            $("#<%= btnAdd.ClientID %>").click(function (e) {
                var val1 = $("#<%= ddlCategory.ClientID %>").val();
                var val2 = $("#<%= ddlBrand.ClientID %>").val();
                var val3 = $("#<%= ddlPart.ClientID %>").val();
                var val4 = $("#<%= QtyTB.ClientID %>").val();

                if (val1 == "0" || val2 == "0" || val3 == "0") {
                    alert("Please make sure all mandatory is selected.");
                    e.preventDefault();
                    return false;
                }

                // Second textbox OR condition → empty OR not number
                if (val4.trim() === "" || !/^\d+$/.test(val4) || val4 == "0") {
                    alert("QTY cannot be empty or 0 and must be numbers only!");
                    e.preventDefault();
                    return false;
                }
            });
        });

        document.addEventListener('DOMContentLoaded', function () {
        <% if (IsPostBack)
        { %>
            var BrandProfile_ID = $("#<%= ddlBrand.ClientID %>").val();
            if (BrandProfile_ID !== "0") {
                $("#<%= ddlBrand.ClientID %>").trigger('change');
            }
        <% } %>
        });

        function showLoader() {
            var overlay = document.getElementById('loader-overlay');
            if (overlay) {
                overlay.style.display = 'flex';
            }
        }


        function LoadddlPart(BrandDropDown) {
            var BrandID = BrandDropDown.value;

            if (BrandID == "0") {
                $("#<%= ddlPart.ClientID %>").empty();
                $("#<%= ddlPart.ClientID %>").append('<option value="0">-- Please Select --</option>');
                return;
            }

            $.ajax({
                type: "POST",
                url: "GR-MaterialList.aspx/GetddlPart",
                data: JSON.stringify({ BrandID: BrandID }),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    var Material_Part = JSON.parse(response.d);
                    var ddlPart = $("#<%= ddlPart.ClientID %>");
                    ddlPart.empty(); // clear old items
                    ddlPart.append('<option value="0">-- Please Select --</option>');

                    $.each(Material_Part, function (i, Part_No) {
                        ddlPart.append('<option value="' + Part_No.Material_Profile_ID + '">' + Part_No.Part_No + '</option>');
                    });

                    var Material_Profile_ID = '<%= Session["Material_Profile_ID"] %>';
                    if (Material_Profile_ID !== null && Material_Profile_ID !== undefined && Material_Profile_ID !== '' && Material_Profile_ID !== 0) {
                        $("#<%= ddlPart.ClientID %>").val(Material_Profile_ID);
                        $("#<%= ddlPart.ClientID %>").trigger('change');
                    }
                },
                error: function (xhr, status, error) {
                    console.error(xhr.responseText);
                }
            });
        }

        function LoadddlSerialNo(PartDropDown) {
            var PartID = PartDropDown.value;

            if (PartID == "0") {
                $("#<%= ddlSN.ClientID %>").empty();
                $("#<%= ddlSN.ClientID %>").append('<option value="0">-- Please Select --</option>');
                $("#<%= Material_Profile_ID_TB.ClientID %>").val('0');
                return;
            }

            $("#<%= Material_Profile_ID_TB.ClientID %>").val(PartID);

            $.ajax({
                type: "POST",
                url: "GR-MaterialList.aspx/GetddlSerialNo",
                data: JSON.stringify({ PartID: PartID }),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    var Material_SerialNo = JSON.parse(response.d);
                    var ddlSN = $("#<%= ddlSN.ClientID %>");
                    ddlSN.empty(); // clear old items
                    ddlSN.append('<option value="0">-- Please Select --</option>');

                    $.each(Material_SerialNo, function (i, Serial_No) {
                        ddlSN.append('<option value="' + Serial_No.Inventory_Ledger_ID + '">' + Serial_No.Serial_No + '</option>');
                    });

                },
                error: function (xhr, status, error) {
                    console.error(xhr.responseText);
                }
            });
        }

        function LoadUOMFromPart(PartDropDown) {
            var PartID = PartDropDown.value;

            if (PartID == "0") {
                $('#<%= UOMLabel.ClientID %>').text('');
                return;
            }

            $.ajax({
                type: "POST",
                url: "GR-MaterialList.aspx/GetUOMFromPart",
                data: JSON.stringify({ PartID: PartID }),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    var UOM = JSON.parse(response.d);
                    $.each(UOM, function (i, UOM_Text) {
                        $('#<%= UOMLabel.ClientID %>').text(UOM_Text.UOM);
                        $('#<%= UOMLabel.ClientID %>').css('height', '100%');
                    });
                },
                error: function (xhr, status, error) {
                    console.error(xhr.responseText);
                }
            });
        }

        function SerialNo_OnChange(SerialNoDropDown) {
            var SerialNo_ID = SerialNoDropDown.value;

            if (SerialNo_ID == "0") {
                $('#<%= QtyTB.ClientID %>').val('');
                $('#<%= QtyTB.ClientID %>').prop("readonly", false);
                $("#<%= Inventory_Ledger_ID_TB.ClientID %>").val('0');
                return;
            }
            else {
                $('#<%= QtyTB.ClientID %>').val('1');
                $('#<%= QtyTB.ClientID %>').prop("readonly", true);
                $("#<%= Inventory_Ledger_ID_TB.ClientID %>").val(SerialNo_ID);
                return;
            }
        }

</script>
</asp:Content>
