using System;
using System.Web.UI;

namespace ReservationSystem
{
    public partial class Confirmation : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserID"] == null)
            {
                Response.Redirect("NoLogin.aspx");
            }

            if (!IsPostBack)
            {
                // Retrieve session data and set label values
                lblName.Text = Session["UserName"] != null ? Session["UserName"].ToString() : "Not Provided";
                lblTopic.Text = Session["EventTopic"] != null ? Session["EventTopic"].ToString() : "Not Provided";
                lblFacilityName.Text = Session["SelectedFacilityName"] != null ? Session["SelectedFacilityName"].ToString() : "Not Provided";
                lblComplexName.Text = Session["SelectedComplex"] != null ? Session["SelectedComplex"].ToString() : "Not Provided";
                lblFirstBookingDate.Text = Session["FirstBookingDate"] != null ? ((DateTime)Session["FirstBookingDate"]).ToString("MMMM dd, yyyy") : "Not Provided";
                lblLastBookingDate.Text = Session["LastBookingDate"] != null ? ((DateTime)Session["LastBookingDate"]).ToString("MMMM dd, yyyy") : "Not Provided";


                // Format the time slot with start and end time
                if (Session["StartTime"] != null && Session["EndTime"] != null)
                {
                    lblTimeSlot.Text = $"{Session["StartTime"]} to {Session["EndTime"]}";
                }
                else
                {
                    lblTimeSlot.Text = "Not Provided";
                }

                // Display the total charges if available
               // lblTotalCharges.Text = Session["TotalCharges"] != null ? Session["TotalCharges"].ToString() : "Not Provided";
            }
        }

        // Event handler for the "Reserve Another Slot" button
        protected void btnDashboard_Click(object sender, EventArgs e)
        {
            // Redirect to the start page to make another reservation
            Response.Redirect("UserDashboard.aspx");
        }
    }
}
