namespace MiniORM
{
    public class ChangeTracker<T>
        where T : class, new()
    {
        private readonly List<T> _allEntities;
        private readonly List<T> _added;
        private readonly List<T> _removed;

        public ChangeTracker(IEnumerable<T> entities)
        {
            if (entities is null) throw new ArgumentNullException(nameof(entities));

            this._added = new List<T>();
            this._removed = new List<T>();
            this._allEntities = CloneEntities(entities).ToList();
        }

        public IReadOnlyCollection<T> Added => _added.AsReadOnly();
        public IReadOnlyCollection<T> Removed => _removed.AsReadOnly();
        public IReadOnlyCollection<T> AllEntities => _allEntities.AsReadOnly();


        public void Add(T item)
        {
            if (item is null) throw new ArgumentNullException(nameof(item));
            this._added.Add(item);
        }

        public void Remove(T item)
        {
            if (item is null) throw new ArgumentNullException(nameof(item));
            this._removed.Add(item);
        }
        private static IEnumerable<T> CloneEntities(IEnumerable<T> entities)
        {
            var properties = typeof(T).GetAllowedSqlProperties();

            List<T> result = new();

            foreach (var entity in entities)
            {
                //var copy = Activator.CreateInstance<T>();
                var copy = new T();//where T : class, new() we can use new T() instead of Activator.CreateInstance<T>()

                foreach (var property in properties)
                {
                    var value = property.GetValue(entity);
                    property.SetValue(copy, value);
                }

                result.Add(copy);
            }

            return result;
        }

    }
}