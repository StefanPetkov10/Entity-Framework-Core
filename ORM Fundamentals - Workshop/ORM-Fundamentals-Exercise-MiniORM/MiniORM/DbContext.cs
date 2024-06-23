namespace MiniORM
{
    public abstract class DbContext
    {
        private readonly DatabaseConnection _connection;

        protected DbContext(string connectionString)
        {
            this._connection = new DatabaseConnection(connectionString);
        }
        internal static HashSet<Type> AllowedSqlTypes { get; } = new HashSet<Type>
        {
            typeof(string),
            typeof(int),
            typeof(uint),
            typeof(long),
            typeof(ulong),
            typeof(decimal),
            typeof(bool),
            typeof(DateTime)
        };

    }
}
