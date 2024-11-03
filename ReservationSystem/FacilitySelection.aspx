<%@ Page Title="FacilitySelection" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="FacilitySelection.aspx.cs" Inherits="ReservationSystem.FacilitySelection" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container mt-4">
        <h2 class="text-center mb-4 font-weight-bold text-white">Select a Facility</h2>
        
        <!-- Facility Dropdown -->
        <asp:DropDownList ID="ddlFacilities" runat="server" CssClass="form-control form-select" AutoPostBack="True" OnSelectedIndexChanged="ddlFacilities_SelectedIndexChanged" />
        
        <!-- Facility Details Display -->
        <div id="facilityDetails" class="container mt-4">
            <asp:Label ID="lblCapacity" runat="server" Text="Capacity: "></asp:Label><br />
            <asp:Label ID="lblRatePerHour" runat="server" Text="Rate per Hour: "></asp:Label><br />
            <asp:Label ID="lblSecurityFees" runat="server" Text="Security Fees: "></asp:Label>
        </div>

        <!-- Error message label (for no facility selected) -->
        <asp:Label ID="lblErrorMessage" runat="server" CssClass="error-message text-center" Visible="False"></asp:Label>

        <!-- Next Button -->
        <asp:Button ID="btnNext" Text="Next" runat="server" CssClass="btn btn-primary" OnClick="btnNext_Click" />
    </div>

    <style>
       
        .container {
            max-width: 600px; /* Limit the container width for better appearance */
            margin: 0 auto; /* Center the container */
            padding: 20px; /* Add padding around the container */
        }

        h2 {
            margin-bottom: 30px; /* Increased space below the heading */
        }

        .form-control {
            margin-bottom: 20px; /* Space below the dropdown */
        }

        .btn-primary {
            width: 100%; /* Make the button full width */
            padding: 10px; /* Add padding for a better touch target */
        }

        #facilityDetails {
            background-color: rgba(255, 255, 255, 0.2); /* Light, semi-transparent background */
            padding: 10px;
            border-radius: 5px;
            width: 300px; /* Set a smaller width */
            margin: 0 auto 50px auto; /* Increased margin below for more space */
        }

        #facilityDetails p {
            margin: 5px 0; /* Space between details */
            font-size: 1em;
        }
    </style>
</asp:Content>
