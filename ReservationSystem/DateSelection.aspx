<%@ Page Title="DateSelection" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="DateSelection.aspx.cs" Inherits="ReservationSystem.DateSelection" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container mt-4">
        <!-- Heading with updated styles -->
        <h2 class="text-center mb-4 font-weight-bold heading-style">Select Booking Dates and Times</h2>

        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <!-- Row for date inputs -->
                <div class="row">
                    <!-- First Booking Date -->
                    <div class="col-md-6 form-group mb-3">
                        <label for="txtFirstBookingDate" class="form-label font-weight-bold text-white">First Booking Date:</label>
                        <asp:TextBox ID="txtFirstBookingDate" runat="server" CssClass="form-control" 
                            placeholder="yyyy-MM-dd" />
                    </div>

                    <!-- Last Booking Date -->
                    <div class="col-md-6 form-group mb-3">
                        <label for="txtLastBookingDate" class="form-label font-weight-bold text-white">Last Booking Date:</label>
                        <asp:TextBox ID="txtLastBookingDate" runat="server" CssClass="form-control" 
                            placeholder="yyyy-MM-dd" />
                    </div>
                </div>

                <!-- Start Time -->
                <div class="form-group">
                    <label for="ddlStartTime" class="form-label font-weight-bold text-white">Start Time:</label>
                    <asp:DropDownList ID="ddlStartTime" runat="server" CssClass="form-control form-select" 
                        AutoPostBack="true" OnSelectedIndexChanged="ValidateTimes">
                        <asp:ListItem Text="Select Start Time" Value="" />
                    </asp:DropDownList>
                </div>

                <!-- End Time -->
                <div class="form-group">
                    <label for="ddlEndTime" class="form-label font-weight-bold text-white">End Time:</label>
                    <asp:DropDownList ID="ddlEndTime" runat="server" CssClass="form-control form-select" 
                        AutoPostBack="true" OnSelectedIndexChanged="ValidateTimes">
                        <asp:ListItem Text="Select End Time" Value="" />
                    </asp:DropDownList>
                </div>

                <!-- Error Message and Button -->
                <asp:Label ID="lblErrorMessage" runat="server" CssClass="error-message text-center" Visible="False"></asp:Label>
                <asp:Button ID="btnNext" Text="Next" runat="server" CssClass="btn btn-primary mt-3" OnClick="btnNext_Click" />
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="ddlStartTime" EventName="SelectedIndexChanged" />
                <asp:AsyncPostBackTrigger ControlID="ddlEndTime" EventName="SelectedIndexChanged" />
                <asp:AsyncPostBackTrigger ControlID="btnNext" EventName="Click" />
            </Triggers>
        </asp:UpdatePanel>
    </div>

    <script type="text/javascript">
        // Initialize flatpickr for the booking date textboxes
        function initializeFlatpickr() {
            const minDate = new Date();
            minDate.setDate(minDate.getDate() + 7);
            flatpickr("#<%= txtFirstBookingDate.ClientID %>", {
                dateFormat: "Y-m-d",
                enableTime: false,
                static: true,
                minDate: minDate
            });
            flatpickr("#<%= txtLastBookingDate.ClientID %>", {
                dateFormat: "Y-m-d",
                enableTime: false,
                static: true,
                minDate: minDate
            });
        }

        // Call this function on page load and after async postbacks
        Sys.Application.add_load(initializeFlatpickr);
    </script>

    <style>
        body {
            margin: 0; /* Remove default body margin */
            padding: 0; /* Remove default body padding */
        }

        /* Heading style */
        .heading-style {
             /* Increase font size */
            padding: 30px 0; /* Add padding for space */
            color: white;
        }

        .form-label {
            color: #fff; /* White color for labels */
            font-weight: bold; /* Bold labels */
        }

        .btn-primary {
            background-color: #007bff; /* Bootstrap primary color */
            border: none;
        }

        .btn-primary:hover {
            background-color: #0056b3; /* Darker shade on hover */
        }
    </style>
</asp:Content>
