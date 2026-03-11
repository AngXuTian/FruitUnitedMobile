<%@ Page Title="" Language="C#" MasterPageFile="~/ModulesPage.Master" AutoEventWireup="true" CodeBehind="Completion-TaskCompletion.aspx.cs" Inherits="FruitUnitedMobile.Modules.Completion_TaskCompletion" %>
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
        <link href="../CSS/Basic.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div style="padding: 20px; font-family: Arial, sans-serif;">
        <!-- Project Section -->
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

        <!-- Task Info Section -->
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
            <asp:TextBox ID="txtCompletionRemark" runat="server" TextMode="MultiLine" Placeholder="Enter Remark" CssClass="form-control" style="width: 100%; "></asp:TextBox>
            <br>
            <asp:Label runat="server" Text="Qty Exceeded Reason:" AssociatedControlID="ddlQtyExceededReason"></asp:Label>
            <asp:DropDownList ID="ddlQtyExceededReason" runat="server" CssClass="form-control" style="width: 100%;">
            </asp:DropDownList>
        </div>

        <!-- Image Upload Section -->
        <div style="margin-bottom: 20px;">
            <asp:Label runat="server" Text="Upload Images (Optional):" AssociatedControlID="fileUploadImages"></asp:Label>
            <asp:FileUpload ID="fileUploadImages" runat="server" AllowMultiple="true" CssClass="form-control"  style="width: 100%; margin-bottom: 10px;" accept="image/*" />
            <asp:Label ID="lblUploadHint" runat="server" Text="You can upload multiple images. This field is optional." ForeColor="Gray"></asp:Label>
        </div>
        <!-- Fiel Upload Section -->
        <div style="margin-bottom: 20px;">
            <asp:Label runat="server" Text="Completion File (Optional):" AssociatedControlID="fileUploadPDF"></asp:Label>
            <asp:FileUpload ID="fileUploadPDF" runat="server" AllowMultiple="true" CssClass="form-control"  style="width: 100%; margin-bottom: 10px;"/>
        </div>
        <!-- Test Report Upload Section -->
        <div style="margin-bottom: 20px;">
            <asp:Label runat="server" Text="Test Report (Optional):" AssociatedControlID="fileUploadTestReport"></asp:Label>
            <asp:FileUpload ID="fileUploadTestReport" runat="server" AllowMultiple="true" CssClass="form-control"  style="width: 100%; margin-bottom: 10px;"/>
        </div>
        </div>
         <br><br><br><br>

        <!-- Bottom Buttons Section -->
        <div style="position: fixed; bottom: 0; left: 0; width: 100vw; background-color: white; border-top: 1px solid #ccc; padding: 10px;">
            <div style="display: flex; justify-content: space-between; align-items: center;">
                <div>
                    <asp:Button ID="btnToProject" runat="server" Text="Project" CausesValidation="false"  OnClick="btnToProject_Click" CssClass="btn btn-secondary" style="padding: 10px 20px;" />
                    <asp:Button ID="btnToScope" runat="server" Text="Scope" CausesValidation="false"  OnClick="btnToScope_Click" CssClass="btn btn-secondary" style="padding: 10px 20px;" />
                    <asp:Button ID="btnToTask" runat="server" Text="Task" CausesValidation="false"  OnClick="btnToTask_Click" CssClass="btn btn-secondary" style="padding: 10px 20px;" />

                </div>
                <div>
                    <asp:Button ID="btnSubmit" runat="server" Text="Submit" OnClick="btnSubmit_Click" OnClientClick="disableButton()"  CssClass="btn btn-success" style="padding: 10px 20px; margin-right:10px;" />
                </div>
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
        function disableButton() {
            var btn = document.getElementById('<%= btnSubmit.ClientID %>');
            setTimeout(function () {
                btn.disabled = true;
            }, 50); // Small delay to allow form submission
            alert('Submission was successful.');
        }

    </script>
</asp:Content>


