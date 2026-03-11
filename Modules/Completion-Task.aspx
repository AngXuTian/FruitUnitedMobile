<%@ Page Title="" Language="C#" MasterPageFile="~/ModulesPage.Master" AutoEventWireup="true" CodeBehind="Completion-Task.aspx.cs" Inherits="FruitUnitedMobile.Modules.Completion_Task" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
        <script type="text/javascript" src="../JS/BookingSummary.js" defer="defer"></script>
        <link rel="stylesheet" href="../CSS/DeliveryList.css" />
        <link href="../CSS/Basic.css" rel="stylesheet" />
    <style>
        .highlight-row {
     background-color: #28a745; /* Green background */
    color: white; /* Optional: make text white for better contrast */
    font-weight: bold; /* Optional: make text bold */
}
         .table {
    border-collapse: collapse;
    width: 100%;
}

.table th, .table td {
    padding: 10px;
    border: 1px solid #ddd;
}

/* Make first row of the header sticky */
.table thead tr:nth-child(1) th {
    position: sticky;
    top: 0; /* First row stays at the very top */
    z-index: 3;
    background-color: #343a40; /* Dark grey */
    color: #ffffff; /* White text */
}

/* Make second row of the header sticky */
.table thead tr:nth-child(2) th {
    position: sticky;
    top: 30.17px; /* Adjust this value to the height of the first row */
    z-index: 2;
    background-color: #343a40; /* Dark grey */
    color: #ffffff; /* White text */
}

/* Add hover effect for table rows */
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
    <div class="container" style="border:5px solid black; width:95%; overflow-x: hidden; background-color: white;">
    <table style="width: 100%; border-collapse: collapse;">
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
    <!-- Search Icon -->
<div class="container">
<div class="row">
    <div class="col-md-12 text-right" style="text-align:right;">
        <asp:LinkButton ID="SearchToggleButton" runat="server" CssClass="btn btn-link" OnClientClick="toggleSearch(); return false;">
            <i id="search-icon" style="color:black;" class="fa fa-search-plus" aria-hidden="true"></i>
        </asp:LinkButton>
    </div>
</div>
</div>
<!-- Search Fields -->
<div class="container">
<div id="search-container" class="row" style="display: none;">
    <div class="row mb-3">
        <div class="col-md-4">
            <asp:TextBox ID="SONoTextBox" runat="server" CssClass="form-control" Placeholder="Search by Task"></asp:TextBox>
        </div>
    </div>
    <div class="row">
        <div class="col-md-12" style="text-align: right;">
            <asp:Button ID="SearchButton" runat="server" Text="Search" CssClass="btn btn-primary" OnClick="SearchButton_Click" />
        </div>
    </div>
</div>
</div>

    <div id="GridDiv" class="container">
        <div  class="row" style="margin-top: 10px;">
<div>
    <asp:Panel ID="GridPanel" runat="server" ScrollBars="Vertical" Style="max-height: 65vh; overflow: auto; width: 100%;">
                <asp:GridView ID="InvoiceGrid" runat="server" AutoGenerateColumns="false" OnRowDataBound="InvoiceGrid_RowDataBound" GridLines="Both" CssClass="table table-bordered table-responsive">
                    <Columns>  
            <asp:TemplateField>  
                <HeaderTemplate> 
                    <thead>
                    <tr> 
                        <th colspan="2" ></th>
                        <th colspan="2" style="text-align:center">Completed</th>
                        <th colspan="1" ></th>
                    </tr>  
                    <tr >  
                        <th style="text-align:center">S/N</th>
                        <th style="text-align:center">Task</th>  
                        <th style="text-align:center">Qty</th>  
                        <th style="text-align:center">%</th>  
                         <th></th>
                    </tr>   
                    </thead>
                </HeaderTemplate>  
                <ItemTemplate>  
                    <tr>
                        <td style="font-size:small; text-align:center;"><%# Eval("S_N")%></td> 
                        <td style="font-size:small">
                            <div><%# Eval("Task") %></div>
                            <asp:Panel ID="PanelTaskInfo" runat="server" Visible='<%# !string.IsNullOrEmpty(Eval("Task_Info")?.ToString()) %>'>
                                <div>Task Info: <%# Eval("Task_Info") %></div>
                            </asp:Panel>
                        </td> 
                        <td style="font-size:small; text-align:center;"><%# Eval("Completed_Qty")%></td>  
                        <td style="font-size:small; text-align:center;"><%# Eval("Actual_Percent")%></td>  
                        <td style="font-size:small; text-align:center;"> <asp:LinkButton ID="ViewInvoice" runat="server" ><i class="fa fa-search" aria-hidden="true"></i></asp:LinkButton></td>  
                        <td id="ProjectScopeColumn" runat="server"><%# Eval("Project_Scope_Task_ID")%></td>  
                    </tr> 
                </ItemTemplate>  
            </asp:TemplateField>  
        </Columns>  

                </asp:GridView>
        </asp:Panel>
            </div>
        </div>
         <br><br><br><br>
          <div style="position: fixed; bottom: 0; left: 0; width: 100vw; background-color: white; border-top: 1px solid #ccc; padding: 10px;">
            <div style="display: flex; justify-content: space-between; align-items: center;">
                <div>
                    <asp:Button ID="btnToProject" runat="server" Text="Project" CausesValidation="false"  OnClick="btnToProject_Click" CssClass="btn btn-secondary" style="padding: 10px 20px;" />
                    <asp:Button ID="btnToScope" runat="server" Text="Scope" CausesValidation="false"  OnClick="btnToScope_Click" CssClass="btn btn-secondary" style="padding: 10px 20px;" />
                </div>
            </div>
        </div>
    </div>

    <script>


        document.addEventListener("DOMContentLoaded", function() {
            // Your code here to remove the <tr><th scope="col"></th></tr> elements
            var thRows = document.querySelectorAll('tr th[scope="col"]');
                thRows.forEach(function(th) {
                var tr = th.closest('tr');
                if (tr) {
                    tr.remove();
                }
            });
        });

        document.addEventListener("DOMContentLoaded", function () {
            // Select all <tr> elements
            var trElements = document.querySelectorAll('tr');

            // Loop through each <tr> element
            trElements.forEach(function (tr) {
                // Get all <td> elements inside the row
                var tdElements = tr.querySelectorAll('td');

                // Check if the row has only one <td> element (with or without content)
                if (tdElements.length === 1 && tdElements[0].textContent.trim() === '') {
                    tr.remove();  // Remove the entire <tr> if there's only one empty <td>
                }
            });
        });

        function toggleSearch() {
            var searchContainer = document.getElementById("search-container");
            var searchIcon = document.getElementById("search-icon");

            // Check if the search container is visible
            if (searchContainer.style.display === "none" || searchContainer.style.display === "") {
                // Show search fields
                searchContainer.style.display = "block";
                searchIcon.className = "fa fa-search-minus"; // Change icon to minus
            } else {
                // Hide search fields
                searchContainer.style.display = "none";
                searchIcon.className = "fa fa-search-plus"; // Change icon to plus
            }
        }

        window.onload = function () {
            // Get all rows of the GridView (excluding the header row)
            var rows = document.querySelectorAll('#<%= InvoiceGrid.ClientID %> tr');

            // Iterate through all rows, starting from 1 to skip the header row
            for (var i = 1; i < rows.length; i++) {
                var row = rows[i];

                // Get the third <td> in each row (index 2) which contains Actual_Percent
                var thirdCell = row.cells[2];

                // Check if third cell exists and contains a number
                if (thirdCell) {
                    var actualPercent = parseFloat(thirdCell.innerText || thirdCell.textContent);

                    // If the value is greater than 30, change the row's background color to green
                    if (actualPercent > 100) {
                        row.style.backgroundColor = 'green';
                        // Change text color to white for better contrast
                        row.style.color = 'white';
                    }
                }
            }
        };

    </script>


</asp:Content>
