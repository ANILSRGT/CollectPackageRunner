public class Singleton<T> where T : class, new()
{
    private static volatile Singleton<T> _instance;
    private static readonly object _lock = new object();

    public static Singleton<T> Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new Singleton<T>();
                    }
                }
            }
            return _instance;
        }
    }
}