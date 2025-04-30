using Fabrino.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using System.Configuration;

namespace Fabrino.Controllers
{
    class AuthController
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["FabrinoConnection"].ConnectionString;

        public bool IsValidUser(UserModel user)
        {
            bool isValid = false;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string query = "SELECT COUNT(*) FROM users WHERE username = @username AND password_hash = @password";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@username", user.username);
                    cmd.Parameters.AddWithValue("@password", user.password_hash);


                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    isValid = count > 0;
                }
            }

            return isValid;

        }
    }

}

