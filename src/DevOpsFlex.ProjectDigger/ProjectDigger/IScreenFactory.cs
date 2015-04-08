namespace ProjectDigger
{
    using Caliburn.Micro;

    public interface IScreenFactory<out T>
        where T : Screen
    {
        T Create();

        void Release(Screen screen);
    }
}
