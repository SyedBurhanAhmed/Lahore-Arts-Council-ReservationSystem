using System;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

namespace ReservationSystem
{
    public partial class FacilitySelection : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserID"] == null)
            {
                Response.Redirect("NoLogin.aspx");
            }

            if (!IsPostBack)
            {
                LoadFacilities();
            }
        }

        private void LoadFacilities()
        {
            string selectedComplex = Session["SelectedComplex"].ToString();
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["LACConnectionString"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("SELECT FacilityID, FacilityName FROM Facilities WHERE Complex = @Complex", conn);
                cmd.Parameters.AddWithValue("@Complex", selectedComplex);

                conn.Open();
                ddlFacilities.DataSource = cmd.ExecuteReader();
                ddlFacilities.DataTextField = "FacilityName";
                ddlFacilities.DataValueField = "FacilityID";
                ddlFacilities.DataBind();
            }

            // Insert a default "Select Facility" item
            ddlFacilities.Items.Insert(0, new ListItem("Select Facility", ""));
        }


        protected void ddlFacilities_SelectedIndexChanged(object sender, EventArgs e)
        {
            string facilityID = ddlFacilities.SelectedValue;
            DisplayFacilityDetails(facilityID);
        }

        private void DisplayFacilityDetails(string facilityID)
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["LACConnectionString"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("SELECT Capacity, RatePerHour, SecurityFees,FacilityName FROM Facilities WHERE FacilityID = @FacilityID", conn);
                cmd.Parameters.AddWithValue("@FacilityID", facilityID);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    lblCapacity.Text = "Capacity: " + reader["Capacity"].ToString();
                    lblRatePerHour.Text = "Rate per Hour: " + reader["RatePerHour"].ToString();
                    lblSecurityFees.Text = "Security Fees: " + reader["SecurityFees"].ToString();
                    Session["SelectedFacilityName"] = reader["FacilityName"].ToString();
                }
            }
        }

        protected void btnNext_Click(object sender, EventArgs e)
        {
            // Check if the user has selected a facility
            if (ddlFacilities.SelectedValue == "")
            {
                // Display an error message to prompt the user to select a facility
                lblErrorMessage.Text = "Please select a facility before proceeding.";
                lblErrorMessage.Visible = true; // Make sure the label is visible
                return; // Stop further execution
            }

            // If a facility is selected, proceed to the next page
            Session["SelectedFacility"] = ddlFacilities.SelectedValue;

            Response.Redirect("DateSelection.aspx");
        }

    }
}
