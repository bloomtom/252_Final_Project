using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Collections.ObjectModel;
using System.Data;
using TheNoise_SharedObjects;
using TheNoise_SharedObjects.GlobalEnumerations;

namespace TheNoise_DatabaseControl
{
    //public struct User
    //{
    //    private string username;
    //    public string Username
    //    {
    //        get { return username; }
    //        set { }
    //    }
    //    private string password;
    //    public string Password
    //    {
    //        get { return password; }
    //        set { }
    //    }

    //    public User(string sentUsername, string sentPassword)
    //    {
    //        this.username = sentUsername;
    //        this.password = sentPassword;
    //    }
    //}

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

        public void readUsers(ObservableCollection<LoginData> userList)
        {
            // DataSet object will store data tables
            DataSet dSetUsers = new DataSet();

            // Data Provider DbConnection object
            SqlConnection dataConnection = new SqlConnection();

            // Data Provider DbCommand object
            SqlCommand dataCommand = new SqlCommand();

            // Data Provider DbDataReader object
            //SqlDataReader dataReader;

            // this example builds the connection string using a SqlConnectionStringBuilder object; 
            // easier and less prone to error than building the string yourself. 

            SqlDataAdapter dataAdaptor = new SqlDataAdapter("Select * FROM users", sqlStrBldr.ConnectionString);

            dataAdaptor.Fill(dSetUsers, TABLE);

            dataAdaptor.FillSchema(dSetUsers, SchemaType.Source);

            DataTable dtUsers = dSetUsers.Tables[TABLE];

            foreach (DataRow row in dtUsers.Rows)
            {
                userList.Add(new LoginData(row[0].ToString(), row[1].ToString()));
                //userList.Add(new employee(int.Parse(row[0].ToString()), row[1].ToString(), row[2].ToString(), row[3].ToString(), DateTime.Parse(row[4].ToString()), bool.Parse(row[5].ToString())));
            }
        }

        public UserAddResult addUser(LoginData sentUser)
        {
            //GlobalEnumerations.UserValidationResult result;
            TheNoise_SharedObjects.GlobalEnumerations.UserAuthenticationResult result;
            result = validateUser(sentUser);

            //If validateUser returns 1, user is not in database
            if (result == UserAuthenticationResult.InvalidUser)
            {
                string SQLstring = "INSERT INTO users(username, password) VALUES(" + sentUser.username + ", " + sentUser.password + ")";

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

        public UserAuthenticationResult validateUser(LoginData sentUser)
        {
            //0: User is valid.
            //1: Username isn't in the database
            //2: Username is in the database, but the password is incorrect.

            ObservableCollection<LoginData> checkList;

            checkList = new ObservableCollection<LoginData>();

            readUsers(checkList);

            foreach (LoginData u in checkList)
            {
                if (u.username == sentUser.username)
                {
                    if (u.password == sentUser.password)
                    {
                        //The validation has succeeded, the username and password are valid
                        return UserAuthenticationResult.Success;
                    }
                    else
                    {
                        //The username and is valid, but the password is wrong.
                        return UserAuthenticationResult.InvalidPassword;
                    }
                }

            }

            //The validation has failed, the user is not in the database
            return UserAuthenticationResult.InvalidUser;
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
