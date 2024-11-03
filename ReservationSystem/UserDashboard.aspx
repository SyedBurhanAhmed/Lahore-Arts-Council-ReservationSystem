<%@ Page Title="User Dashboard" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="UserDashboard.aspx.cs" Inherits="ReservationSystem.UserDashboard" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container text-center">
        <h2 class="text-center">User Dashboard</h2>

        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <asp:Label ID="lblNoBookings" runat="server" CssClass="success-message text-center" Visible="False"></asp:Label>

                <!-- Repeater control to generate individual grids for each booking -->
                <asp:Repeater ID="rptBookings" runat="server">
                    <ItemTemplate>
                        <div class="table-responsive">
                            <table class="table table-striped mx-auto">
                                <thead>
                                    <tr>
                                        <th>Booking ID</th>
                                        <th>Complex Name</th>
                                        <th>Facility Name</th>
                                        <th>Topic</th>
                                        <th>First Booking Date</th>
                                        <th>Last Booking Date</th>
                                        <th>Start Time</th>
                                        <th>End Time</th>
                                        <th>Application Status</th>
                                        <th>Security Fee Submission Status</th>
                                        <th>Security Fee Approval Status</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <tr>
                                        <td><%# Eval("BookingID") %></td>
                                        <td><%# Eval("ComplexName") %></td>
                                        <td><%# Eval("FacilityName") %></td>
                                        <td><%# Eval("Topic") %></td>
                                        <td><%# Eval("FirstBookingDate", "{0:dd-MM-yyyy}") %></td>
                                        <td><%# Eval("LastBookingDate", "{0:dd-MM-yyyy}") %></td>
                                        <td><%# Eval("StartTime", "{0:hh\\:mm}") %></td>
                                        <td><%# Eval("EndTime", "{0:hh\\:mm}") %></td>

                                        <!-- Application Status -->
                                        <td>
                                            <asp:Label ID="lblApplicationStatus" runat="server" 
                                                       Text='<%# Eval("IsBookingApproved").ToString() == "1" ? "Approved" : Eval("IsBookingApproved").ToString() == "-1" ? "Denied" : "Pending" %>'>
                                            </asp:Label>
                                        </td>

                                        <!-- Security Fee Status -->
                                        <td>
                                            <asp:Label ID="lblSecurityFeeStatus" runat="server" 
                                                       Text='<%# Eval("IsBookingApproved").ToString() == "1" ? (string.IsNullOrEmpty(Eval("SecurityFeePath").ToString()) ? "Pending" : "Submitted") : "" %>'>
                                            </asp:Label>
                                        </td>
                                        <td>
                                            <asp:Label ID="lblSecurityFeeApprovalStatus" runat="server" 
                                                       Text='<%# !string.IsNullOrEmpty(Eval("SecurityFeePath").ToString()) ? (Eval("IsSecurityFeeApproved").ToString() == "1" ? "Approved" : Eval("IsSecurityFeeApproved").ToString() == "-1" ? "Denied" : "Pending") : "" %>'>
                                            </asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="10" class="text-left">
                                            <asp:Label ID="lblMessage" runat="server" 
                                                Text='<%# Eval("IsBookingApproved").ToString() == "0" ? "Booking approval pending" 
                                                       : Eval("IsBookingApproved").ToString() == "-1" ? "Booking denied" 
                                                       : (!string.IsNullOrEmpty(Eval("SecurityFeePath").ToString()) && Eval("IsSecurityFeeApproved").ToString() == "0") ? "Security Fee Approval Pending"
                                                       : Eval("IsSecurityFeeApproved").ToString() == "1" ? "Submit the total charges as a pay order to Lahore Arts Council. Total Charges: " + GetTotalCharges(Eval("BookingID")) 
                                                       : Eval("IsSecurityFeeApproved").ToString() == "-1" ? "Booking denied" 
                                                       : "" %>'>
                                            </asp:Label>


                                            <!-- Submit Security Fee Button -->
                                            <asp:Button ID="btnSubmitSecurityFee" runat="server" Text="Submit Security Fee" 
                                                        CommandName="SubmitSecurityFee" 
                                                        CommandArgument='<%# Eval("BookingID").ToString() %>' 
                                                        CssClass="btn btn-primary btn-sm" 
                                                        style="float: left;"  
                                                        Visible='<%# Eval("IsBookingApproved").ToString() == "1" && string.IsNullOrEmpty(Eval("SecurityFeePath").ToString()) && Eval("IsSecurityFeeApproved").ToString() == "0" %>' />
                                        </td>
                                     </tr>

                                </tbody>
                            </table>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="rptBookings" EventName="ItemCommand" />
            </Triggers>
        </asp:UpdatePanel>

        <asp:Label ID="lblError" runat="server" CssClass="success-message text-center" Visible="False"></asp:Label>

        <div class="text-right">
            <asp:Button ID="btnLogout" runat="server" Text="Logout" CssClass="btn btn-danger" OnClick="btnLogout_Click" />
        </div>

        <!-- Position of the button in the markup -->
        <div class="text-left" id="bottomButton">
            <asp:Button ID="btnBookSlot" runat="server" Text="Book Another Slot" CssClass="btn btn-primary" OnClick="btnBookSlot_Click" Visible="True" />
        </div>

    </div>

    <style>
        .text-right {
            text-align: right;
            margin-bottom: 15px;
        }

        .text-left {
            text-align: left;
            margin-bottom: 15px;
        }

        /* Makes each booking's table and container centered */
        .container {
            margin-top: 20px;
        }

        /* Styles to ensure the Book Another Slot button stays at the bottom */
        #bottomButton {
            margin-top: 500px; /* Ensure there's at least 500px space below the last booking */
            position: relative;
            width: 100%;
            text-align: left;
        }
    </style>
</asp:Content>
