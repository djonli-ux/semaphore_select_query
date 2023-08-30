
using Microsoft.Data.SqlClient;
using System.Data;

string connString = @"Server=DESKTOP-TO8EGSF;Database=books;Trusted_Connection=True;Encrypt=False;";
Semaphore semaphore = new Semaphore(5, 5);

for (int i = 1; i <= 50; ++i) 
{
    int n = 1;
    Thread t = new Thread(RunSelectQuery);
    t.Start();
}

void RunSelectQuery() 
{
    using (SqlConnection conn = new SqlConnection(connString))
    {
        try
        {
            conn.Open();

            string query = @"SELECT * FROM users;";

            SqlCommand cmd = new SqlCommand()
            {
                Connection = conn,
                CommandType = CommandType.Text,
                CommandText = query
            };

            SqlDataReader reader = cmd.ExecuteReader();

            DataTable dt = new DataTable();
            dt.Load(reader);

            foreach (DataRow dr in dt.Rows)
            {
                foreach (DataColumn dc in dt.Columns)
                {
                    Console.Write(dr[dc] + " ");
                }
                Console.WriteLine();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error:{ex.Message}");
        }
        finally
        {
            if (conn.State == ConnectionState.Open)
                conn.Close();
        }
    }
}
