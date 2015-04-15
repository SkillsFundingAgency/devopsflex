namespace DevOpsFlex.Data
{
    using Naming;

    public static class DataConfiguration
    {
        public static IName<T> GetNaming<T>()
            where T : DevOpsComponent
        {
            return new DefaultNaming<T>();
        }
    }
}
