using NSpec;
using CSharpCodeGenerator;
using Tests;

class describe_SolutionParser : nspec
{

    void when_providing ()
    {

        context ["parse project from solutionFile"] = () => {
            it ["does return empty array when solution does not contain projects"] = () => {
                SolutionParser.ParseProjectsFromSolution (string.Empty).should_be_empty ();
            };

            it ["parses projects from solution string"] = () => {
                var projects = SolutionParser.ParseProjectsFromSolution (SolutionFixtures.SOLUTION_FILE);

                Project expectedProject1 = new Project {
                    ProjectName = "CSharpCodeGenerator",
                    RelativePath = @"CSharpCodeGenerator.csproj",
                    ProjectType = "cs"
                };
                Project expectedProject2 = new Project {
                    ProjectName = "Tests",
                    RelativePath = @"Tests\Tests.csproj",
                    ProjectType = "cs"
                };

                projects.Length.should_be (2);
                projects.should_contain (p => p.Equals (expectedProject1));
                projects.should_contain (p => p.Equals (expectedProject2));
            };
        };

        context ["parse files from projectFiles"] = () => {

            it ["does not return files when project does not contain files"] = () => {
                SolutionParser.ParseSourceFilesFromProject (string.Empty).Length.should_be (0);
            };

            it ["does return files from project file with correct values"] = () => {

                var files = SolutionParser.ParseSourceFilesFromProject (@"
  <ItemGroup>
    <Compile Include=""Example Tests\describe_RoslynComponentInfoProvider.cs"" />
    <Compile Include=""describe_SolutionParser.cs"" />
  </ItemGroup>");
                
                var expectedFile1 = new SolutionParser.ProjectFile () {
                    BuildAction = SolutionParser.BuildActionProperty.Compile,
                    FilePath = "Example Tests\\describe_RoslynComponentInfoProvider.cs",
                    FileType = "cs"
                };
                var expectedFile2 = new SolutionParser.ProjectFile () {
                    BuildAction = SolutionParser.BuildActionProperty.Compile,
                    FilePath = "describe_SolutionParser.cs",
                    FileType = "cs"
                };


                files.Length.should_be (2);
                files.should_contain (f => f.Equals (expectedFile1));
            };

            it ["does return files from complex projectFile"] = () => {
                var files = SolutionParser.ParseSourceFilesFromProject (SolutionFixtures.PROJECT_FILE);
                files.Length.should_be (2);
            };

            it ["does only parse source code files"] = () => {
                var files = SolutionParser.ParseSourceFilesFromProject (@"
  <ItemGroup>
    <None Include=""Libraries\NSpec\NSpec.dll"" />
    <None Include=""Libraries\NSpec\NSpec.exe"" />
  </ItemGroup>");
                files.Length.should_be (0);
                //files.Where (p => p.);
            };

            /*
  <ItemGroup>
    <Compile Include=""Example Tests\describe_RoslynComponentInfoProvider.cs"" />
    <Compile Include=""describe_SolutionParser.cs"" />
  </ItemGroup>
  <ItemGroup>
    <None Include=""Libraries\NSpec\NSpec.dll"" />*/
        };
    }

}

