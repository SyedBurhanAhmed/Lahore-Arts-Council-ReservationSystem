<%@ Page Title="Admin Dashboard" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="AdminDashboard.aspx.cs" Inherits="ReservationSystem.AdminDashboard" Async="true" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container">
        <h2 class="text-center">Admin Dashboard</h2>
        <asp:UpdatePanel ID="UpdatePanel1" runat="server" ChildrenAsTriggers="true">
            <ContentTemplate>
                <asp:GridView ID="gvBookings" runat="server" AutoGenerateColumns="False" CssClass="table table-striped" 
                    DataKeyNames="BookingID" OnRowCommand="gvBookings_RowCommand" OnRowDataBound="gvBookings_RowDataBound">
                    <Columns>
                        <asp:BoundField DataField="BookingID" HeaderText="Booking ID" />
                        <asp:BoundField DataField="Name" HeaderText="Full Name" />
                        <asp:BoundField DataField="CNIC" HeaderText="CNIC" />
                        <asp:BoundField DataField="ComplexName" HeaderText="Complex Name" /> 
                        <asp:BoundField DataField="FacilityName" HeaderText="Facility Name" />
                        <asp:BoundField DataField="Topic" HeaderText="Topic" /> 
                        <asp:BoundField DataField="FirstBookingDate" HeaderText="First Booking Date" />
                        <asp:BoundField DataField="LastBookingDate" HeaderText="Last Booking Date" />
                        <asp:BoundField DataField="StartTime" HeaderText="Start Time" />
                        <asp:BoundField DataField="EndTime" HeaderText="End Time" />

                        <asp:TemplateField HeaderText="Actions">
                            <ItemTemplate>
                                <asp:Label ID="lblBookingStatus" runat="server" CssClass="status-label" Text="" Visible="false"></asp:Label>
                                <br />
                                <asp:LinkButton ID="btnViewForm" runat="server" Text="View Application Form" CommandName="ViewApplicationForm" 
                                    CommandArgument='<%# Eval("BookingID") %>' CssClass="btn btn-info action-button" Visible="false" />
                                <asp:LinkButton ID="btnApprove" runat="server" Text="Approve Booking" CommandName="Approve" 
                                    CommandArgument='<%# Eval("BookingID") %>' CssClass="btn btn-success action-button" Visible="false" />
                                <asp:LinkButton ID="btnDisapprove" runat="server" Text="Disapprove Booking" CommandName="Disapprove" 
                                    CommandArgument='<%# Eval("BookingID") %>' CssClass="btn btn-danger action-button" Visible="false" />
                                <asp:LinkButton ID="btnViewSecurityFee" runat="server" Text="View Security Fee" CommandName="ViewSecurityFee" 
                                    CommandArgument='<%# Eval("BookingID") %>' CssClass="btn btn-info action-button" Visible="false" />
                                <asp:LinkButton ID="btnApproveSecurityFee" runat="server" Text="Approve Security Fee" CommandName="ApproveSecurityFee" 
                                    CommandArgument='<%# Eval("BookingID") %>' CssClass="btn btn-success action-button" Visible="false" />
                                <asp:LinkButton ID="btnDenySecurityFee" runat="server" Text="Disapprove Security Fee" CommandName="DenySecurityFee" 
                                    CommandArgument='<%# Eval("BookingID") %>' CssClass="btn btn-danger action-button" Visible="false" />
                            </ItemTemplate>
                        </asp:TemplateField>

                    </Columns>
                </asp:GridView>

                <asp:Label ID="lblNoBookings" runat="server" CssClass="success-message text-center" Visible="False" Text="No booking requests available." />
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="gvBookings" EventName="RowCommand" />
            </Triggers>
        </asp:UpdatePanel>
        <asp:Label ID="lblMessage" runat="server" CssClass="success-message text-center" Visible="False"></asp:Label>
        
        <asp:Button ID="btnClearBookings" runat="server" Text="Clear Denied Bookings" CssClass="btn btn-primary" OnClick="btnClearBookings_Click" />
        
        <div class="text-left">
            <asp:LinkButton ID="btnLogout" runat="server" Text="Logout" CssClass="btn btn-danger" OnClick="btnLogout_Click" />
        </div>
    </div>
    <style>
        .text-left {
            text-align: left;
            margin-top: 25px;
            margin-bottom: 15px; /* Space below the LinkButton */
        }
        .status-label {
            display: block;
            margin-bottom: 10px; /* Space between the label and buttons */
        }
        .action-button {
            margin-bottom: 5px; /* Space between each button */
            margin-right: 5px; /* Space between buttons on the same row */
        }
    </style>
</asp:Content>
