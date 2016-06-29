namespace CSharpCodeGenerator.Deployer
{
    public struct SourceCodeFile
    {
        public readonly string Name;
        public readonly string Content;
        public SourceCodeFile(string name, string content)
        {
            Name = name;
            Content = content;
        }
        public SourceCodeFile(SourceCodeFile f)
        {
            Name = f.Name;
            Content = f.Content;
        }
    }
}
