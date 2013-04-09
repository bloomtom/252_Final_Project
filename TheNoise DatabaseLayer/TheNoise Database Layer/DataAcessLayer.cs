using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Collections.ObjectModel;
using System.Data;

namespace TheNoise_DatabaseControl
{
    public enum UserAddResult
    {
        Success = 0,
        AlreadyExists = 1,
        UsernameTooLong = 2,
        InvalidPassword = 3
    }

    public enum UserValidationResult
    {
        Success = 0,
        UserNotInDatabase = 1,
        InvalidPassword = 2
    }

    public struct User
    {
        private string username;
        public string Username
        {
            get { return username; }
            set { }
        }
        private string password;
        public string Password
        {
            get { return password; }
            set { }
        }

        public User(string sentUsername, string sentPassword)
        {
            this.username = sentUsername;
            this.password = sentPassword;
        }
    }

    public class DataAccessLayer
    {
        const string TABLE = "userFiles";

        SqlConnectionStringBuilder sqlStrBldr = new SqlConnectionStringBuilder();

        private DataAccessLayer(string databaseAddress, string databaseName, bool useIntegratedSecurity)
        {
            sqlStrBldr.DataSource = databaseAddress;
            sqlStrBldr.InitialCatalog = databaseName;
            sqlStrBldr.IntegratedSecurity = useIntegratedSecurity;
        }
        public DataAccessLayer(string databaseAddress, string databaseName) : this(databaseAddress, databaseName, true) { }
        public DataAccessLayer(string databaseAddress, string databaseName, string username, string password)
            : this(databaseAddress, databaseName, false)
        {
            sqlStrBldr.UserID = username;
            sqlStrBldr.Password = password;
            sqlStrBldr.IntegratedSecurity = false;
        }

        public void readUsers(ObservableCollection<User> userList)
        {
            // DataSet object will store data tables
            DataSet dSetUsers = new DataSet();

            // Data Provider DbConnection object
            SqlConnection dataConnection = new SqlConnection();

            // Data Provider DbCommand object
            SqlCommand dataCommand = new SqlCommand();

            // Data Provider DbDataReader object
            SqlDataReader dataReader;

            // this example builds the connection string using a SqlConnectionStringBuilder object; 
            // easier and less prone to error than building the string yourself. 

            SqlDataAdapter dataAdaptor = new SqlDataAdapter("Select * FROM users", sqlStrBldr.ConnectionString);

            dataAdaptor.Fill(dSetUsers, TABLE);

            dataAdaptor.FillSchema(dSetUsers, SchemaType.Source);

            DataTable dtUsers = dSetUsers.Tables[TABLE];

            foreach (DataRow row in dtUsers.Rows)
            {
                userList.Add(new User(row[0].ToString(), row[1].ToString()));
                //userList.Add(new employee(int.Parse(row[0].ToString()), row[1].ToString(), row[2].ToString(), row[3].ToString(), DateTime.Parse(row[4].ToString()), bool.Parse(row[5].ToString())));
            }
        }

        public UserAddResult addUser(User sentUser)
        {
            UserValidationResult result;

            result = validateUser(sentUser);

            //If validateUser returns 1, user is not in database
            if (result == UserValidationResult.UserNotInDatabase)
            {
                string SQLstring = "INSERT INTO users(username, password) VALUES(" + sentUser.Username + ", " + sentUser.Password + ")";

                executeNonQuery(SQLstring);

                //succeeded. User was added.
                return UserAddResult.Success;
            }
            else
            {
                return UserAddResult.AlreadyExists;
            }
            //

        }

        public UserValidationResult validateUser(User sentUser)
        {
            //0: User is valid.
            //1: Username isn't in the database
            //2: Username is in the database, but the password is incorrect.

            ObservableCollection<User> checkList;

            checkList = new ObservableCollection<User>();

            readUsers(checkList);

            foreach (User u in checkList)
            {
                if (u.Username == sentUser.Username)
                {
                    if (u.Password == sentUser.Password)
                    {
                        //The validation has succeeded, the username and password are valid
                        return UserValidationResult.Success;
                    }
                    else
                    {
                        //The username and is valid, but the password is wrong.
                        return UserValidationResult.InvalidPassword;
                    }
                }

            }

            //The validation has failed, the user is not in the database
            return UserValidationResult.UserNotInDatabase;
        }

        private bool executeNonQuery(string sql)
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
