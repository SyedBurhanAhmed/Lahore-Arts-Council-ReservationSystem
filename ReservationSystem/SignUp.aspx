<%@ Page Title="SignUp" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="SignUp.aspx.cs" Inherits="ReservationSystem.SignUp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container d-flex justify-content-center align-items-center min-vh-100">
        <div class="signup-box">
            <h2 class="text-center font-weight-bold">Sign Up</h2>
            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                <ContentTemplate>
                    <div class="row">
                        <div class="col-md-6">
                            <label class="form-label font-weight-bold">Full Name:</label>
                            <asp:TextBox ID="txtName" runat="server" Placeholder="Enter your name" CssClass="form-control" />
                        </div>

                        <div class="col-md-6">
                            <label class="form-label font-weight-bold">Father's Name:</label>
                            <asp:TextBox ID="txtFatherName" runat="server" Placeholder="Enter father's name" CssClass="form-control" />
                        </div>

                        <div class="col-md-6">
                            <label class="form-label font-weight-bold">CNIC:</label>
                            <asp:TextBox ID="txtCNIC" runat="server" Placeholder="xxxxx-xxxxxxx-x" CssClass="form-control" />
                        </div>

                        <div class="col-md-6">
                            <label class="form-label font-weight-bold">Phone Number:</label>
                            <asp:TextBox ID="txtPhoneNumber" runat="server" Placeholder="Enter phone number" CssClass="form-control" />
                        </div>

                        <div class="col-md-6">
                            <label class="form-label font-weight-bold">Email:</label>
                            <asp:TextBox ID="txtEmail" runat="server" Placeholder="Enter email" CssClass="form-control" TextMode="Email" />
                        </div>

                        <div class="col-md-6">
                            <label class="form-label font-weight-bold">Password:</label>
                            <div class="input-group">
                                <asp:TextBox ID="txtPassword" runat="server" CssClass="form-control" TextMode="Password" Placeholder="Enter password" />
                                <div class="input-group-append">
                                    <button class="btn btn-outline-secondary toggle-btn" type="button" onclick="togglePassword('<%= txtPassword.ClientID %>')">Show</button>
                                </div>
                            </div>
                        </div>

                        <div class="col-md-6">
                            <label class="form-label font-weight-bold">Confirm Password:</label>
                            <div class="input-group">
                                <asp:TextBox ID="txtConfirmPassword" runat="server" CssClass="form-control" TextMode="Password" Placeholder="Confirm password" />
                                <div class="input-group-append">
                                    <button class="btn btn-outline-secondary toggle-btn" type="button" onclick="togglePassword('<%= txtConfirmPassword.ClientID %>')">Show</button>
                                </div>
                            </div>
                        </div>

                        <div class="col-12 mt-3">
                            <asp:Button ID="btnSignUp" runat="server" CssClass="btn btn-primary btn-block" Text="Register" OnClick="btnSignUp_Click" />
                        </div>

                        <div class="col-12 mt-3 text-center">
                            <asp:Label ID="lblErrorMessage" runat="server" CssClass="error-message text-center" Visible="False"></asp:Label>
                            <asp:Label ID="lblLogin" runat="server" Text="Already registered?" CssClass="form-label font-weight-bold" />
                            <br />
                            <asp:HyperLink ID="lnkLogin" runat="server" CssClass="btn btn-link" NavigateUrl="~/Login.aspx" Text="Go to Login" />
                        </div>
                    </div>
                </ContentTemplate>

                <Triggers>
                    <asp:PostBackTrigger ControlID="btnSignUp" />
                </Triggers>
            </asp:UpdatePanel>
        </div>
    </div>

    <script type="text/javascript">
        function togglePassword(fieldID) {
            var field = document.getElementById(fieldID);
            if (field.type === "password") {
                field.type = "text";
            } else {
                field.type = "password";
            }
        }
    </script>

    <style>
        .signup-box {
            width: 800px; /* Set the width of the box */
            padding: 40px;
            background-color: rgba(255, 255, 255, 0.1); /* Set background to be transparent */
            text-align: left;
            border-radius: 10px;
            box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
        }

    </style>
</asp:Content>
