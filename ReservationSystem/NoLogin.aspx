<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="NoLogin.aspx.cs" Inherits="ReservationSystem.NoLogin" %>
    <asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container">
        <h2 class="text-center">Please Login. </h2>
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <asp:Label ID="lblMessage" runat="server" CssClass="success-message text-center" Visible="False"></asp:Label>
</ContentTemplate>
        </asp:UpdatePanel>
    </div>
</asp:Content>
