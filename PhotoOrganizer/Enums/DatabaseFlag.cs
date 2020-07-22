namespace PhotoOrganizer.Enums
{
    /// <summary>
    /// Specifies the available choices for databases.
    /// </summary>
    public enum DatabaseFlag
    {
        /// <summary>
        /// Mark SQLite as database choice.
        /// </summary>
        /// <remarks>Default choice</remarks>
        SQLite,
        /// <summary>
        /// Mark MySQL as database choice.
        /// </summary>
        MySQL,
        /// <summary>
        /// Mark PostgreSQL as database choice.
        /// </summary>
        PostgreSQL
    }
}
