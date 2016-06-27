using System;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;

namespace CSharpCodeGenerator.Writer
{
    class ProjectWriter
    {
        internal Project Project;
        internal string[] TargetPathFolders;

        public ProjectWriter(string projectPath, string targetFolderPath)
        {
            var workspace = MSBuildWorkspace.Create();
            
            var project = workspace.OpenProjectAsync(projectPath).Result;
            
            Project = project;
            TargetPathFolders = targetFolderPath.Split('/');
        }
        internal ProjectWriter(Project project, string targetFolderPath)
        {
            Project = project;
            TargetPathFolders = targetFolderPath.Split('/');
        }

        public void AddFile(SourceCodeFile file)
        {
            Project = Project.AddDocument(file.Name, file.Content, TargetPathFolders).Project;
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
