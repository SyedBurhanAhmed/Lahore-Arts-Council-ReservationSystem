using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.IO;
using System.Net.Mail;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.Threading;
using System.Threading.Tasks;
using System.Net;

namespace ReservationSystem
{
    public partial class AdminDashboard : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["AdminEmail"] == null)
            {
                Response.Redirect("NoLogin.aspx");
            }

            if (!IsPostBack)
            {
                LoadAllBookings(); // Load only on first visit
            }
            
            else
            {
                LoadAllBookings();
            }
            /*
            LoadAllBookings();*/

        }


        // Method to load all pending bookings for the admin
        private void LoadAllBookings()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["LACConnectionString"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(@"
                                SELECT 
                                    B.BookingID, 
                                    U.Name, 
                                    U.CNIC, 
                                    B.FirstBookingDate,
                                    B.LastBookingDate,
                                    B.StartTime, 
                                    B.EndTime,
                                    F.Complex AS ComplexName,
                                    F.FacilityName,
                                    B.Topic,
                                    B.SecurityFeePath,  -- Include flag for security fee submission
                                    B.IsBookingApproved,       -- Include flag for booking approval
                                    B.IsSecurityFeeApproved,
                                    CASE 
                                        WHEN B.IsBookingApproved = 1 THEN 'Booking Approved' 
                                        WHEN B.IsBookingApproved = -1 THEN 'Booking Denied' 
                                        ELSE 'Pending' 
                                    END AS ApprovalStatus, 
                                    B.ApplicationFormPath
                                FROM 
                                    Bookings B
                                INNER JOIN 
                                    Users U ON B.UserID = U.UserID
                                INNER JOIN 
                                    Facilities F ON B.FacilityID = F.FacilityID
                            ", conn); // Only pending bookings


                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    gvBookings.DataSource = dt;
                    gvBookings.DataBind();
                    lblNoBookings.Visible = false; // Hide "No bookings" message
                }
                else
                {
                    gvBookings.DataSource = null;
                    gvBookings.DataBind();
                    lblNoBookings.Visible = true; // Show "No bookings" message
                    lblMessage.Visible = false;    // Hide the other message
                }
            }
        }


        // Method triggered when admin clicks any button (Approve/Disapprove/ViewForm)

        // Method to approve the booking
        private void ApproveBooking(int bookingID, string userEmail, int rowIndex)
        {
            var bookingDetails = GetBookingDetails(bookingID);

            if (!bookingDetails.HasValue)
            {
                lblMessage.Text = "Booking details not found or invalid data.";
                lblMessage.Visible = true;
                return;
            }

            var (facilityIDStr, FirstBookingDateStr, LastBookingDateStr, startTimeStr, endTimeStr) = bookingDetails.Value;
            var (complexNameStr, facilityNameStr, topicStr) = GetBookingDetailsById(bookingID);

            int facilityID = int.Parse(facilityIDStr);

            if (!IsFacilityAvailable(facilityID, FirstBookingDateStr, LastBookingDateStr, startTimeStr, endTimeStr))
            {
                lblMessage.Text = "Booking denied due to facility unavailability.";
                DisapproveBooking(bookingID, userEmail, rowIndex, "Booking denied due to facility unavailability.", "Your booking has been disapproved as the facility is unavailable.");
                return;
            }

            UpdateBookingStatus(bookingID, 1); // 1 means approved
            UpdateGridRow(rowIndex, "Booking Approved");

            lblMessage.Text = "Booking approved. Email sent to the user.";
            lblMessage.Visible = true;

            // Send approval email in the background
            _ = Task.Run(() =>
            {
                string subject = "LAC Booking Approved";
                string body = $@"
                <p>Dear User,</p>
                <h3>Your booking has been approved!</h3>
                <p>Your booking for the facility '{facilityNameStr}' at '{complexNameStr}' has been approved.</p>
                <p><strong>Booking Details:</strong></p>
                <ul>

                <li> Facility: {facilityNameStr}</li>
                <li> Complex: {complexNameStr}</li>
                <li> Dates: {FirstBookingDateStr} to {LastBookingDateStr}</li>
                <li> Time: {startTimeStr} to {endTimeStr}</li>
                <li> Topic: {topicStr}</li>

                <p><strong>Please submit the security fee to confirm your booking.</strong></p>
            ";
                _ = SendEmailAsync(userEmail, subject, body);
            });

            // Refresh GridView after delay
            //ScriptManager.RegisterStartupScript(this, this.GetType(), "ReloadGrid", "setTimeout(function() { __doPostBack('" + UpdatePanel1.UniqueID + "', ''); }, 3000);", true);
        }

        private void DisapproveBooking(int bookingID, string userEmail, int rowIndex, string adminText, string userMessage)
        {
            var bookingDetails = GetBookingDetails(bookingID);
            UpdateBookingStatus(bookingID, -1); // -1 means dIsBookingApproved
            if (lblMessage.Text != "Booking denied due to facility unavailability.")
            {
                UpdateGridRow(rowIndex, "Booking Denied");
            }
            else
            {
                UpdateGridRow(rowIndex, "Booking denied due to facility unavailability.");
            }
            var (facilityIDStr, FirstBookingDateStr, LastBookingDateStr, startTimeStr, endTimeStr) = bookingDetails.Value;
            var (complexNameStr, facilityNameStr, topicStr) = GetBookingDetailsById(bookingID);

            lblMessage.Text = adminText;
            lblMessage.Visible = true;

            // Send disapproval email in the background
            _ = Task.Run(() =>
            {
                string subject = "LAC Booking Denied";
                string body = $@"
            Dear User,
                <h3>Your booking has been denied!</h3>
            <p>Your booking for the facility '{facilityNameStr}' at '{complexNameStr}' from {FirstBookingDateStr} to {LastBookingDateStr} has been dIsBookingApproved.</p>
            <p>Reason: {userMessage}</p>";

                _ = SendEmailAsync(userEmail, subject, body);
            });

            // Refresh GridView after delay
            //ScriptManager.RegisterStartupScript(this, this.GetType(), "ReloadGrid", "setTimeout(function() { __doPostBack('" + UpdatePanel1.UniqueID + "', ''); }, 3000);", true);
        }



        // Method to get booking details for the given booking ID
        private (string FacilityID, string FirstBookingDate, string LastBookingDate,string StartTime, string EndTime)? GetBookingDetails(int bookingID)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["LACConnectionString"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(@"
            SELECT FacilityID, FirstBookingDate, LastBookingDate, StartTime, EndTime 
            FROM Bookings 
            WHERE BookingID = @BookingID", conn);
                cmd.Parameters.AddWithValue("@BookingID", bookingID);
                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        // Return all values as strings
                        return (
                            FacilityID: reader[0].ToString(),    // Convert to string
                            FirstBookingDate: reader[1].ToString(),   // Convert to string
                            LastBookingDate: reader[2].ToString(),   // Convert to string
                            StartTime: reader[3].ToString(),     // Convert to string
                            EndTime: reader[4].ToString()        // Convert to string
                        );
                    }
                }
            }
            return null; // Return null if booking details are not found
        }

        private (string complexName, string facilityName, string topic) GetBookingDetailsById(int bookingId)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["LACConnectionString"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(@"
            SELECT 
                ComplexName AS ComplexName,
                FacilityName,
                Topic
            FROM 
                BookingDetails
            WHERE BookingID = @BookingID", conn);

                cmd.Parameters.AddWithValue("@BookingID", bookingId);

                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return (
                            complexName: reader["ComplexName"].ToString(),
                            facilityName: reader["FacilityName"].ToString(),
                            topic: reader["Topic"].ToString()
                        );
                    }
                }
            }

            // Return default values if not found
            return (null, null, null);
        }


        // Helper method to update the booking status in the database
        private void ViewApplicationForm(int bookingID)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["LACConnectionString"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("SELECT ApplicationFormPath FROM Bookings WHERE BookingID = @BookingID", conn);
                cmd.Parameters.AddWithValue("@BookingID", bookingID);
                conn.Open();
                string relativeFilePath = cmd.ExecuteScalar().ToString();
                string absoluteFilePath = Server.MapPath(relativeFilePath);

                if (File.Exists(absoluteFilePath))
                {
                    string url = ResolveUrl(relativeFilePath);
                    string script = "window.open('" + url + "', '_blank');";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "OpenForm", script, true);

                    // Ensure the UpdatePanel updates correctly.
                    UpdatePanel1.Update();
                }
                else
                {
                    lblMessage.Text = "Application form not found.";
                    lblMessage.Visible = true;
                    UpdatePanel1.Update();
                }
            }
        }

        private void ViewSecurityFee(int bookingID)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["LACConnectionString"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("SELECT SecurityFeePath FROM Bookings WHERE BookingID = @BookingID", conn);
                cmd.Parameters.AddWithValue("@BookingID", bookingID);
                conn.Open();
                string relativeFilePath = cmd.ExecuteScalar().ToString();
                string absoluteFilePath = Server.MapPath(relativeFilePath);

                if (File.Exists(absoluteFilePath))
                {
                    string url = ResolveUrl(relativeFilePath);
                    string script = "window.open('" + url + "', '_blank');";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "OpenForm", script, true);

                    // Ensure the UpdatePanel updates correctly.
                    UpdatePanel1.Update();
                }
                else
                {
                    lblMessage.Text = "Security Fee not found.";
                    lblMessage.Visible = true;
                    UpdatePanel1.Update();
                }
            }
        }


        private void ApproveSecurityFee(int bookingID, string userEmail, int rowIndex)
        {
            UpdateSecurityFeeStatus(bookingID, 1); // 1 means fee approved
            UpdateGridRow(rowIndex, "Security Fee Approved");

            lblMessage.Text = "Security fee approved.";
            lblMessage.Visible = true;

            // Send approval email to the user
            _ = Task.Run(() =>
            {
                string subject = "Security Fee Approved";
                string body = "Your security fee has been approved. Your booking is now confirmed.";
                _ = SendEmailAsync(userEmail, subject, body);
            });
        }

        private void DisapproveSecurityFee(int bookingID, string userEmail, int rowIndex)
        {
            UpdateSecurityFeeStatus(bookingID, -1); // -1 means fee disapproved
            UpdateGridRow(rowIndex, "Security Fee Denied");

            lblMessage.Text = "Security fee disapproved.";
            lblMessage.Visible = true;

            // Send disapproval email to the user
            _ = Task.Run(() =>
            {
                string subject = "Security Fee Denied";
                string body = "Your security fee has been denied. Please contact us for more information.";
                _ = SendEmailAsync(userEmail, subject, body);
            });
        }

        private bool IsFacilityAvailable(int facilityID, string firstBookingDate, string lastBookingDate, string startTime, string endTime)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["LACConnectionString"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                // SQL query with string comparison
                SqlCommand cmd = new SqlCommand(
                    "SELECT COUNT(*) " +
                    "FROM Bookings " +
                    "WHERE FacilityID = @FacilityID " +
                    "AND ((FirstBookingDate <= @LastBookingDate AND LastBookingDate >= @FirstBookingDate)) " +
                    "AND ((StartTime < @EndTime AND EndTime > @StartTime)) " +
                    "AND IsSecurityFeeApproved = 1", conn);

                cmd.Parameters.AddWithValue("@FacilityID", facilityID);
                cmd.Parameters.AddWithValue("@FirstBookingDate", firstBookingDate);
                cmd.Parameters.AddWithValue("@LastBookingDate", lastBookingDate);
                cmd.Parameters.AddWithValue("@StartTime", startTime);
                cmd.Parameters.AddWithValue("@EndTime", endTime);

                conn.Open();
                int count = (int)cmd.ExecuteScalar();
                return count == 0;
            }
        }

        private void UpdateBookingStatus(int bookingID, int status)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["LACConnectionString"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("UPDATE Bookings SET IsBookingApproved = @Status WHERE BookingID = @BookingID", conn);
                cmd.Parameters.AddWithValue("@Status", status);
                cmd.Parameters.AddWithValue("@BookingID", bookingID);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }


        private void UpdateSecurityFeeStatus(int bookingID, int status)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["LACConnectionString"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("UPDATE Bookings SET IsSecurityFeeApproved = @Status WHERE BookingID = @BookingID", conn);
                cmd.Parameters.AddWithValue("@Status", status);
                cmd.Parameters.AddWithValue("@BookingID", bookingID);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Response.Redirect("~/AdminLogin.aspx");
        }

        // Helper method to get the user's email by booking ID
        private string GetUserEmail(int bookingID)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["LACConnectionString"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("SELECT Email FROM Users INNER JOIN Bookings ON Users.UserID = Bookings.UserID WHERE BookingID = @BookingID", conn);
                cmd.Parameters.AddWithValue("@BookingID", bookingID);
                conn.Open();
                return cmd.ExecuteScalar().ToString();
            }
        }

        // Method to send an email to the user
        // Method to send an email to the user asynchronously
        private async Task SendEmailAsync(string recipientEmail, string subject, string body)
        {/*
            try
            {
                var fromAddress = new MailAddress("ogloo950@gmail.com", "Lahore Arts Council");
                var toAddress = new MailAddress(recipientEmail);
                const string fromPassword = "mxtuitogabgdcduo";

                var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
                };

                using (var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true  // To support HTML content in the email body
                })
                {
                    await smtp.SendMailAsync(message);
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = "Error in sending email: " + ex.Message;
                lblMessage.Visible = true;
            }*/
            return;
        }


        // Method to update the GridView row after approval/disapproval
        private void UpdateGridRow(int rowIndex, string statusMessage)
        {
            GridViewRow row = gvBookings.Rows[rowIndex];
            row.Cells[10].Text = statusMessage;

            row.Cells[11].Controls.Clear(); // Clear Approve button
            row.Cells[12].Controls.Clear(); // Clear Disapprove button
            //row.Cells[13].Controls.Clear(); // Clear Disapprove button
        }

        // Method to remove the GridView row after a delay

        /*protected void gvBookings_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                string approvalStatus = DataBinder.Eval(e.Row.DataItem, "ApprovalStatus").ToString();

                // Clear buttons for approved or denied bookings
                if (approvalStatus == "Booking Approved" || approvalStatus == "Booking Denied")
                {
                    e.Row.Cells[11].Controls.Clear(); // Clear View Application Form button
                    e.Row.Cells[12].Controls.Clear(); // Clear Approve button
                    e.Row.Cells[13].Controls.Clear(); // Clear Disapprove button
                    e.Row.Cells[14].Controls.Clear();
                }
            }
        }*/
        protected void gvBookings_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                // Get the row index using the GridViewRow
                GridViewRow row = ((GridViewRow)((Control)e.CommandSource).NamingContainer);
                int rowIndex = row.RowIndex;
                int bookingID = Convert.ToInt32(gvBookings.DataKeys[rowIndex].Value); // Get the BookingID correctly using the row index

                // Fetch user email or other details using the correct bookingID
                string userEmail = GetUserEmail(bookingID);
                Label lblBookingStatus = (Label)row.FindControl("lblBookingStatus");
                Label lblSecurityFeeStatus = (Label)row.FindControl("lblSecurityFeeStatus");

                // Check the command name and execute corresponding logic
                switch (e.CommandName)
                {
                    case "ViewApplicationForm":
                        ViewApplicationForm(bookingID);
                        break;
                    case "Approve":
                        ApproveBooking(bookingID, userEmail, rowIndex);
                        lblBookingStatus.Text = "Booking Approved";
                        lblBookingStatus.Visible = true;
                        break;
                    case "Disapprove":
                        DisapproveBooking(bookingID, userEmail, rowIndex, "Booking disapproved and email sent to the user.", "Your booking has been disapproved due to Application rejection.");
                        lblBookingStatus.Text = "Booking Denied";
                        lblBookingStatus.Visible = true;
                        break;
                    case "ViewSecurityFee":
                        ViewSecurityFee(bookingID);
                        break;
                    case "ApproveSecurityFee":
                        ApproveSecurityFee(bookingID, userEmail, rowIndex);
                        lblSecurityFeeStatus.Text = "Security Fee Approved";
                        lblSecurityFeeStatus.Visible = true;
                        break;
                    case "DenySecurityFee":
                        DisapproveSecurityFee(bookingID, userEmail, rowIndex);
                        lblSecurityFeeStatus.Text = "Security Fee Denied";
                        lblSecurityFeeStatus.Visible = true;
                        break;
                }
                gvBookings.DataBind();
            }
            catch (Exception ex)
            {
                // Log or display the error
                lblMessage.Text = "An error occurred: " + ex.Message;
                lblMessage.Visible = true;
            }
        }




        protected void gvBookings_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                // Extract the necessary values from the data item
                var bookingStatus = Convert.ToInt32(DataBinder.Eval(e.Row.DataItem, "IsBookingApproved"));
                var securityFeePath = DataBinder.Eval(e.Row.DataItem, "SecurityFeePath") as string;
                var securityFeeApproved = Convert.ToInt32(DataBinder.Eval(e.Row.DataItem, "IsSecurityFeeApproved"));

                // Find the buttons and labels using FindControl
                LinkButton btnViewForm = (LinkButton)e.Row.FindControl("btnViewForm");
                LinkButton btnApprove = (LinkButton)e.Row.FindControl("btnApprove");
                LinkButton btnDisapprove = (LinkButton)e.Row.FindControl("btnDisapprove");
                LinkButton btnViewSecurityFee = (LinkButton)e.Row.FindControl("btnViewSecurityFee");
                LinkButton btnApproveSecurityFee = (LinkButton)e.Row.FindControl("btnApproveSecurityFee");
                LinkButton btnDenySecurityFee = (LinkButton)e.Row.FindControl("btnDenySecurityFee");

                Label lblBookingStatus = (Label)e.Row.FindControl("lblBookingStatus");
             //   Label lblSecurityFeeStatus = (Label)e.Row.FindControl("lblSecurityFeeStatus");

                // Logic to show/hide buttons based on booking status and security fee
                switch (bookingStatus)
                {
                    case 0: // Pending
                        btnViewForm.Visible = true;
                        btnApprove.Visible = true;
                        btnDisapprove.Visible = true;
                        btnViewSecurityFee.Visible = false;
                        btnApproveSecurityFee.Visible = false;
                        btnDenySecurityFee.Visible = false;
                        break;

                    case 1: // Approved
                        btnViewForm.Visible = false;
                        btnApprove.Visible = false;
                        btnDisapprove.Visible = false;
                        lblBookingStatus.Text = "Booking Approved. Security Fee pending.";
                        lblBookingStatus.Visible = true;

                        // Show security fee buttons only if the fee has been submitted and not yet approved or denied
                        if (!string.IsNullOrEmpty(securityFeePath))
                        {
                            btnViewSecurityFee.Visible = true;
                            lblBookingStatus.Text = "Security Fee submitted.";
                            lblBookingStatus.Visible = true;
                            // Show or hide approval/denial buttons based on the current status of the security fee
                            if (securityFeeApproved == 0)
                            {
                                // Security fee not yet approved or denied, show the buttons
                                btnApproveSecurityFee.Visible = true;
                                btnDenySecurityFee.Visible = true;
                                

                            }
                            else if (securityFeeApproved == 1)
                            {
                                // Security fee is approved
                                lblBookingStatus.Text = "Booking and Security Fee Approved";
                                lblBookingStatus.Visible = true;
                                btnViewSecurityFee.Visible = false;
                               // lblSecurityFeeStatus.Visible = true;
                                btnApproveSecurityFee.Visible = false;
                                btnDenySecurityFee.Visible = false;
                            }
                            else if (securityFeeApproved == -1)
                            {
                                // Security fee is denied
                                lblBookingStatus.Text = "Booking and Security Fee Denied";
                                btnViewSecurityFee.Visible = false;
                               // lblSecurityFeeStatus.Visible = true;
                                btnApproveSecurityFee.Visible = false;
                                btnDenySecurityFee.Visible = false;
                            }
                        }
                        else
                        {
                            // Security fee not submitted, hide the buttons and show appropriate message if needed
                            btnViewSecurityFee.Visible = false;
                            btnApproveSecurityFee.Visible = false;
                            btnDenySecurityFee.Visible = false;
                           // lblSecurityFeeStatus.Visible = false;
                        }
                        break;

                    case -1: // Denied
                        lblBookingStatus.Text = "Booking Denied";
                        lblBookingStatus.Visible = true;
                        btnViewForm.Visible = false;
                        btnApprove.Visible = false;
                        btnDisapprove.Visible = false;
                        btnViewSecurityFee.Visible = false;
                        btnApproveSecurityFee.Visible = false;
                        btnDenySecurityFee.Visible = false;
                        break;

                    default:
                        break;
                }
            }
        }

        protected void btnClearBookings_Click(object sender, EventArgs e)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["LACConnectionString"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                using (SqlCommand cmd = new SqlCommand(@"
            DELETE FROM Bookings 
            WHERE IsBookingApproved = -1 OR IsSecurityFeeApproved = -1", conn))
                {
                    // Execute the command and check how many rows were affected
                    int rowsAffected = cmd.ExecuteNonQuery();

                    // Optionally, provide feedback based on the result
                    if (rowsAffected > 0)
                    {
                        lblMessage.Text = $"{rowsAffected} denied bookings cleared.";
                    }
                    else
                    {
                        lblMessage.Text = "No denied bookings to clear.";
                    }
                }
            }

            LoadAllBookings(); // Refresh the GridView
            lblMessage.Visible = true;
        }








        /* protected void gvBookings_RowDataBound(object sender, GridViewRowEventArgs e)
         {
             if (e.Row.RowType == DataControlRowType.DataRow)
             {
                 string approvalStatus = DataBinder.Eval(e.Row.DataItem, "ApprovalStatus").ToString();

                 // Clear buttons for approved or denied bookings
                 *//*if (approvalStatus == "Booking Approved" || approvalStatus == "Booking Denied")
                 {
                     e.Row.Cells[11].Controls.Clear(); // Clear View Application Form button
                     e.Row.Cells[12].Controls.Clear(); // Clear Approve button
                     e.Row.Cells[13].Controls.Clear(); // Clear Disapprove button
                     e.Row.Cells[14].Controls.Clear();
                 }*//*
             }
         }*/
    }






}




