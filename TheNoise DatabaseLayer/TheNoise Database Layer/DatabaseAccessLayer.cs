using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Collections.ObjectModel;
using System.Data;
using TheNoiseHLC;
using TheNoiseHLC.CommunicationObjects;
using TheNoiseHLC.CommunicationObjects.GlobalEnumerations;

namespace TheNoise_DatabaseControl
{
    public class DataAccessLayer : IDisposable
    {
        private readonly string dbNameAndTable = "userFiles";
        private SqlConnectionStringBuilder sqlStrBldr = new SqlConnectionStringBuilder();

        /// <summary>
        /// Generic initialization of the <see cref="DataAccessLayer"/> class. Don't expect this to work with no connection details!
        /// </summary>
        public DataAccessLayer()
        {
            sqlStrBldr.InitialCatalog = dbNameAndTable;
            sqlStrBldr.IntegratedSecurity = true;
            sqlStrBldr.InitialCatalog = dbNameAndTable;
        }
        public DataAccessLayer(string databaseAddress, string databaseName, string username, string password, bool useIntegratedSecurity)
        {
            dbNameAndTable = databaseName;

            sqlStrBldr.DataSource = databaseAddress;
            sqlStrBldr.InitialCatalog = databaseName;
            sqlStrBldr.UserID = username;
            sqlStrBldr.Password = password;
            sqlStrBldr.IntegratedSecurity = useIntegratedSecurity;
            sqlStrBldr.InitialCatalog = dbNameAndTable;
        }
        public DataAccessLayer(string databaseAddress, string databaseName) : this(databaseAddress, databaseName, string.Empty, string.Empty, true) { }
        public DataAccessLayer(string databaseAddress, string databaseName, string username, string password)
            : this(databaseAddress, databaseName, username, password, false)
        {
        }

        /// <summary>
        /// Adds a user to the users table if the requested username doesn't exist.
        /// </summary>
        /// <param name="sentUser">The user to register in the database.</param>
        /// <returns>UserAddResult giving feedback on the process.</returns>
        public UserAddResult addUser(LoginData sentUser)
        {
            //GlobalEnumerations.UserValidationResult result;
            UserAuthenticationResult preCheck;
            preCheck = validateUser(sentUser);

            //If validateUser returns 1, user is not in database
            if (preCheck == UserAuthenticationResult.InvalidUser)
            {
                using (SqlConnection connection = new SqlConnection(sqlStrBldr.ConnectionString))
                {
                    SqlCommand command = new SqlCommand("addMusicUser", connection);
                    command.CommandType = CommandType.StoredProcedure;

                    SqlParameter userName = new SqlParameter("@uname", SqlDbType.VarChar);
                    userName.Direction = ParameterDirection.Input;
                    userName.Value = sentUser.username;
                    userName.Size = 50;
                    command.Parameters.Add(userName);

                    SqlParameter password = new SqlParameter("@passw", SqlDbType.VarChar);
                    password.Direction = ParameterDirection.Input;
                    password.Value = sentUser.password;
                    password.Size = 80;
                    command.Parameters.Add(password);

                    command.Connection.Open();
                    command.ExecuteNonQuery();
                }

                // Check user again  to make sure things went well.
                // This is a hack because the database doesn't give you feedback on addMusicUser...
                UserAddResult result = UserAddResult.UnknownResult;
                UserAuthenticationResult postCheck;
                postCheck = validateUser(sentUser);
                switch (postCheck)
                {
                    case UserAuthenticationResult.UnknownResult:
                        break;
                    case UserAuthenticationResult.Success:
                        result = UserAddResult.Success;
                        break;
                    case UserAuthenticationResult.InvalidUser:
                        break;
                    case UserAuthenticationResult.InvalidPassword:
                        break;
                    default:
                        break;
                }
                return result;
            }
            else
            {
                return UserAddResult.AlreadyExists;
            }
        }

        /// <summary>
        /// Validates the passed username and password against the database.
        /// </summary>
        /// <param name="sentUser">The user to validate</param>
        /// <returns>Result of authentication request indicating success or failure mode.</returns>
        public UserAuthenticationResult validateUser(LoginData sentUser)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(sqlStrBldr.ConnectionString))
                {
                    SqlCommand command = new SqlCommand("checkMusicUser", connection);
                    command.CommandType = CommandType.StoredProcedure;

                    SqlParameter userName = new SqlParameter("@uname", SqlDbType.VarChar);
                    userName.Direction = ParameterDirection.Input;
                    userName.Value = sentUser.username;
                    userName.Size = 50;
                    command.Parameters.Add(userName);

                    SqlParameter password = new SqlParameter("@passw", SqlDbType.VarChar);
                    password.Direction = ParameterDirection.Input;
                    password.Value = sentUser.password;
                    password.Size = 80;
                    command.Parameters.Add(password);

                    SqlParameter returnValue = new SqlParameter("@retval", SqlDbType.Int);
                    returnValue.Direction = ParameterDirection.Output;
                    command.Parameters.Add(returnValue);

                    command.Connection.Open();
                    command.ExecuteNonQuery();

                    return (UserAuthenticationResult)(int)command.Parameters["@retval"].Value;
                }
            }
            catch
            {
                return UserAuthenticationResult.UnknownResult;
            }
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

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <exception cref="System.Net.Sockets.SocketException">An exception was generated while closing the TcpListener.</exception>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Dispose all managed resources.
                
            }

            // Dispose of unmanaged resources here if any.
        }
    }
}
