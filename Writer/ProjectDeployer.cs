using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.MSBuild;

namespace CSharpCodeGenerator.Deployer
{
    public class ProjectDeployer
    {
        internal Project Project;
        internal string[] TargetPathFolders;

        public ProjectDeployer(string projectPath, string targetFolderPath)
        {
            var workspace = MSBuildWorkspace.Create();
            
            var project = workspace.OpenProjectAsync(projectPath).Result;
            
            Project = project;
            TargetPathFolders = targetFolderPath.Split('/');
        }
        internal ProjectDeployer(Project project, string targetFolderPath)
        {
            Project = project;
            TargetPathFolders = targetFolderPath.Split('/');
        }

        public void AddFile(SourceCodeFile file)
        {
            var document = Project.AddDocument(file.Name, file.Content, TargetPathFolders);
            Project = document.Project;
        }

        public void AddFileWithFormatting(SourceCodeFile file)
        {
            var fileRootNode = CSharpSyntaxTree.ParseText(file.Content).GetRoot();
            fileRootNode = Formatter.Format(fileRootNode, Project.Solution.Workspace);

            var document = Project.AddDocument(file.Name, fileRootNode, TargetPathFolders);
            Project = document.Project;
        }

        public void ClearTargetFolder()
        {
            var documentIds = Project.Documents.Where(IsWithinTargetFolder).Select(d => d.Id);
            foreach (var id in documentIds)
            {
                Project = Project.RemoveDocument(id);
            }
        }

        private bool IsWithinTargetFolder(Document d)
        {
            for(var i = 0; i < TargetPathFolders.Length; i++)
            {
                if (d.Folders.Count <= i ||
                    TargetPathFolders[i] != d.Folders[i])
                {
                    return false;
                }
            }
            return true;
        }

        public bool TryApplyChangesToProject()
        {
            return Project.Solution.Workspace.TryApplyChanges(Project.Solution);
        }
    }
}
