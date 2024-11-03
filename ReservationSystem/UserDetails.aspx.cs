using System;
using System.Data.SqlClient;
using System.Configuration;
using System.IO;
using System.Web.UI.WebControls;

namespace ReservationSystem
{
    public partial class UserDetails : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserID"] == null)
            {
                Response.Redirect("NoLogin.aspx");
            }

            if (!IsPostBack)
            {
                DisplayBookingDetails();
                LoadUserDetails();
            }
        }

        private void DisplayBookingDetails()
        {
            if (Session["FirstBookingDate"] == null || Session["LastBookingDate"] == null || Session["StartTime"] == null || Session["EndTime"] == null)
                return;

            DateTime firstBookingDate = (DateTime)Session["FirstBookingDate"];
            DateTime lastBookingDate = (DateTime)Session["LastBookingDate"];
            TimeSpan startTime = (TimeSpan)Session["StartTime"];
            TimeSpan endTime = (TimeSpan)Session["EndTime"];

            lblFirstBookingDate.Text = firstBookingDate.ToString("dd-MM-yyyy");
            lblLastBookingDate.Text = lastBookingDate.ToString("dd-MM-yyyy");
            lblTimeSlot.Text = $"{startTime} - {endTime}";
        }


        private void LoadUserDetails()
        {
            lblFullName.Text = Session["UserName"]?.ToString() ?? "N/A";
            lblFatherName.Text = Session["FatherName"]?.ToString() ?? "N/A";
            lblCNIC.Text = Session["CNIC"]?.ToString() ?? "N/A";
            lblPhoneNumber.Text = Session["PhoneNumber"]?.ToString() ?? "N/A";
            lblEmail.Text = Session["Email"]?.ToString() ?? "N/A";
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            string name = lblFullName.Text;
            string cnic = lblCNIC.Text;
            string topic = txtTopic.Text;
            int facilityID = int.Parse(Session["SelectedFacility"].ToString());
            DateTime firstBookingDate = (DateTime)Session["FirstBookingDate"];
            DateTime lastBookingDate = (DateTime)Session["LastBookingDate"];
            int totalDays = (int)Session["TotalDays"];
            TimeSpan startTime = (TimeSpan)Session["StartTime"];
            TimeSpan endTime = (TimeSpan)Session["EndTime"];

            if (string.IsNullOrEmpty(topic))
            {
                lblErrorMessage.Text = "Event Topic is required.";
                lblErrorMessage.Visible = true;
                return;
            }

            DateTime currentDate = firstBookingDate;

            while (currentDate <= lastBookingDate)
            {
                if (!IsFacilityAvailable(facilityID, currentDate, startTime, endTime))
                {
                    lblErrorMessage.Text = $"Facility is already booked for {currentDate:dd-MM-yyyy} at the selected time.";
                    lblErrorMessage.Visible = true;
                    return;
                }
                currentDate = currentDate.AddDays(1); // Move to the next day
            }

            if (fuApplicationForm.HasFile)
            {
                string fileExtension = Path.GetExtension(fuApplicationForm.FileName).ToLower();
                string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".pdf" };

                if (Array.Exists(allowedExtensions, ext => ext == fileExtension))
                {
                    // Folder path where the file will be saved (relative to the application root)
                    string folderPath = "~/ApplicationForms/";

                    // Ensure the directory exists
                    string absoluteFolderPath = Server.MapPath(folderPath);
                    if (!Directory.Exists(absoluteFolderPath))
                    {
                        Directory.CreateDirectory(absoluteFolderPath);
                    }

                    // Create a unique file name to avoid conflicts
                    string fileName = Path.GetFileNameWithoutExtension(fuApplicationForm.FileName) + "_" + DateTime.Now.Ticks + fileExtension;

                    // Save the file physically
                    string relativeFilePath = folderPath + fileName; // Storing the relative path (e.g., "~/ApplicationForms/file.jpg")
                    string absoluteFilePath = Path.Combine(absoluteFolderPath, fileName); // Full physical path

                    fuApplicationForm.SaveAs(absoluteFilePath);

                    // Now save the relative path to the database instead of the full path
                    SaveBookingDetails(topic, facilityID, firstBookingDate, lastBookingDate, totalDays, startTime, endTime, relativeFilePath);

                    lblErrorMessage.Text = "Your application has been submitted.";
                    lblErrorMessage.CssClass = "text-success";
                    lblErrorMessage.Visible = true;

                    Session["EventTopic"] = topic;
                    // Redirect after successful submission
                    Response.Redirect("Confirmation.aspx");
                }
                else
                {
                    lblErrorMessage.Text = "Only JPG, JPEG, PNG, and PDF files are allowed.";
                    lblErrorMessage.Visible = true;
                }
            }
            else
            {
                lblErrorMessage.Text = "Please upload the application form.";
                lblErrorMessage.Visible = true;
            }
        }

        private void SaveBookingDetails(string topic, int facilityID, DateTime firstBookingDate, DateTime lastBookingDate, int totalDays, TimeSpan startTime, TimeSpan endTime, string filePath)
        {
            // Ensure the UserID is retrieved from the session
            if (Session["UserID"] == null)
            {
                throw new InvalidOperationException("User is not logged in or UserID is missing in session.");
            }

            int userID = Convert.ToInt32(Session["UserID"]); // Get UserID from session
            string connectionString = ConfigurationManager.ConnectionStrings["LACConnectionString"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // Insert the booking details for the logged-in user
                SqlCommand insertBookingCmd = new SqlCommand(
                    "INSERT INTO Bookings (UserID, FacilityID, FirstBookingDate, LastBookingDate, TotalDays, StartTime, EndTime, Topic, ApplicationFormPath) " +
                    "VALUES (@UserID, @FacilityID, @FirstBookingDate, @LastBookingDate, @TotalDays, @StartTime, @EndTime, @Topic, @ApplicationFormPath)", conn);

                insertBookingCmd.Parameters.AddWithValue("@UserID", userID);
                insertBookingCmd.Parameters.AddWithValue("@FacilityID", facilityID);
                insertBookingCmd.Parameters.AddWithValue("@FirstBookingDate", firstBookingDate.ToString("dd-MM-yyyy"));
                insertBookingCmd.Parameters.AddWithValue("@LastBookingDate", lastBookingDate.ToString("dd-MM-yyyy"));
                insertBookingCmd.Parameters.AddWithValue("@TotalDays", totalDays);
                insertBookingCmd.Parameters.AddWithValue("@StartTime", startTime.ToString(@"hh\:mm"));
                insertBookingCmd.Parameters.AddWithValue("@EndTime", endTime.ToString(@"hh\:mm"));
                insertBookingCmd.Parameters.AddWithValue("@Topic", topic);
                insertBookingCmd.Parameters.AddWithValue("@ApplicationFormPath", filePath);

                insertBookingCmd.ExecuteNonQuery();
            }
        }
        private bool IsFacilityAvailable(int facilityID, DateTime date, TimeSpan startTime, TimeSpan endTime)
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["LACConnectionString"].ConnectionString;

            // Convert DateTime and TimeSpan to string
            string bookingDate = date.ToString("dd-MM-yyyy"); // Format as 'dd-MM-yyyy'
            string startTimeStr = startTime.ToString(@"hh\:mm"); // Format as 'HH:mm'
            string endTimeStr = endTime.ToString(@"hh\:mm"); // Format as 'HH:mm'

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                // SQL query with string comparison for date and time
                SqlCommand cmd = new SqlCommand(
                    "SELECT COUNT(*) " +
                    "FROM Bookings " +
                    "WHERE FacilityID = @FacilityID " +
                    "AND @BookingDate BETWEEN FirstBookingDate AND LastBookingDate " +
                    "AND ((StartTime < @EndTime AND EndTime > @StartTime)) " +
                    "AND IsSecurityFeeApproved = 1", conn);

                cmd.Parameters.AddWithValue("@FacilityID", facilityID);
                cmd.Parameters.AddWithValue("@BookingDate", bookingDate);
                cmd.Parameters.AddWithValue("@StartTime", startTimeStr);
                cmd.Parameters.AddWithValue("@EndTime", endTimeStr);

                conn.Open();
                int count = (int)cmd.ExecuteScalar();
                return count == 0;
            }
        }


    }
}
