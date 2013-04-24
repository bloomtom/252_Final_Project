/**
 * Example_8
 * Stored procedures and functions with parameters
 * Requires departments database. 
 **/

using System;
using System.Collections.Generic;
using System.Text;

using System.Data.SqlClient; // for SqlDataAdapter
using System.Data;           // for DataSet

namespace Example_4
{
    class Program
    {
        static void Main(string[] args)
        {
          
            string connectionString = "Integrated Security=true;" +
                                "database=userFiles;" +
                                "Data Source=.;"; // local machine

            SqlConnectionStringBuilder sqlStrBldr = new SqlConnectionStringBuilder(connectionString);
            sqlStrBldr.UserID = "sa";
            sqlStrBldr.Password = "nhti";

            SqlConnection connection = new SqlConnection(sqlStrBldr.ConnectionString); //  new SqlConnection(connectionString);

            SqlCommand testCommand = new SqlCommand("INSERT INTO musicUsers VALUES('Test5', 'Test5PW')");

            testCommand.Connection = connection;
            testCommand.Connection.Open();

            testCommand.ExecuteNonQuery();

            testCommand.Connection.Close();

            Console.WriteLine(testCommand.ToString());


            SqlCommand command = new SqlCommand("checkMusicUser", connection);
            command.CommandType = CommandType.StoredProcedure;

            SqlParameter userName = new SqlParameter("@uname", SqlDbType.VarChar);
            userName.Direction = ParameterDirection.Input;
            userName.Value = "Test1";
            userName.Size = 50;
            command.Parameters.Add(userName);

            SqlParameter password = new SqlParameter("@passw", SqlDbType.VarChar);
            password.Direction = ParameterDirection.Input;
            password.Value = "Test1PW";
            password.Size = 50;
            command.Parameters.Add(password);

            SqlParameter returnValue = new SqlParameter("@retval", SqlDbType.Int);
            returnValue.Direction = ParameterDirection.Output;
            command.Parameters.Add(returnValue);

            

            command.Connection.Open();

            command.ExecuteNonQuery();

            Console.WriteLine("TEST");
            Console.WriteLine("Return result: " + command.Parameters["@retval"].Value.ToString());
            Console.ReadKey();
        }

        private bool executeNonQuery(string sql, SqlConnectionStringBuilder sqlStrBldr)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(sqlStrBldr.ConnectionString))
                {
                    SqlCommand command = new SqlCommand(sql, connection);
                    command.Connection.Open();
                    if (command.ExecuteNonQuery() < 1)
                    {
                        return false;
                    }
                }
            }
            catch
            {
                return false;
            }

            return true;
        }
    }
}
