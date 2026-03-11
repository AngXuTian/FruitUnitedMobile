<%@ Page Title="" Language="C#" MasterPageFile="~/ModulesPage.Master" AutoEventWireup="true" CodeBehind="UploadedImages.aspx.cs" Inherits="FruitUnitedMobile.Modules.UploadedImages" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        /* Gallery container */
        .image-gallery {
            display: flex;
            flex-wrap: wrap;
            gap: 15px;
            justify-content: center;
            margin: 20px 0;
        }

        /* Gallery item style */
        .image-item {
            width: 100%;
            max-width: 300px;
            box-sizing: border-box;
            margin: 0 auto;
        }

        /* Image style */
        .image-item img {
            width: 100%;
            height: auto;
            object-fit: cover;
            border-radius: 8px;
            border: 1px solid #ddd;
            box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
            transition: transform 0.3s ease, box-shadow 0.3s ease;
            cursor: pointer; /* Change cursor to pointer on hover */
        }

        /* Hover effect */
        .image-item img:hover {
            transform: scale(1.05);
            box-shadow: 0 6px 10px rgba(0, 0, 0, 0.15);
        }

        /* Modal background */
        .modal {
            display: none; /* Hidden by default */
            position: fixed;
            z-index: 1000;
            left: 0;
            top: 0;
            width: 100%;
            height: 100%;
            background-color: rgba(0, 0, 0, 0.8);
            justify-content: center;
            align-items: center;
        }

        /* Modal content */
        /* Style for the modal image */
.modal-content {
    width: auto;   /* Adjust width automatically */
    height:auto;
    max-width: 90%; /* Maximum width (optional, for responsiveness) */
    max-height: 80vh; /* Limit the height to 80% of the viewport height */
    margin: auto;
    display: block;
}


        /* Close button */
        .close {
            position: absolute;
            top: 20px;
            right: 20px;
            color: #fff;
            font-size: 30px;
            font-weight: bold;
            cursor: pointer;
        }

        /* Mobile-first design with media queries */
        @media (max-width: 1024px) {
            .image-item {
                max-width: 220px;
            }
        }

        @media (max-width: 768px) {
            .image-item {
                max-width: 200px;
            }
        }

        @media (max-width: 480px) {
            .image-item {
                max-width: 180px;
            }

            .image-gallery {
                gap: 10px;
            }
        }

        h2 {
            text-align: center;
            font-size: 28px;
            color: #333;
            margin-bottom: 20px;
        }
    </style>
        <link href="../CSS/Basic.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
   <div>
        <h2>Uploaded Images</h2>
        <div class="image-gallery">
           <asp:Repeater ID="imageRepeater" runat="server">
    <ItemTemplate>
        <div class="image-item">
            <!-- Pass the correct file path to openModal -->
            <img src='<%# ResolveUrl(Container.DataItem.ToString()) %>' alt="Uploaded Image"
                 onclick="openModal('<%# ResolveUrl(Container.DataItem.ToString()) %>')" />
        </div>
    </ItemTemplate>
</asp:Repeater>

        </div>
       <div class="container" style="text-align:center;">
       <asp:Label ID="alert" runat="server" class="alert alert-danger"  role="alert">No images found for this project! </asp:Label>
        </div>
    </div>
      <br><br><br><br>
        <div style="position: fixed; bottom: 0; left: 0; width: 100vw; background-color: white; border-top: 1px solid #ccc; padding: 10px;">
            <div style="display: flex; justify-content: space-between; align-items: center;">
                <div>
                    <asp:Button ID="btnToProject" runat="server" Text="Project" CausesValidation="false"  OnClick="btnToProject_Click" CssClass="btn btn-secondary" style="padding: 10px 20px;" />
                    <asp:Button ID="btnToScope" runat="server" Text="Scope" CausesValidation="false"  OnClick="btnToScope_Click" CssClass="btn btn-secondary" style="padding: 10px 20px;" />
                    <asp:Button ID="btnToTask" runat="server" Text="Task" CausesValidation="false"  OnClick="btnToTask_Click" CssClass="btn btn-secondary" style="padding: 10px 20px;" />
                </div>
            </div>
        </div>

    <!-- Modal -->
    <div id="myModal" class="modal">
        <span class="close" onclick="closeModal()">&times;</span>
        <img class="modal-content" id="modalImage" />
    </div>

     <script>
        // Open modal with image
         function openModal(imageSrc) {
             // Ensure that the image source is correctly passed and decoded
             document.getElementById("myModal").style.display = "flex"; // Show modal
             document.getElementById("modalImage").src = imageSrc; // Set image source directly without decoding
         }


        // Close modal
        function closeModal() {
            document.getElementById("myModal").style.display = "none"; // Hide modal
        }

         window.onpageshow = function (event) {
             if (event.persisted || (window.performance && window.performance.navigation.type === 2)) {
                 window.location.reload();
             }
         };

     </script>
</asp:Content>
