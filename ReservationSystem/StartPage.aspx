<%@ Page Title="Lahore Arts Council Reservation System" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="StartPage.aspx.cs" Inherits="ReservationSystem.StartPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container text-center mt-5">
        <h1 class="mb-4 font-weight-bold text-white">Lahore Arts Council Reservation System</h1>
        <a href="ComplexSelection.aspx" class="btn btn-primary btn-lg px-4 py-2">Book Now</a>
    </div>

    <style>
        .container {
            padding: 80px; /* Add padding around the container */
        }
        
        h1 {
            font-size: 2.5rem; /* Larger font size for the heading */
        }

        .btn-primary {
            padding: 40px;
            font-size: 1.25rem; /* Increase button font size */
        }
    </style>
</asp:Content>
