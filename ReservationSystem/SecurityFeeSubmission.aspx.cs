using System;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;

namespace ReservationSystem
{
    public partial class SubmitSecurityFee : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Retrieve booking details using BookingID from the session
                if (Session["BookingID"] != null)
                {
                    int bookingID = int.Parse(Session["BookingID"].ToString());
                    LoadBookingDetails(bookingID);
                    DisplayTotalCharges(bookingID);
                }
            }
        }

        private void LoadBookingDetails(int bookingID)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["LACConnectionString"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT Topic, FirstBookingDate, LastBookingDate, StartTime, EndTime FROM Bookings WHERE BookingID = @BookingID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@BookingID", bookingID);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    lblTopic.Text = reader["Topic"].ToString();
                    lblFirstBookingDate.Text = reader["FirstBookingDate"].ToString();
                    lblLastBookingDate.Text = reader["LastBookingDate"].ToString();

                    string startTime = reader["StartTime"] != DBNull.Value ? reader["StartTime"].ToString() : "N/A";
                    string endTime = reader["EndTime"] != DBNull.Value ? reader["EndTime"].ToString() : "N/A";
                    lblTimeSlot.Text = $"{startTime} - {endTime}";

                    // Store start and end times as strings in session
                    Session["StartTime"] = startTime;
                    Session["EndTime"] = endTime;
                }
            }
        }

        private void DisplayTotalCharges(int bookingID)
        {
            // Database connection string
            string connectionString = ConfigurationManager.ConnectionStrings["LACConnectionString"].ConnectionString;

            // Retrieve FacilityID, StartTime, and EndTime using BookingID
            int facilityID = 0;
            TimeSpan startTime;
            TimeSpan endTime;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(
                    "SELECT FacilityID, StartTime, EndTime FROM Bookings WHERE BookingID = @BookingID", conn);
                cmd.Parameters.AddWithValue("@BookingID", bookingID);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    facilityID = reader["FacilityID"] != DBNull.Value ? (int)reader["FacilityID"] : 0;
                    startTime = reader["StartTime"] != DBNull.Value ? TimeSpan.Parse(reader["StartTime"].ToString()) : TimeSpan.Zero;
                    endTime = reader["EndTime"] != DBNull.Value ? TimeSpan.Parse(reader["EndTime"].ToString()) : TimeSpan.Zero;
                }
                else
                {
                    lblTotalCharges.Text = "Total Charges: Facility or Time Information Not Found";
                    return;
                }
            }

            // Now, get the RatePerHour and SecurityFee using the FacilityID
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("SELECT RatePerHour, SecurityFees FROM Facilities WHERE FacilityID = @FacilityID", conn);
                cmd.Parameters.AddWithValue("@FacilityID", facilityID);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    int ratePerHour = reader["RatePerHour"] != DBNull.Value ? (int)reader["RatePerHour"] : 0;
                    int securityFee = reader["SecurityFees"] != DBNull.Value ? (int)reader["SecurityFees"] : 0;
                    double hours = (endTime - startTime).TotalHours;

                    // Parse dates from string
                    DateTime firstBookingDate = DateTime.ParseExact(lblFirstBookingDate.Text, "dd-MM-yyyy", null);
                    DateTime lastBookingDate = DateTime.ParseExact(lblLastBookingDate.Text, "dd-MM-yyyy", null);
                    int totalDays = (lastBookingDate - firstBookingDate).Days + 1; // Adding 1 to include the last day

                    // Calculate total charges
                    double totalCharges = hours * ratePerHour * totalDays;
                    lblTotalCharges.Text = $"Rs. {totalCharges:N2}";

                    // Display the security fee
                    lblSecurityFee.Text = $"Rs. {securityFee:N2}";
                }
                else
                {
                    lblTotalCharges.Text = "Total Charges: Rate Not Found";
                    lblSecurityFee.Text = "Security Fee: Not Found";
                }
            }
        }




        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (fileSecurityFee.HasFile)
            {
                string bookingID = Session["BookingID"].ToString();
                string fileName = $"{bookingID}_{Path.GetFileName(fileSecurityFee.FileName)}";
                string virtualPath = "~/SecurityFees/" + fileName;
                string physicalPath = Server.MapPath(virtualPath);

                string directoryPath = Path.GetDirectoryName(physicalPath);
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath); // Create the directory if it doesn't exist

                }
                    // Save the uploaded file
                fileSecurityFee.SaveAs(physicalPath);

                // Save the virtual path to the database
                string connectionString = ConfigurationManager.ConnectionStrings["LACConnectionString"].ConnectionString;
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand("UPDATE Bookings SET SecurityFeePath = @Path WHERE BookingID = @BookingID", conn);
                    cmd.Parameters.AddWithValue("@Path", virtualPath);
                    cmd.Parameters.AddWithValue("@BookingID", bookingID);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }

                lblMessage.Text = "Security fee screenshot submitted successfully! Please submit the total charges as a pay order to Lahore Arts Council. Redirecting to User Dashboard..";
                lblMessage.Visible = true;
                string script = "setTimeout(function() { window.location='UserDashboard.aspx'; }, 3000);"; // 3000 ms = 3 seconds
                ClientScript.RegisterStartupScript(this.GetType(), "Redirect", script, true);
            }
            else
            {
                lblMessage.Text = "Please upload a security fee screenshot.";
                lblMessage.Visible = true;
            }
        }
    }
}
