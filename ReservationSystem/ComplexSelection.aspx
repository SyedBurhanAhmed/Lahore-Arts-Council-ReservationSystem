<%@ Page Title="ComplexSelection" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ComplexSelection.aspx.cs" Inherits="ReservationSystem.ComplexSelection" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <div class="selection-container">
        <h2 class="text-center mb-4 font-weight-bold text-white">Select a Complex</h2>
        
        <div class="form-container">
            <asp:RadioButtonList ID="rblComplex" runat="server" AutoPostBack="true" OnSelectedIndexChanged="rblComplex_SelectedIndexChanged" CssClass="form-check">
                <asp:ListItem Text="Mall Complex" Value="Mall Complex" />
                <asp:ListItem Text="Cultural Complex" Value="Cultural Complex" />
            </asp:RadioButtonList>
        </div>
    </div>

    <style>
        /* Center the entire selection container */
        .selection-container {
            display: flex;
            flex-direction: column;
            justify-content: center; /* Center vertically */
            align-items: center; /* Center horizontally */
            min-height: 100vh; /* Full viewport height */
            text-align: center; /* Center text */
        }

        .form-container {
            display: flex;
            flex-direction: column; /* Vertical layout */
            align-items: center; /* Center horizontally */
            padding: 20px;
            background: transparent; /* Transparent background */
        }

        h2 {
            margin-bottom: 20px; /* Space below the heading */
            color: white; /* Make the heading white */
        }

        .form-check {
            display: flex;
            flex-direction: column; /* Vertical layout for radio buttons */
            align-items: center;
            margin-bottom: 20px;
            max-width: 600px; /* Set max width for the options */
        }

        .form-check input[type="radio"] {
            display: none; /* Hide the actual radio buttons */
        }

        .form-check label {
            display: block;
            width: 100%; /* Full width for radio button labels */
            padding: 15px;
            margin-bottom: 15px; /* Space between options */
            border: 1px solid #ccc;
            border-radius: 5px;
            background-color: rgba(255, 255, 255, 0.8);
            cursor: pointer; /* Hand cursor on hover */
            transition: background-color 0.3s ease, border-color 0.3s ease;
            text-align: center;
            font-size: 1.2rem;
        }

        .form-check input[type="radio"]:checked + label {
            background-color: rgba(0, 123, 255, 0.1); /* Blue background for selected option */
            border-color: #007bff;
        }

        .form-check label:hover {
            background-color: rgba(0, 123, 255, 0.1); /* Light blue background on hover */
        }
    </style>
</asp:Content>
