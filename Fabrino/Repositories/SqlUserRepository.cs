using System;
using Microsoft.Data.SqlClient;
using System.Configuration;

public class SqlUserRepository : IUserRepository
{
    private string connectionString = ConfigurationManager.ConnectionStrings["FabrinoConnection"].ConnectionString;

    public bool IsValidUser(string username, string passwordHash)
    {
        bool isValid = false;

        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            conn.Open();

            string query = "SELECT COUNT(*) FROM users WHERE username = @username AND password_hash = @password";
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@username", username);
                cmd.Parameters.AddWithValue("@password", passwordHash);

                int count = Convert.ToInt32(cmd.ExecuteScalar());
                isValid = count > 0;
            }
        }

        return isValid;
    }
}
