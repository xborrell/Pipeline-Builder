namespace Pipeline
{
    public interface ICompilerOptions
    {
        string InputFile { get; set; }
        string OutputFolder { get; set; }
        bool ShowAbstractTree { get; set; }
        bool ShowConcreteTree { get; set; }
    }
}