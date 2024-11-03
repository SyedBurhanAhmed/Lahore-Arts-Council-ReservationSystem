<%@ Page Title="Submit Security Fee" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="SubmitSecurityFee.aspx.cs" Inherits="ReservationSystem.SubmitSecurityFee" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container">
        <h2 class="text-center font-weight-bold text-white">Submit Security Fee</h2> <!-- Center the heading -->

        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <div class="row">
                    <div class="col-md-12 mt-4">
                        <asp:Label ID="lblMessage" runat="server" CssClass="success-message" Visible="False"></asp:Label>
                    </div>

                    <div class="col-md-12">
                        <h4 class="text-white">Your booking details are as follows:</h4>
                    </div>

                    <div class="col-md-6">
                        <label class="form-label font-weight-bold text-white">Event Topic:</label>
                        <asp:Label ID="lblTopic" runat="server" CssClass="form-control" />
                    </div>

                    <div class="col-md-6">
                        <label class="form-label font-weight-bold text-white">First Booking Date:</label>
                        <asp:Label ID="lblFirstBookingDate" runat="server" CssClass="form-control" />
                    </div>

                    <div class="col-md-6">
                        <label class="form-label font-weight-bold text-white">Last Booking Date:</label>
                        <asp:Label ID="lblLastBookingDate" runat="server" CssClass="form-control" />
                    </div>

                    <div class="col-md-6">
                        <label class="form-label font-weight-bold text-white">Time Slot:</label>
                        <asp:Label ID="lblTimeSlot" runat="server" CssClass="form-control" />
                    </div>

                    <div class="col-md-12 mt-4">
                        <h4 class="text-white">Security Fee:</h4>
                        <asp:Label ID="lblSecurityFee" runat="server" CssClass="font-weight-bold text-white total-charges" />
                    </div>

                    <div class="col-md-12 mt-4">
                        <h4 class="text-white">Upload Security Fee Screenshot</h4>
                        <asp:FileUpload ID="fileSecurityFee" runat="server" CssClass="form-control" />
                    </div>


                    <div class="col-md-12 mt-3">
                        <asp:Button ID="btnSubmit" runat="server" Text="Submit" OnClick="btnSubmit_Click" CssClass="btn btn-danger" />
                    </div>

                    <div class="col-md-12 mt-4">
                        <h4 class="text-white">Total Charges:</h4>
                        <asp:Label ID="lblTotalCharges" runat="server" CssClass="font-weight-bold text-white total-charges" />
                        <p class="text-white"></p>
                    </div>

                    

                  
                </div>
            </ContentTemplate>

            <Triggers>
                <asp:PostBackTrigger ControlID="btnSubmit" />
            </Triggers>
        </asp:UpdatePanel>
    </div>

    <style>
        /* Centered general container */
        .container {
            max-width: 800px;
            margin: 0 auto;
            padding: 10px;
        }

        /* Style for the total charges label */
        .total-charges {
            font-size: 18px;
            font-weight: bold;
        }

        /* Optional: you can modify this to fit your overall design */
        .text-white {
            color: white !important; /* Ensure text is always white */
        }
    </style>
</asp:Content>
