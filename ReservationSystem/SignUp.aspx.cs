using System;
using System.Data.SqlClient;
using System.Web.UI;

namespace ReservationSystem
{
    public partial class SignUp : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void btnSignUp_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                string userName = txtName.Text.Trim();
                string fatherName = txtFatherName.Text.Trim();
                string userCNIC = txtCNIC.Text.Trim();
                string phoneNumber = txtPhoneNumber.Text.Trim();
                string email = txtEmail.Text.Trim();
                string password = txtPassword.Text.Trim();
                string confirmPassword = txtConfirmPassword.Text.Trim();

                // Check for missing input fields
                if (string.IsNullOrEmpty(userName))
                {
                    ShowError("Name is required.");
                    return;
                }
                if (string.IsNullOrEmpty(fatherName))
                {
                    ShowError("Father's Name is required.");
                    return;
                }
                if (string.IsNullOrEmpty(userCNIC))
                {
                    ShowError("CNIC is required.");
                    return;
                }
                if (!System.Text.RegularExpressions.Regex.IsMatch(userCNIC, @"^\d{5}-\d{7}-\d$"))
                {
                    ShowError("Invalid CNIC Format.");
                    return;
                }
                if (string.IsNullOrEmpty(phoneNumber))
                {
                    ShowError("Phone number is required.");
                    return;
                }
                if (!System.Text.RegularExpressions.Regex.IsMatch(phoneNumber, @"^\d{4}-\d{7}$"))
                {
                    ShowError("Invalid phone number format. Correct format: xxxx-xxxxxxx.");
                    return;
                }
                if (string.IsNullOrEmpty(email))
                {
                    ShowError("Email is required.");
                    return;
                }
                if (!IsValidEmail(email) || !System.Text.RegularExpressions.Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                {
                    ShowError("Invalid email format.");
                    return;
                }
                if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassword))
                {
                    ShowError("Password and Confirm Password are required.");
                    return;
                }
                if (password != confirmPassword)
                {
                    ShowError("Passwords do not match.");
                    return;
                }

                phoneNumber = phoneNumber.Replace("-", "");

                // If validations pass, save the user data
                if (UserExists(userCNIC, email))
                {
                    ShowError("User with this CNIC or Email already exists. Please log in.");
                    return;
                }

                SaveUserDetails(userName, fatherName, userCNIC, phoneNumber, email, password);

                // Redirect to a welcome page or display success message
                Response.Redirect("Login.aspx");
            }
            else
            {
                ShowError("Form validation failed. Please check your input.");
            }
        }

        private void ShowError(string message)
        {
            lblErrorMessage.Text = message;
            lblErrorMessage.CssClass = "error-message"; // Red error message class
            lblErrorMessage.Visible = true;
        }

        private bool UserExists(string cnic, string email)
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["LACConnectionString"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT COUNT(*) FROM Users WHERE CNIC = @CNIC OR Email = @Email";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@CNIC", cnic);
                command.Parameters.AddWithValue("@Email", email);

                connection.Open();
                int userCount = (int)command.ExecuteScalar();
                return userCount > 0;
            }
        }

        private void SaveUserDetails(string name, string fatherName, string cnic, string phoneNumber, string email, string password)
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["LACConnectionString"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO Users (Name, FatherName, CNIC, PhoneNumber, Email, Password) VALUES (@Name, @FatherName, @CNIC, @PhoneNumber, @Email, @Password)";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Name", name);
                command.Parameters.AddWithValue("@FatherName", fatherName);
                command.Parameters.AddWithValue("@CNIC", cnic);
                command.Parameters.AddWithValue("@PhoneNumber", phoneNumber);
                command.Parameters.AddWithValue("@Email", email);
                command.Parameters.AddWithValue("@Password", password); // Ensure to hash this password in production

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        private bool IsValidEmail(string email)
        {
            // Add email validation logic here (if needed)
            return true;
        }
    }
}
