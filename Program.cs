using System;

namespace CSharpCodeGenerator
{
	class MainClass
	{
		public static void Main(string[] args)
		{
			const string pathToSolution = @"/Users/ced/Documents/roslyn-code-generator/SimpleRoslynAnalysis/SimpleRoslynAnalysis.sln";
			var ws = Microsoft.CodeAnalysis.MSBuild.MSBuildWorkspace.Create();
			var solution = ws.OpenSolutionAsync(pathToSolution).Result;
			Console.WriteLine("Hello World!");
			Console.ReadLine();
		}
	}
}
