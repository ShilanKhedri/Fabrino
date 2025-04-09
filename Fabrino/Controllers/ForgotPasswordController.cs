using System;
using System.Configuration;
using Microsoft.Data.SqlClient;
using Fabrino.Helpers;

namespace Fabrino.Controllers
{
    public class ForgotPasswordController
    {
        private string connectionString;

        public ForgotPasswordController()
        {
            connectionString = ConfigurationManager.ConnectionStrings["FabrinoConnection"]?.ConnectionString;
        }

        public string GetSecurityQuestion(string username)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT security_question FROM users WHERE Username = @Username";
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@Username", username);
                connection.Open();

                var result = cmd.ExecuteScalar();
                return result?.ToString();
            }
        }

        public bool ValidateSecurityAnswer(string username, string answer)
        {
            string hashedAnswer = SecurityHelper.ComputeSha256Hash(answer);

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT COUNT(*) FROM users WHERE username = @Username AND security_answer_hash = @Answer";
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@Username", username);
                cmd.Parameters.AddWithValue("@Answer", hashedAnswer);
                connection.Open();

                int count = (int)cmd.ExecuteScalar();
                return count > 0;
            }
        }

        public bool ResetPassword(string username, string newPassword)
        {
            string hashedPassword = SecurityHelper.ComputeSha256Hash(newPassword);

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "UPDATE users SET password_hash = @Password WHERE Username = @Username";
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@Password", hashedPassword);
                cmd.Parameters.AddWithValue("@Username", username);
                connection.Open();

                int rows = cmd.ExecuteNonQuery();
                return rows > 0;
            }
        }
    }
}
