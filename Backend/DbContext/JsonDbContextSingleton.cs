using Backend.DbContext.Interfaces;

namespace Backend.DbContext
{
    public class JsonDbContextSingleton
    {
        private static IDbContext? _instance;

        public static IDbContext GetInstance()
        {
            if (_instance is null)
                _instance = new JsonDbContext();

            return _instance;
        }
    }
}
