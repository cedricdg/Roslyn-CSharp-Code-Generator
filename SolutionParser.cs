using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace CSharpCodeGenerator
{

    public static class SolutionParser
    {

        public enum BuildActionProperty
        {
            None,
            Compile,
            Content,
            EmbeddedResource
        }

        public struct ProjectFile
        {
            public BuildActionProperty BuildAction;
            public string FilePath;
            public string FileType;
        }

        public static Project [] ParseProjectsFromSolution (string solutionContent)
        {

            Regex projReg = new Regex (
                @"Project\(\""\{([-\w]*)\}\""\) = \""([\w _]+.+)\"", \""(.*\.(cs|vcx|vb)proj)\""",
                RegexOptions.Compiled);

            return ApplyRegex (projReg, solutionContent, match => new Project {
                ProjectGuid = match.Groups [1].Value,
                ProjectName = match.Groups [2].Value,
                RelativePath = match.Groups [3].Value,
                ProjectType = match.Groups [4].Value
            });
        }

        public static ProjectFile [] ParseSourceFilesFromProject (string projectFile)
        {
            Regex projReg = new Regex (
                @"<(\w*) Include=\""(.*[\w\s]*.(cs|vcx|vb))\"" \/>",
                RegexOptions.Compiled);

            return ApplyRegex (projReg, projectFile, match => {
                var file = new ProjectFile {
                    BuildAction = ConvertToBuildAction (match.Groups [1].Value),
                    FilePath = match.Groups [2].Value,
                    FileType = match.Groups [3].Value,
                };
                return file;
            });
        }

        static BuildActionProperty ConvertToBuildAction (string value)
        {
            switch (value) {
            case "Content":
                return BuildActionProperty.Content;
            case "Compile":
                return BuildActionProperty.Compile;
            case "Embedded Resource":
                return BuildActionProperty.EmbeddedResource;
            case "None":
            default:
                return BuildActionProperty.None;
            }

        }

        static T [] ApplyRegex<T> (Regex regex, string input, Func<Match, T> wrapMatchToObject)
        {
            var matches = regex.Matches (input).Cast<Match> ();
            var results = new List<T> (matches.Count ());
            foreach (var match in matches) {
                var result = wrapMatchToObject (match);
                if (result != null) {
                    results.Add (result);
                }
            }
            return results.ToArray ();
        }
    }
}

