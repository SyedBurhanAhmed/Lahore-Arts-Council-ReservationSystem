<%@ Page Title="Confirmation" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Confirmation.aspx.cs" Inherits="ReservationSystem.Confirmation" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <!-- Centered container for the success message -->
    <div class="container text-center">
        <h2>Application Submitted!</h2>
    </div>

    <!-- Container for booking details with whitish transparency -->
    <div class="container mt-3" id="bookingDetails">
        <p>Your booking has been successfully submitted. Here are your booking details:</p>
        
        <p><strong>Full Name: </strong><asp:Label ID="lblName" runat="server" CssClass="font-weight-bold" /></p>
        <p><strong>Event Topic: </strong><asp:Label ID="lblTopic" runat="server" CssClass="font-weight-bold" /></p>
        <p><strong>Facility Name: </strong><asp:Label ID="lblFacilityName" runat="server" CssClass="font-weight-bold" /></p>
        <p><strong>Complex Name: </strong><asp:Label ID="lblComplexName" runat="server" CssClass="font-weight-bold" /></p>
        <p><strong>First Booking Date: </strong><asp:Label ID="lblFirstBookingDate" runat="server" CssClass="font-weight-bold" /></p>
        <p><strong>Last Booking Date: </strong><asp:Label ID="lblLastBookingDate" runat="server" CssClass="font-weight-bold" /></p>
        <p><strong>Time Slot: </strong><asp:Label ID="lblTimeSlot" runat="server" CssClass="font-weight-bold" /></p>
        <!-- Button to reserve another slot -->
        <asp:Button ID="btnReserveAnother" runat="server" Text="Return to Dashboard" CssClass="btn btn-primary mt-4" OnClick="btnDashboard_Click" />
    </div>

    <style>
        /* Styles for the booking details container */
        #bookingDetails {
            background-color: rgba(255, 255, 255, 0.2); /* Whitish transparent background */
            padding: 20px;
            border-radius: 8px;
            width: 100%; /* Full width container */
            max-width: 600px; /* Limit container width */
            margin: 10px auto; /* Reduced margin for less space */
        }

        /* General container styling */
        .container {
            max-width: 600px; /* Limit the container width for better appearance */
            margin: 0 auto; /* Center the container */
            padding: 10px; /* Add padding around the container */
        }

        /*h2 {
            margin-bottom: 10px;*/ /* Reduced space below the heading */
        /*}*/

        .btn-primary {
            width: 100%; /* Make the button full width */
            padding: 10px; /* Add padding for a better touch target */
        }
    </style>
</asp:Content>
