<%@ Page Title="Login" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="ReservationSystem.Login" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container d-flex justify-content-center align-items-center min-vh-100">
        <div class="login-box">
            <h2 class="text-center font-weight-bold">Login</h2>
            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                <ContentTemplate>
                    <div class="row">
                        <div class="col-md-12">
                            <label class="form-label font-weight-bold"></label>
                            <asp:TextBox ID="txtEmail" runat="server" Placeholder="Email" CssClass="form-control" TextMode="Email" />
                        </div>

                        <div class="col-md-12">
                            <label class="form-label font-weight-bold"></label>
                            <div class="input-group">
                                <asp:TextBox ID="txtPassword" runat="server" CssClass="form-control" TextMode="Password" Placeholder="Password" />
                                <div class="input-group-append">
                                    <button class="btn btn-outline-secondary toggle-btn" type="button" onclick="togglePassword('<%= txtPassword.ClientID %>')">Show</button>
                                </div>
                            </div>
                        </div>

                        <div class="col-12 mt-3">
                            <asp:Button ID="btnLogin" runat="server" CssClass="btn btn-primary btn-block" Text="Login" OnClick="btnLogin_Click" />
                        </div>

                        <div class="col-12 mt-2 text-center">
                            <asp:Label ID="lblErrorMessage" runat="server" CssClass="error-message text-center" Visible="False"></asp:Label>
                        </div>

                        <div class="col-12 mt-2 text-center">
                            <asp:Label ID="lblSignup" runat="server" Text="Not registered yet?" CssClass="form-label font-weight-bold" />
                            <br />
                            <asp:HyperLink ID="lnkSignUp" runat="server" CssClass="btn btn-link signup-link" NavigateUrl="~/SignUp.aspx" Text="Sign Up Here!" />
                        </div>

                        <!-- Admin Access Button -->
                        <div class="col-12 mt-2 text-center">
                            <asp:Label ID="lblAdminAccess" runat="server" Text="Click Here for Admin Access:" CssClass="form-label font-weight-bold" />
                            <br />
                            <asp:Button ID="btnAdminAccess" runat="server" CssClass="btn btn-secondary" Text="Admin Access" OnClick="btnAdminAccess_Click" />
                        </div>
                    </div>
                </ContentTemplate>

                <Triggers>
                    <asp:PostBackTrigger ControlID="btnLogin" />
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
        .login-box {
            width: 400px; /* Set the width of the box */
            padding: 40px;
            text-align: center;
            border-radius: 10px;
            box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
            background-color: rgba(255, 255, 255, 0.1);
        }
    </style>
</asp:Content>
