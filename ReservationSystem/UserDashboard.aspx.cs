using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI.WebControls;
using System.Web.UI;

namespace ReservationSystem
{
    public partial class UserDashboard : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserID"] == null)
            {
                Response.Redirect("NoLogin.aspx");
            }

            rptBookings.ItemCommand += new RepeaterCommandEventHandler(rptBookings_ItemCommand);
            if (!IsPostBack)
            {
                int userID = Convert.ToInt32(Session["UserID"]);
                LoadUserBookings(userID);
            }
        }


        protected void rptBookings_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "SubmitSecurityFee")
            {
                // Get the BookingID from the CommandArgument
                int bookingID = Convert.ToInt32(e.CommandArgument);

                // Redirect to the page where the user can submit the security fee
                Session["BookingID"] = bookingID;
                Response.Redirect("SecurityFeeSubmission.aspx?BookingID=" + Server.UrlEncode(bookingID.ToString()));
            }
        }

        protected string GetTotalCharges(object bookingIDObj)
        {
            int bookingID = Convert.ToInt32(bookingIDObj);

            // Database connection string
            string connectionString = ConfigurationManager.ConnectionStrings["LACConnectionString"].ConnectionString;

            // Variables to hold start time, end time, and facility ID
            TimeSpan startTime;
            TimeSpan endTime;
            int facilityID;

            // Retrieve StartTime, EndTime, and FacilityID using BookingID
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT StartTime, EndTime, FacilityID, FirstBookingDate, LastBookingDate FROM Bookings WHERE BookingID = @BookingID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@BookingID", bookingID);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    // Parse StartTime and EndTime from the database
                    startTime = TimeSpan.Parse(reader["StartTime"].ToString());
                    endTime = TimeSpan.Parse(reader["EndTime"].ToString());
                    facilityID = (int)reader["FacilityID"];

                    // Retrieve booking dates
                    DateTime firstBookingDate = DateTime.ParseExact(reader["FirstBookingDate"].ToString(), "dd-MM-yyyy", null);
                    DateTime lastBookingDate = DateTime.ParseExact(reader["LastBookingDate"].ToString(), "dd-MM-yyyy", null);
                    int totalDays = (lastBookingDate - firstBookingDate).Days + 1;

                    // Close the reader
                    reader.Close();

                    // Retrieve RatePerHour using FacilityID and calculate total charges
                    using (SqlCommand rateCmd = new SqlCommand("SELECT RatePerHour FROM Facilities WHERE FacilityID = @FacilityID", conn))
                    {
                        rateCmd.Parameters.AddWithValue("@FacilityID", facilityID);
                        object rateResult = rateCmd.ExecuteScalar();

                        if (rateResult != null)
                        {
                            int ratePerHour = (int)rateResult;
                            double hours = (endTime - startTime).TotalHours;

                            // Calculate total charges
                            double totalCharges = hours * ratePerHour * totalDays;
                            return $"Rs. {totalCharges:N2}";
                        }
                        else
                        {
                            return "Total Charges: Rate Not Found";
                        }
                    }
                }
                else
                {
                    return "Total Charges: Booking Not Found";
                }
            }
        }


        /*


        private void LoadUserBookings(int userID)
         {
             string cnic = FindCNIC(userID);

             if (string.IsNullOrEmpty(cnic))
             {
                 lblNoBookings.Text = "Error: CNIC not found for the user.";
                 lblNoBookings.Visible = true;
                 btnBookSlot.Visible = false;
                 return;
             }

             string connectionString = ConfigurationManager.ConnectionStrings["LACConnectionString"].ConnectionString;
             using (SqlConnection conn = new SqlConnection(connectionString))
             {
                 SqlCommand cmd = new SqlCommand(@"
             SELECT BookingID, ComplexName, FacilityName, Topic, FirstBookingDate, LastBookingDate, StartTime, EndTime, IsBookingApproved, SecurityFeePath, IsSecurityFeeApproved
             FROM BookingDetails 
             WHERE CNIC = @CNIC", conn);
                 cmd.Parameters.AddWithValue("@CNIC", cnic);

                 SqlDataAdapter da = new SqlDataAdapter(cmd);
                 DataTable dt = new DataTable();
                 da.Fill(dt);

                 if (dt.Rows.Count > 0)
                 {
                     gvBookings.DataSource = dt;
                     gvBookings.DataBind();
                     gvBookings.Visible = true;
                     btnBookSlot.Visible = true;
                 }
                 else
                 {
                     lblNoBookings.Text = "You have no bookings. Please book a slot.";
                     lblNoBookings.Visible = true;
                     btnBookSlot.Visible = true;
                 }
             }
         }*/

        private DataTable GetBookingsFromDatabase(string cnic)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["LACConnectionString"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(@"
            SELECT BookingID, ComplexName, FacilityName, Topic, FirstBookingDate, LastBookingDate, StartTime, EndTime, IsBookingApproved, SecurityFeePath, IsSecurityFeeApproved
            FROM BookingDetails 
            WHERE CNIC = @CNIC", conn);
                cmd.Parameters.AddWithValue("@CNIC", cnic);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }

        private void LoadUserBookings(int userID)
        {
            string cnic = FindCNIC(userID);

            if (string.IsNullOrEmpty(cnic))
            {
                lblNoBookings.Text = "Error: CNIC not found for the user.";
                lblNoBookings.Visible = true;
                btnBookSlot.Visible = false;
                return;
            }
            // Get the bookings from the database
            var bookings = GetBookingsFromDatabase(cnic);

            // Bind the bookings to the Repeater
            rptBookings.DataSource = bookings;
            rptBookings.DataBind();

            // Optionally, display a message if there are no bookings
            lblNoBookings.Visible = bookings == null;
        }

        private string FindCNIC(int userID)
        {
            string cnic = null;
            string connectionString = ConfigurationManager.ConnectionStrings["LACConnectionString"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("SELECT CNIC FROM Users WHERE UserID = @UserID", conn);
                cmd.Parameters.AddWithValue("@UserID", userID);

                try
                {
                    conn.Open();
                    object result = cmd.ExecuteScalar();

                    if (result != null)
                    {
                        cnic = result.ToString();
                    }
                }
                catch (Exception ex)
                {
                    // Handle exception (log it or display an error message)
                    Console.WriteLine("Error fetching CNIC: " + ex.Message);
                }
            }

            return cnic;
        }


        /*protected void gvBookings_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "SubmitSecurityFee")
            {
                // Retrieve the row index from CommandArgument
                int rowIndex;
                if (int.TryParse(e.CommandArgument.ToString(), out rowIndex))
                {
                    // Ensure the index is within valid range
                    if (rowIndex >= 0 && rowIndex < gvBookings.Rows.Count)
                    {
                        // Retrieve the BookingID from DataKeys collection
                        int bookingID = Convert.ToInt32(gvBookings.DataKeys[rowIndex].Value);

                        // Store the BookingID in session and redirect
                        Session["BookingID"] = bookingID;
                        Response.Redirect("SecurityFeeSubmission.aspx?BookingID=" + Server.UrlEncode(bookingID.ToString()));
                    }
                    else
                    {
                        // Handle invalid index scenario
                        // Optional: Log the error or show a message to the user
                        lblError.Text = "Invalid booking selection. Please try again.";
                        lblError.Visible = true;
                    }
                }
                else
                {
                    // Handle conversion error if CommandArgument is not a valid index
                    lblError.Text = "Unable to process the request. Please try again.";
                    lblError.Visible = true;
                }
            }
        }*/

        protected void gvBookings_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "SubmitSecurityFee")
            {
                // Get the BookingID from the CommandArgument
                int bookingID = Convert.ToInt32(e.CommandArgument);

                // Redirect to the page where the user can submit the security fee
                Response.Redirect($"SubmitSecurityFee.aspx?BookingID={bookingID}");
            }
        }



       /* protected void gvBookings_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                // Get the booking data from the current row
                var booking = (DataRowView)e.Row.DataItem;

                // Create a new row to hold the "Submit Security Fee" button
                GridViewRow buttonRow = new GridViewRow(-1, -1, DataControlRowType.DataRow, DataControlRowState.Normal);

                // Create a TableCell to span all columns
                TableCell buttonCell = new TableCell();
                buttonCell.ColumnSpan = gvBookings.Columns.Count;

                // Create the "Submit Security Fee" button
                Button btnSubmitSecurityFee = new Button
                {
                    ID = "btnSubmitSecurityFee_" + booking["BookingID"].ToString(),
                    Text = "Submit Security Fee",
                    CommandName = "SubmitSecurityFee",
                    CommandArgument = booking["BookingID"].ToString(),
                    CssClass = "btn btn-primary btn-sm"
                };

                // Check if the booking is approved and the security fee has not been submitted
                bool isBookingApproved = booking["IsBookingApproved"].ToString() == "1";
                bool isSecurityFeeSubmitted = !string.IsNullOrEmpty(booking["SecurityFeePath"].ToString());

                // Set the button visibility based on the conditions
                btnSubmitSecurityFee.Visible = isBookingApproved && !isSecurityFeeSubmitted;

                // Add the button to the cell
                buttonCell.Controls.Add(btnSubmitSecurityFee);

                // Add the cell to the new row
                buttonRow.Cells.Add(buttonCell);

                // Insert the new button row after the current data row
                gvBookings.Controls[0].Controls.AddAt(e.Row.RowIndex + 1, buttonRow);
            }
        }*/




        private string GetSecurityFeePath(int bookingID)
        {
            string securityFeePath = string.Empty; // Default value if not found
            string connectionString = ConfigurationManager.ConnectionStrings["LACConnectionString"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                // SQL query to retrieve the SecurityFeePath based on BookingID
                string query = "SELECT SecurityFeePath FROM Bookings WHERE BookingID = @BookingID";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@BookingID", bookingID);
                    conn.Open();

                    // Execute the command and get the value
                    var result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        securityFeePath = result.ToString(); // Get the path as string
                    }
                }
            }

            return securityFeePath; // Return the retrieved path or empty string
        }











        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Response.Redirect("~/Login.aspx");
        }
        protected void btnBookSlot_Click(object sender, EventArgs e)
        {
            Response.Redirect("StartPage.aspx");
        }
    }
}
