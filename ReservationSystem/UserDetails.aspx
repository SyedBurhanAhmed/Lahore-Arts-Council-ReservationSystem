<%@ Page Title="UserDetails" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="UserDetails.aspx.cs" Inherits="ReservationSystem.UserDetails" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container">
        <h2 class="text-center font-weight-bold text-white">Your Booking Details</h2> <!-- Center the heading -->

        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <div class="row">
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

                    <div class="col-md-6">
                        <label class="form-label font-weight-bold text-white">Full Name</label>
                        <asp:Label ID="lblFullName" runat="server" CssClass="form-control" />
                    </div>

                    <div class="col-md-6">
                        <label class="form-label font-weight-bold text-white">Father's Name</label>
                        <asp:Label ID="lblFatherName" runat="server" CssClass="form-control" />
                    </div>

                    <div class="col-md-6">
                        <label class="form-label font-weight-bold text-white">CNIC</label>
                        <asp:Label ID="lblCNIC" runat="server" CssClass="form-control" />
                    </div>

                    <div class="col-md-6">
                        <label class="form-label font-weight-bold text-white">Phone Number</label>
                        <asp:Label ID="lblPhoneNumber" runat="server" CssClass="form-control" />
                    </div>

                    <div class="col-md-6">
                        <label class="form-label font-weight-bold text-white">Email</label>
                        <asp:Label ID="lblEmail" runat="server" CssClass="form-control" />
                    </div>

                    <div class="col-md-6">
                        <label class="form-label font-weight-bold text-white">Event Topic</label>
                        <asp:TextBox ID="txtTopic" runat="server" Placeholder="Enter your Topic" CssClass="form-control" />
                    </div>

                    <div class="col-md-6">
                        <label class="form-label font-weight-bold text-white">Attach Application Screenshot</label>
                        <asp:FileUpload ID="fuApplicationForm" runat="server" CssClass="form-control" />
                    </div>

                    <asp:Label ID="lblErrorMessage" runat="server" CssClass="error-message text-center" Visible="False"></asp:Label>

                    <div class="col-12 mb-3 mt-3">
                        <asp:Button ID="btnSubmit" Text="Reserve" runat="server" CssClass="btn btn-danger" OnClick="btnSubmit_Click" />
                    </div>
                </div>
            </ContentTemplate>

            <Triggers>
                <asp:PostBackTrigger ControlID="btnSubmit" />
            </Triggers>
        </asp:UpdatePanel>
    </div>
</asp:Content>
