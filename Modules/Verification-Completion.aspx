<%@ Page Title="" Language="C#" MasterPageFile="~/ModulesPage.Master" AutoEventWireup="true" CodeBehind="Verification-Completion.aspx.cs" Inherits="FruitUnitedMobile.Modules.Verification_Completion" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
              .table tbody tr:hover {
    background-color: #f1f1f1;
}
.container .label-column {
        vertical-align: top;
    }

    @media (max-width: 768px) {
        .container .label-column {
            vertical-align: top; /* Align top in mobile view */
        }
    }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
          <link href="../CSS/Basic.css" rel="stylesheet" />
     <div class="container">
        <!-- Project Section -->
       <%-- <div style="margin-bottom: 20px;">
            <asp:Label runat="server" Text="Project:" AssociatedControlID="txtProject"></asp:Label>
            <asp:TextBox ID="txtProject" runat="server" Text="" ReadOnly="true" CssClass="form-control" style="width: 100%; margin-bottom: 10px;"></asp:TextBox>

            <asp:Label runat="server" Text="From:" AssociatedControlID="txtFrom"></asp:Label>
            <asp:TextBox ID="txtFrom" runat="server" Text="" ReadOnly="true" CssClass="form-control" style="width: 100%; margin-bottom: 10px;"></asp:TextBox>

            <asp:Label runat="server" Text="To:" AssociatedControlID="txtTo"></asp:Label>
            <asp:TextBox ID="txtTo" runat="server" ReadOnly="true" CssClass="form-control" style="width: 100%; margin-bottom: 10px;"></asp:TextBox>

            <asp:Label runat="server" Text="Scope:" AssociatedControlID="txtScope"></asp:Label>
            <asp:TextBox ID="txtScope" runat="server" Text="" ReadOnly="true" CssClass="form-control" style="width: 100%;"></asp:TextBox>
        </div>--%>
        <div class="container" style="border:2px solid black; width:95%; overflow-x: hidden; background-color: white;">
         <table style="width: 100%; border-collapse: collapse;">
              <tr>
                  <td colspan="2"><span class="badge bg-primary">Project Info</span></td>
              </tr>
                <tr>
                    <td class="label-column" style="font-weight: bold;">Project:</td>
                    <td class="label-column"><asp:Label runat="server" ID="txtProject"></asp:Label></td>
                </tr>
                <tr>
                    <td class="label-column" style="font-weight: bold;">From:</td>
                    <td class="label-column"><asp:Label runat="server" ID="txtFrom"></asp:Label></td>
                </tr>
                <tr>
                    <td class="label-column" style="font-weight: bold;">To:</td>
                    <td class="label-column"><asp:Label runat="server" ID="txtTo"></asp:Label></td>
                </tr>
                <tr>
                    <td class="label-column" style="font-weight: bold;">Scope:</td>
                    <td class="label-column"><asp:Label runat="server" ID="txtScope"></asp:Label></td>
                </tr>
                  <tr>
                    <td class="label-column" style="font-weight: bold;">Scope Info:</td>
                    <td class="label-column"><asp:Label runat="server" ID="txtScopeInfo"></asp:Label></td>
                </tr>
            </table>
        
        </div>
        <%--<div class="card bg-light" >
          <div class="card-header" style="font-weight:bold;">Project Info</div>
          <div class="card-body">
            
          </div>
        </div>   --%>

        <!-- Task Info Section -->
        <%--<div style="background-color: green; color: white; padding: 10px; margin-bottom: 10px;">
            <strong>Task Info</strong>
        </div>
        <div style="margin-bottom: 20px;">
            <asp:Label runat="server" Text="Task:" AssociatedControlID="txtTask"></asp:Label>
            <asp:TextBox ID="txtTask" runat="server" Text="" ReadOnly="true" CssClass="form-control" style="width: 100%; margin-bottom: 10px;"></asp:TextBox>

            <asp:Label runat="server" Text="Tools:" AssociatedControlID="txtTools"></asp:Label>
            <asp:TextBox ID="txtTools" runat="server" Text="" ReadOnly="true" CssClass="form-control" style="width: 100%; margin-bottom: 10px;"></asp:TextBox>

            <asp:Label runat="server" Text="Remark:" AssociatedControlID="txtRemark"></asp:Label>
            <asp:TextBox ID="txtRemark" runat="server" TextMode="MultiLine" ReadOnly="true" CssClass="form-control" style="width: 100%; margin-bottom: 10px;"></asp:TextBox>

            <asp:Label runat="server" Text="Completed:" AssociatedControlID="txtCompletedQty"></asp:Label>
            <div style="display: flex; align-items: center; gap: 10px;">
                <asp:Label runat="server" Text="Qty:" AssociatedControlID="txtCompletedQty"></asp:Label>
                <asp:TextBox ID="txtCompletedQty" runat="server" Text="" ReadOnly="true" CssClass="form-control" style="width: 150px;"></asp:TextBox>
                <asp:Label runat="server" Text="(%):" AssociatedControlID="txtCompletedPercentage"></asp:Label>
                <asp:TextBox ID="txtCompletedPercentage" runat="server" Text="" ReadOnly="true" CssClass="form-control" style="width: 50px;"></asp:TextBox>
            </div>
        </div>--%>
         <br/>
         <div class="container" style="border:2px solid black; width:95%; overflow-x: hidden; background-color: white;">
         <table style="width: 100%; border-collapse: collapse;">
             <tr>
                  <td colspan="2"><span class="badge bg-primary">Task Info</span></td>
              </tr>
               <tr>
                    <td class="label-column" style="font-weight: bold;">Task:</td>
                    <td class="label-column"><asp:Label runat="server" ID="txtTask"></asp:Label></td>
                </tr>
             <tr>
                    <td class="label-column" style="font-weight: bold;">Task Info:</td>
                    <td class="label-column"><asp:Label runat="server" ID="txtTaskInfo"></asp:Label></td>
                </tr>
                <tr>
                    <td class="label-column" style="font-weight: bold;">Tools:</td>
                    <td class="label-column"><asp:Label runat="server" ID="txtTools"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="label-column" style="font-weight: bold;">Remark:</td>
                    <td class="label-column"><asp:Label runat="server" ID="txtRemark"></asp:Label></td>
                </tr>
                <tr>
                    <td class="label-column" style="font-weight: bold;">Comp Qty:</td>
                    <td><asp:Label runat="server" ID="txtCompletedQty"></asp:Label>
                        <asp:Label runat="server" Style="margin-left:15px;" Font-Bold="true" Text="(%):"></asp:Label>
                        <asp:Label runat="server" ID="txtCompletedPercentage"></asp:Label></td>
                </tr>
                 
            </table>
        
        </div>


        <!-- Completion Section -->
        <%--<div style="background-color: green; color: white; padding: 10px; margin-bottom: 10px;">
            <strong>Completion</strong>
        </div>--%>
         <br>
         <div class="container">
         <span class="badge bg-primary" width:95%;>Completion</span>
        <div width:95%;>
            <div style="display: flex; justify-content: space-between;">
                <div>
                    <asp:Label runat="server" Text="Qty:" AssociatedControlID="txtQty"></asp:Label>
                    <asp:TextBox ID="txtQty" runat="server" CssClass="form-control calc-input" ></asp:TextBox>
                    <asp:RequiredFieldValidator 
                        ID="rfvQty" 
                        runat="server" 
                        ControlToValidate="txtQty" 
                        ErrorMessage="* Required" 
                        CssClass="text-danger small">
                    </asp:RequiredFieldValidator>
                    <asp:HiddenField ID="ops" runat="server" Value="0"></asp:HiddenField>
                    <asp:HiddenField ID="average" runat="server" Value="0"></asp:HiddenField>
                    <asp:HiddenField ID="highest" runat="server" Value="0"></asp:HiddenField>

                </div>
                <div>
                    <asp:Label runat="server" Text="No of Man:" AssociatedControlID="txtNoOfMan"></asp:Label>
                    <asp:TextBox ID="txtNoOfMan" runat="server" CssClass="form-control calc-input" ></asp:TextBox>
                     <asp:RequiredFieldValidator 
     ID="RequiredFieldValidator1" 
     runat="server" 
     ControlToValidate="txtNoOfMan" 
    ErrorMessage="* Required" 
    CssClass="text-danger small">
 </asp:RequiredFieldValidator>
                </div>
                <div>
                    <asp:Label runat="server" Text="No of Hr(s):" AssociatedControlID="txtNoOfHours"></asp:Label>
                    <asp:TextBox ID="txtNoOfHours" runat="server" CssClass="form-control calc-input" ></asp:TextBox>
                     <asp:RequiredFieldValidator 
     ID="RequiredFieldValidator2" 
     runat="server" 
     ControlToValidate="txtNoOfHours" 
      ErrorMessage="* Required" 
    CssClass="text-danger small">
 </asp:RequiredFieldValidator>
                </div>
            </div>
             
            <div>
                <asp:Label runat="server" Text="Benchmark:"></asp:Label>
                <div style="display: flex; justify-content: space-between;">
                    <div>
                    <asp:Label runat="server" Text="Ops:" ></asp:Label>
                    <asp:TextBox ID="txtOps" runat="server" ReadOnly="true" CssClass="form-control" ></asp:TextBox>
                        </div>
                    <div><asp:Label runat="server" Text="Average:"></asp:Label>
                    <asp:TextBox ID="txtAverage" runat="server" ReadOnly="true" CssClass="form-control" ></asp:TextBox></div>
                    <div><asp:Label runat="server" Text="Highest:"></asp:Label>
                    <asp:TextBox ID="txtHighest" runat="server" ReadOnly="true" CssClass="form-control" ></asp:TextBox></div>
                </div>
            </div>
            <br>
            <asp:Label runat="server" Text="Remark:" AssociatedControlID="txtCompletionRemark"></asp:Label>
            <asp:TextBox ID="txtCompletionRemark" runat="server" TextMode="MultiLine" Placeholder="Enter Remark" CssClass="form-control" style="width: 100%;"></asp:TextBox>
            <br>
            <asp:Label runat="server" Text="Qty Exceeded Reason:" AssociatedControlID="ddlQtyExceededReason"></asp:Label>
            <asp:DropDownList ID="ddlQtyExceededReason" runat="server" CssClass="form-control" style="width: 100%;">
            </asp:DropDownList>
        </div>

       <%-- <!-- Image Upload Section -->
        <div style="margin-bottom: 20px;">
            <asp:Label runat="server" Text="Upload Images (Optional):" AssociatedControlID="fileUploadImages"></asp:Label>
            <asp:FileUpload ID="fileUploadImages" runat="server" AllowMultiple="true" CssClass="form-control" style="width: 100%; margin-bottom: 10px;" accept="image/*" />
            <asp:Label ID="lblUploadHint" runat="server" Text="You can upload multiple images. This field is optional." ForeColor="Gray"></asp:Label>
        </div>--%>

        
        <div style="display: flex; justify-content: space-between; margin-bottom: 20px;">
            <div>
                            <asp:Label runat="server" Text="By" ></asp:Label>
                            <asp:TextBox ID="txtBy" runat="server" ReadOnly CssClass="form-control" Text=""></asp:TextBox>
            </div>
            <div>
                          <asp:Label runat="server" Text="On" ></asp:Label>
                            <asp:TextBox ID="txt_completion_date" runat="server" ReadOnly CssClass="form-control" Text=""></asp:TextBox>

            </div>
        </div>
<asp:Panel ID="pnlFiles" runat="server" Visible="false">
    <table class="table">
        <thead>
            <tr>
                <th>File Name</th>
                <th>Download</th>
            </tr>
        </thead>
        <tbody>
            <asp:Repeater ID="rptFiles" runat="server" OnItemCommand="rptFiles_ItemCommand">
                <ItemTemplate>
                    <tr>
                        <td><%# Eval("FileName") %></td>
                        <td>
                            <asp:Button ID="btnDownload" runat="server" Text="Download" 
                                        CommandName="Download" CommandArgument='<%# Eval("FilePath") %>' 
                                        CssClass="btn btn-success" />
                        </td>
                    </tr>
                </ItemTemplate>
            </asp:Repeater>
        </tbody>
    </table>
</asp:Panel>

<asp:Label ID="lblMessage" runat="server" ForeColor="Red" Visible="false"></asp:Label>




    </div>
       <br><br><br><br>

        <div style="position: fixed; bottom: 0; left: 0; width: 100vw; background-color: white; border-top: 1px solid #ccc; padding: 10px;">
            <div style="display: flex; justify-content: space-between; align-items: center;">
                <div>
                    <asp:Button ID="btnToProject" runat="server" Text="Project" CausesValidation="false"  OnClick="btnToProject_Click" CssClass="btn btn-secondary" style="padding: 10px 20px;" />
                    <asp:Button ID="btnToScope" runat="server" Text="Scope" CausesValidation="false"  OnClick="btnToScope_Click" CssClass="btn btn-secondary" style="padding: 10px 20px;" />
                    <asp:Button ID="btnToTask" runat="server" Text="Task" CausesValidation="false"  OnClick="btnToTask_Click" CssClass="btn btn-secondary" style="padding: 10px 20px;" />

                </div>
                <div>
                    <asp:Button ID="btnDelete" runat="server" Text="❌" OnClick="btnDelete_Click" CssClass="btn btn-light" style="padding: 10px 20px;margin-right:10px;" /> 
                    <asp:Button ID="btnSubmit" runat="server" Text="✓" OnClick="btnSubmit_Click" CssClass="btn btn-success" style="padding: 10px 20px; margin-right:10px;" /> </div>
            </div>
        </div>
    </div>
    <script>
        document.addEventListener('DOMContentLoaded', function () {
            // Get all input elements
            const inputs = document.querySelectorAll('.calc-input');
            const txtOps = document.getElementById('<%= txtOps.ClientID %>');
            const txtAverage = document.getElementById('<%= txtAverage.ClientID %>');
            const txtHighest = document.getElementById('<%= txtHighest.ClientID %>');

            const opsTask = parseFloat(document.getElementById('<%= ops.ClientID %>').value) || 0;
            const averageTask = parseFloat(document.getElementById('<%= average.ClientID %>').value) || 0;
            const highestTask = parseFloat(document.getElementById('<%= highest.ClientID %>').value) || 0;

    // Attach input event listeners to calculation inputs
    inputs.forEach(input => {
        input.addEventListener('input', calculate);
    });

    // Calculation function
    function calculate() {
        // Parse values from inputs
        const qty = parseFloat(document.getElementById('<%= txtQty.ClientID %>').value) || 0;
        const noOfMan = parseFloat(document.getElementById('<%= txtNoOfMan.ClientID %>').value) || 0;
        const noOfHours = parseFloat(document.getElementById('<%= txtNoOfHours.ClientID %>').value) || 0;

        // Perform calculations
        const ops = opsTask *  noOfMan * noOfHours; // Ops calculation
        const average = averageTask * noOfMan * noOfHours; // Example Average calculation
        const highest = highestTask * noOfMan * noOfHours; // Example Highest calculation
        //console.log("ops:" + opsTask);
        //console.log("average:" + opsTask);
        //console.log("highest:" + opsTask);

                // Update read-only fields
                txtOps.value = `${ops.toFixed(2)}`;
                txtAverage.value = `${average.toFixed(2)}`;
                txtHighest.value = `${highest.toFixed(2)}`;
            }
        });

    </script>
</asp:Content>
