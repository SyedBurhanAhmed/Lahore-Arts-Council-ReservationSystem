using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ReservationSystem
{
    public partial class DateSelection : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserID"] == null)
            {
                Response.Redirect("NoLogin.aspx");
            }

            if (!IsPostBack)
            {
                PopulateStartTimeDropdowns();
                PopulateEndTimeDropdowns();
                SetMinBookingDate();
            }
        }

        private void SetMinBookingDate()
        {
            DateTime minBookingDate = DateTime.Now.AddDays(7);
            txtFirstBookingDate.Text = minBookingDate.ToString("yyyy-MM-dd");
            txtLastBookingDate.Text = minBookingDate.ToString("yyyy-MM-dd");
        
        }

        // Populate Start and End Time dropdowns dynamically
        private void PopulateStartTimeDropdowns()
        {
            // Create a list of time slots between 8 AM to 10 PM
            List<string> timeSlots = new List<string>();
            timeSlots.Add("Select Start Time"); // Default option
            for (int i = 8; i <= 22; i++)  // from 8 AM to 10 PM
            {
                timeSlots.Add($"{i:00}:00");
                timeSlots.Add($"{i:00}:30");
            }

            // Bind time slots to the dropdown
            ddlStartTime.DataSource = timeSlots;
            ddlStartTime.DataBind();
            ddlStartTime.SelectedIndex = 0; // Set the default option as selected
        }

        private void PopulateEndTimeDropdowns()
        {
            // Create a list of time slots between 8 AM to 10 PM
            List<string> timeSlots = new List<string>();
            timeSlots.Add("Select End Time"); // Default option
            for (int i = 10; i <= 22; i++)  // from 10 AM to 10 PM
            {
                if (i == 10)
                {
                    timeSlots.Add($"{i:00}:30");
                    continue;
                }
                timeSlots.Add($"{i:00}:00");
                timeSlots.Add($"{i:00}:30");
            }

            // Bind time slots to the dropdown
            ddlEndTime.DataSource = timeSlots;
            ddlEndTime.DataBind();
            ddlEndTime.SelectedIndex = 0; // Set the default option as selected
        }


        // Time validation logic triggered when user selects time
        protected void ValidateTimes(object sender, EventArgs e)
        {
            lblErrorMessage.Visible = false; // Hide the label initially

            if (ddlStartTime.SelectedValue == "Select Start Time" || ddlEndTime.SelectedValue == "Select End Time")
            {
                return; // Do nothing if either dropdown has the default value
            }

            TimeSpan startTime = TimeSpan.Parse(ddlStartTime.SelectedValue);
            TimeSpan endTime = TimeSpan.Parse(ddlEndTime.SelectedValue);

            // Check if end time is greater than start time
            if (endTime <= startTime)
            {
                lblErrorMessage.Text = "End time must be greater than start time.";
                lblErrorMessage.Visible = true;
                return;
            }

            // Check if the duration is at least 2.5 hours
            if (endTime - startTime < TimeSpan.FromHours(2.5))
            {
                lblErrorMessage.Text = "Booking must be at least 2.5 hours.";
                lblErrorMessage.Visible = true;
                return;
            }

            lblErrorMessage.Visible = false; // Hide the label if validation passes
        }



        // Proceed with booking if time is valid
        protected void btnNext_Click(object sender, EventArgs e)
        {
            lblErrorMessage.Visible = false; // Hide the label initially

            // Check if first and last booking dates are valid
            if (string.IsNullOrWhiteSpace(txtFirstBookingDate.Text) || !DateTime.TryParse(txtFirstBookingDate.Text, out DateTime firstBookingDate))
            {
                lblErrorMessage.Text = "Please select a valid first booking date before proceeding.";
                lblErrorMessage.Visible = true;
                return;
            }

            if (string.IsNullOrWhiteSpace(txtLastBookingDate.Text) || !DateTime.TryParse(txtLastBookingDate.Text, out DateTime lastBookingDate))
            {
                lblErrorMessage.Text = "Please select a valid last booking date before proceeding.";
                lblErrorMessage.Visible = true;
                return;
            }

            if (firstBookingDate > lastBookingDate)
            {
                lblErrorMessage.Text = "The first booking date must be before or equal to the last booking date.";
                lblErrorMessage.Visible = true;
                return;
            }

            // Validate start time selection
            if (ddlStartTime.SelectedValue == "Select Start Time")
            {
                lblErrorMessage.Text = "Please select a start time before proceeding.";
                lblErrorMessage.Visible = true;
                return;
            }

            // Validate end time selection
            if (ddlEndTime.SelectedValue == "Select End Time")
            {
                lblErrorMessage.Text = "Please select an end time before proceeding.";
                lblErrorMessage.Visible = true;
                return;
            }

            TimeSpan startTime = TimeSpan.Parse(ddlStartTime.SelectedValue);
            TimeSpan endTime = TimeSpan.Parse(ddlEndTime.SelectedValue);

            if (endTime <= startTime)
            {
                lblErrorMessage.Text = "End time must be greater than start time.";
                lblErrorMessage.Visible = true;
                return;
            }

            // Check if the duration is at least 2.5 hours
            if (endTime - startTime < TimeSpan.FromHours(2.5))
            {
                lblErrorMessage.Text = "Booking must be at least 2.5 hours.";
                lblErrorMessage.Visible = true;
                return;
            }

            int facilityID = int.Parse(Session["SelectedFacility"].ToString());

            // Check availability for each date in the range
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

            // Calculate total number of booking days
            int totalDays = (lastBookingDate - firstBookingDate).Days + 1; // Include both start and end date

            // If all dates are available, proceed to the next step
            Session["FirstBookingDate"] = firstBookingDate; // Store first booking date
            Session["LastBookingDate"] = lastBookingDate; // Store last booking date
            Session["StartTime"] = startTime; // Store start time
            Session["EndTime"] = endTime; // Store end time
            Session["TotalDays"] = totalDays; // Store total booking days

            Response.Redirect("UserDetails.aspx");
        }


        // Check if facility is available during the selected time
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