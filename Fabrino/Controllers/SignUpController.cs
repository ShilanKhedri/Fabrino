using Fabrino.Models;
using System;
using Microsoft.Data.SqlClient;
using System.Windows;

namespace Fabrino.Controllers
{
    public class SignUpController
    {
        private string connectionString = "Server=SHILAN;Database=Fabrino;Trusted_Connection=True;Encrypt=False;TrustServerCertificate=True;";

        public bool RegisterUser(UserModel user)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = @"INSERT INTO Users 
                                     (username, full_name, password_hash, security_question, security_answer_hash, role) 
                                     VALUES 
                                     (@username, @fullname, @password_hash, @security_question, @security_answer, @role)";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@username", user.username);
                        command.Parameters.AddWithValue("@fullname", user.full_name);
                        command.Parameters.AddWithValue("@password_hash", user.password_hash);
                        command.Parameters.AddWithValue("@security_question", user.security_question);
                        command.Parameters.AddWithValue("@security_answer", user.security_answer_hash);
                        command.Parameters.AddWithValue("@role", user.role);


                        command.ExecuteNonQuery();
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("خطا در ثبت نام: " + ex.Message);
                return false;
            }
        }
    }
}
