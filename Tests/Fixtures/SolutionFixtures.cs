using System;
namespace Tests
{
    public static class SolutionFixtures
    {

        public const string SOLUTION_FILE = @"
Microsoft Visual Studio Solution File, Format Version 12.00
# Visual Studio 2012
Project(""{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}"") = ""CSharpCodeGenerator"", ""CSharpCodeGenerator.csproj"", ""{4371EA70-9D22-481F-BA10-AF256394214D}""
EndProject
Project(""{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}"") = ""Tests"", ""Tests\Tests.csproj"", ""{5FAB2364-7086-4269-AD3E-5E8D2913A9B4}""
EndProject
";

        public const string PROJECT_FILE = @"<?xml version=""1.0"" encoding=""utf-8""?>
<Project DefaultTargets=""Build"" ToolsVersion=""4.0"" xmlns=""http://schemas.microsoft.com/developer/msbuild/2003"">
  <PropertyGroup>
    <Configuration Condition="" '$(Configuration)' == '' "">Debug</Configuration>
    <Platform Condition="" '$(Platform)' == '' "">AnyCPU</Platform>
    <ProjectGuid>{5FAB2364-7086-4269-AD3E-5E8D2913A9B4}</ProjectGuid>
    <OutputType>Library</OutputType>
    <RootNamespace>Tests</RootNamespace>
    <AssemblyName>Tests</AssemblyName>
    <TargetFrameworkVersion>v4.5</TargetFrameworkVersion>
  </PropertyGroup>
  <PropertyGroup Condition="" '$(Configuration)|$(Platform)' == 'Release|AnyCPU' "">
    <Optimize>true</Optimize>
    <OutputPath>bin\Release</OutputPath>
    <ErrorReport>prompt</ErrorReport>
    <WarningLevel>4</WarningLevel>
    <ConsolePause>false</ConsolePause>
  </PropertyGroup>
  <ItemGroup>
    <Reference Include=""System"" />
    <Reference Include=""NSpec"">
      <HintPath>Libraries\NSpec\NSpec.dll</HintPath>
    </Reference>
    <Reference Include=""nunit.framework"">
      <HintPath>Libraries\nunit.framework.dll</HintPath>
    </Reference>
  </ItemGroup>
  <ItemGroup>
    <Compile Include=""Example Tests\describe_RoslynComponentInfoProvider.cs"" />
    <Compile Include=""describe_SolutionParser.cs"" />
  </ItemGroup>
  <ItemGroup>
    <ProjectReference Include=""..\CSharpCodeGenerator.csproj"">
      <Project>{4371EA70-9D22-481F-BA10-AF256394214D}</Project>
      <Name>CSharpCodeGenerator</Name>
    </ProjectReference>
  </ItemGroup>
  <ItemGroup>
    <Folder Include=""Entitas Implementation\"" />
    <Folder Include=""Entitas Tests\"" />
  </ItemGroup>
  <Import Project=""$(MSBuildBinPath)\Microsoft.CSharp.targets"" />
</Project>
";
    }
}

