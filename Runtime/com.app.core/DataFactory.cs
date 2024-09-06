public static class DataFactory
{
    public interface IDataFactory
    {
        T Get<T>() where T : new();
        System.Collections.Generic.List<T> GetList<T>();
        void Release(object obj);
        void ForceClear<T>();
        void Clear();
    }

    class DefaultDataFactory: IDataFactory
    {
        public T Get<T>() where T : new()
        {
            return new T();
        }

        public System.Collections.Generic.List<T> GetList<T>() 
        {
            return Get<System.Collections.Generic.List<T>>();
        }

        public void Release(object obj)
        {

        }
        public void ForceClear<T>()
        {

        }
        public void Clear()
        {

        }
    }

    private static IDataFactory Factory;
    private static IDataFactory DefaultFactory;
    static DataFactory()
    {
        DefaultFactory = new DefaultDataFactory();
        Factory = DefaultFactory;
    }

    public static void SetDataFactory(IDataFactory dataFactory)
    {
        Factory = dataFactory ?? DefaultFactory;
    }

    public static T Get<T>() where T : new()
    {
        return Factory.Get<T>();
    }

    public static System.Collections.Generic.List<T> GetList<T>()
    {
        return Factory.Get<System.Collections.Generic.List<T>>();
    }

    public static void Release(object obj)
    {
        Factory.Release(obj);
    }

    public static void ForceClear<T>()
    {
        Factory.ForceClear<T>();
    }

    public static void Clear()
    {
        Factory.Clear();
    }
}
