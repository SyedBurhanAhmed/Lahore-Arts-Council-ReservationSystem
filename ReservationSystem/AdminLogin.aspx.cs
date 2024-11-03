using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.UI;

namespace ReservationSystem
{
    public partial class AdminLogin : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["AdminEmail"] != null)
            {
                Response.Redirect("AdminDashboard.aspx");
            }
        }

        protected void btnAdminLogin_Click(object sender, EventArgs e)
        {
            string adminEmail = txtAdminEmail.Text.Trim();
            string adminPassword = txtAdminPassword.Text.Trim();

            // Assume connection string is already configured in web.config
            string connectionString = ConfigurationManager.ConnectionStrings["LACConnectionString"].ConnectionString;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT COUNT(1) FROM Admins WHERE Email=@Email AND Password=@Password";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Email", adminEmail);
                cmd.Parameters.AddWithValue("@Password", adminPassword);

                con.Open();
                int count = Convert.ToInt32(cmd.ExecuteScalar());
                con.Close();

                if (count == 1)
                {
                    // Login successful, redirect to Admin Dashboard
                    Session["AdminEmail"] = adminEmail;  // Store admin session
                    Response.Redirect("AdminDashboard.aspx");
                }
                else
                {
                    // Invalid login, show error message
                    lblAdminErrorMessage.Text = "Invalid Email or Password";
                    lblAdminErrorMessage.Visible = true;
                }
            }
        }

        protected void btnUserAccess_Click(object sender, EventArgs e)
        {
            // Redirect to Login page when the admin access button is clicked
            Response.Redirect("Login.aspx");
        }
    }
}
