namespace DevOpsFlex.InspectSolution
{
    public class StyleCopSettingsFile
    {
        public string FilePath { get; private set; }

        public StyleCopSettingsFile(string filePath)
        {
            FilePath = filePath;

            // TODO: Ideally this would use the StyleCop NuGet package and use their own object model to parse the file (this requires investigating that they don't do funny things like declare all internals/private).
        }
    }

    public interface IStyleCopSettingsFileFactory
    {
        StyleCopSettingsFile Create(string filePath);

        void Release(StyleCopSettingsFile styleCopSettingsFile);
    }
}
