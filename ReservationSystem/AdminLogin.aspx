<%@ Page Title="Admin Login" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="AdminLogin.aspx.cs" Inherits="ReservationSystem.AdminLogin" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container d-flex justify-content-center align-items-center min-vh-100">
        <div class="login-box">
            <h2 class="text-center font-weight-bold">Admin Login</h2>
            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                <ContentTemplate>
                    <div class="row">
                        <div class="col-md-12">
                            <label class="form-label font-weight-bold"></label>
                            <asp:TextBox ID="txtAdminEmail" runat="server" Placeholder="Admin Email" CssClass="form-control" TextMode="Email" />
                        </div>

                        <div class="col-md-12">
                            <label class="form-label font-weight-bold"></label>
                            <div class="input-group">
                                <asp:TextBox ID="txtAdminPassword" runat="server" CssClass="form-control" TextMode="Password" Placeholder="Password" />
                                <div class="input-group-append">
                                    <button class="btn btn-outline-secondary toggle-btn" type="button" onclick="togglePassword('<%= txtAdminPassword.ClientID %>')">Show</button>
                                </div>
                            </div>
                        </div>

                        <div class="col-12 mt-3">
                            <asp:Button ID="btnAdminLogin" runat="server" CssClass="btn btn-primary btn-block" Text="Login" OnClick="btnAdminLogin_Click" />
                        </div>

                        <div class="col-12 mt-2 text-center">
                            <asp:Label ID="lblAdminErrorMessage" runat="server" CssClass="error-message text-center" Visible="False"></asp:Label>
                        </div>

                        <div class="col-12 mt-2 text-center">
                            <asp:Label ID="lblUseeAccess" runat="server" Text="Not an Admin? Log in:" CssClass="form-label font-weight-bold" />
                            <br />
                            <asp:Button ID="btnUserAccess" runat="server" CssClass="btn btn-secondary" Text="User Login" OnClick="btnUserAccess_Click" />
                        </div>
                    </div>
                </ContentTemplate>

                <Triggers>
                    <asp:PostBackTrigger ControlID="btnAdminLogin" />
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
