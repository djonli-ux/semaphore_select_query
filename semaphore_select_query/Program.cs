
using Microsoft.Data.SqlClient;
using System.Data;

string connString = @"Server=DESKTOP-TO8EGSF;Database=books;Trusted_Connection=True;Encrypt=False;";
string query = @"SELECT * FROM users;";

Semaphore semaphore = new Semaphore(0, 5);
object consoleLock = new object();

for (int i = 1; i <= 90; ++i)
{
    int n = i;
    Thread t = new Thread(() => RunSelectQuery(n));
    t.Start();
}

Thread.Sleep(5000);
semaphore.Release(5);

void RunSelectQuery(int id) 
{
    Console.WriteLine($"Thread {id} started");
    semaphore.WaitOne();

    using (SqlConnection conn = new SqlConnection(connString))
    {
        try
        {
            conn.Open();

            SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
            DataSet dataSet = new DataSet();

            adapter.Fill(dataSet);

            lock (consoleLock) 
            { 
                foreach (DataTable dt in dataSet.Tables) 
                {
                    Console.WriteLine(dt.TableName);
                    foreach (DataColumn col in dt.Columns)
                        Console.Write($"\t{col.ColumnName}");
                    Console.WriteLine("\n");
                    foreach (DataRow row in dt.Rows)
                    {
                        var cells = row.ItemArray;
                        foreach(object cell in cells)
                            Console.Write("\t{0}", cell);
                        Console.WriteLine();
                    }
                }
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
    semaphore.Release();
}
