namespace PhotoOrganizer.Models
{
    /// <summary>
    /// Object for storing common information about connection strings.
    /// </summary>
    public class ConnectionStringInfo
    {
        /// <summary>
        /// UserId/Username for database.
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// Password for User.
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// IP or hostname to database server.
        /// </summary>
        public string Server { get; set; }
        /// <summary>
        /// Port the database is running on.
        /// </summary>
        public int Port { get; set; }
        /// <summary>
        /// Database to connect to.
        /// </summary>
        public string Database { get; set; }
    }
}
