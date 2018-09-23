namespace Pipeline
{
    public class CompilerOptions : ICompilerOptions
    {
        public bool ShowConcreteTree { get; set; }
        public bool ShowAbstractTree { get; set; }
        public string InputFile { get; set; }
        public string OutputFolder { get; set; }
    }
}
