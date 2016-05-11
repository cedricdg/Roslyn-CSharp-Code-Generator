using System;
namespace CSharpCodeGenerator
{
    public struct Project
    {
        public string ProjectName;
        public string RelativePath;
        public string ProjectGuid;
        public string ProjectType;


        public override bool Equals (object obj)
        {
            if (!(obj is Project))
                return false;

            Project otherProject = (Project)obj;
            
            if (ProjectName != otherProject.ProjectName ||
                ProjectType != otherProject.ProjectType ||
                RelativePath != otherProject.RelativePath)
                return false;

            return true;

        }
    }

}

