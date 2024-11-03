using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ReservationSystem
{
    public partial class ComplexSelection : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserID"] == null)
            {
                Response.Redirect("NoLogin.aspx");
            }
        }
        protected void rblComplex_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedComplex = rblComplex.SelectedValue;
            Session["SelectedComplex"] = selectedComplex;
            Response.Redirect("FacilitySelection.aspx");
        }
    }
}