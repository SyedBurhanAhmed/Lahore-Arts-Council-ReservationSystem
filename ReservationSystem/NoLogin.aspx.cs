using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ReservationSystem
{
    public partial class NoLogin : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserID"] == null)
            {
                // Show the message to the user
                lblMessage.Text = "You must be logged in to access this page. Redirecting to the login page...";
                lblMessage.Visible = true;

                // Register the JavaScript to delay the redirection by 3 seconds
                string script = "setTimeout(function() { window.location='Login.aspx'; }, 3000);"; // 3000 ms = 3 seconds
                ClientScript.RegisterStartupScript(this.GetType(), "Redirect", script, true);

                return; // Exit the method early
            }
            else
            {
                Response.Redirect("UserDashboard.aspx");
            }

            if (Session["AdminEmail"] == null)
            {
                // Show the message to the user
                lblMessage.Text = "You must be logged in as an admin to access the dashboard. Redirecting to the login page...";
                lblMessage.Visible = true;

                // Register the JavaScript to delay the redirection by 3 seconds
                string script = "setTimeout(function() { window.location='AdminLogin.aspx'; }, 3000);"; // 3000 ms = 3 seconds
                ClientScript.RegisterStartupScript(this.GetType(), "Redirect", script, true);
                return;
            }
            else
            {
                Response.Redirect("AdminDashboard.aspx");
            }

        }
    }
}