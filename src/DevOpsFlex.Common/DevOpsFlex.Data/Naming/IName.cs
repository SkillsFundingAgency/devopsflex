namespace DevOpsFlex.Data.Naming
{
    public interface IName<in T>
        where T : DevOpsComponent
    {
        string GetSlotName(T component);

        string GetMinimalSlotName(T component);
    }
}
