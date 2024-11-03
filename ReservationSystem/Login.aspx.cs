using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace ReservationSystem
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserID"] != null)
            {
                Response.Redirect("UserDashboard.aspx");
            }
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            string userEmail = txtEmail.Text.Trim();
            string userPassword = txtPassword.Text.Trim();

            if (ValidateUser(userEmail, userPassword))
            {
                // Get user details and store them in session
                DataTable userDetails = GetUserDetails(userEmail);
                if (userDetails.Rows.Count > 0)
                {
                    // Assuming the first row contains the user details
                    DataRow row = userDetails.Rows[0];
                    Session["UserID"] = row["UserID"];
                    Session["UserName"] = row["Name"];
                    Session["FatherName"] = row["FatherName"];
                    Session["CNIC"] = row["CNIC"];
                    Session["PhoneNumber"] = row["PhoneNumber"];
                    Session["Email"] = userEmail;

                    // Redirect based on booking status
                    if (UserHasBookings((int)row["UserID"]))
                    {
                        Response.Redirect("UserDashboard.aspx");
                    }
                    else
                    {
                        Response.Redirect("StartPage.aspx");
                    }
                }
            }
            else
            {
                lblErrorMessage.Text = "Invalid email or password.";
                lblErrorMessage.Visible = true;
            }
        }

        protected void btnAdminAccess_Click(object sender, EventArgs e)
        {
            // Redirect to AdminLogin page when the admin access button is clicked
            Response.Redirect("AdminLogin.aspx");
        }

        private bool ValidateUser(string email, string password)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["LACConnectionString"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Users WHERE Email = @Email AND Password = @Password", conn);
                cmd.Parameters.AddWithValue("@Email", email);
                cmd.Parameters.AddWithValue("@Password", password);

                conn.Open();
                int count = (int)cmd.ExecuteScalar();
                return count > 0;
            }
        }

        private DataTable GetUserDetails(string email)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["LACConnectionString"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(
                    "SELECT UserID, Name, FatherName, CNIC, PhoneNumber " +
                    "FROM Users WHERE Email = @Email", conn);
                cmd.Parameters.AddWithValue("@Email", email);

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable userDetails = new DataTable();
                adapter.Fill(userDetails);
                return userDetails;
            }
        }

        private bool UserHasBookings(int userID)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["LACConnectionString"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Bookings WHERE UserID = @UserID", conn);
                cmd.Parameters.AddWithValue("@UserID", userID);

                conn.Open();
                int count = (int)cmd.ExecuteScalar();
                return count > 0;
            }
        }
    }
}
