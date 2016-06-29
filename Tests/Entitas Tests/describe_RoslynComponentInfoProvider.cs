using System;
using System.Linq;
using CodeGenerator.Roslyn.Providers;
using CSharpCodeGenerator;
using CSharpCodeGenerator.Extensions;
using Entitas.CodeGenerator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;
using My.Namespace;
using NSpec;
using Tests.Fixtures;
using IComponent = Entitas.IComponent;

class describe_RoslynComponentInfoProvider : nspec
{
    // TODO Continue with workspaces: http://stackoverflow.com/questions/31861762/finding-all-references-to-a-method-with-roslyn
    MSBuildWorkspace _testsWorkspace;
    const string TESTS_PROJECT_PATH = @"..\..\CSharpCodeGenerator.Tests.csproj";
    private const string FIXTURES_PROJECT_PATH = @"..\..\..\Fixtures\CSharpCodeGenerator.TestFixtures.csproj";
    private const string FIXTURES_PROJECT_NAME = @"CSharpCodeGenerator.TestFixtures";
    Compilation _testsProjectCompilation;
    Compilation _fixturesProjectCompilation;

    RoslynComponentInfoProvider createProviderWithTypes(params Type[] types)
    {
        var workspace = MSBuildWorkspace.Create();

        var fixturesProject = workspace.OpenProjectAsync(FIXTURES_PROJECT_PATH).Result;

        var projectMock = CreateProjectOnSolution(fixturesProject.Solution, "MockedProject");

        var mscorlib = MetadataReference.CreateFromFile(typeof(object).Assembly.Location);
        projectMock = projectMock
            .AddMetadataReference(mscorlib)
            .AddProjectReference(new ProjectReference(fixturesProject.Id));

        foreach (var type in types)
        {
            var declarationNode = CompilationHelper.GetTypeDeclarationAsSyntaxNode(type.FullName, _testsProjectCompilation, _fixturesProjectCompilation);
            //            Console.WriteLine($"Adding Document {type.Name}: \n'{declarationNode.SyntaxTree}'");
            var document = projectMock.AddDocument($"{type.Name}.cs", declarationNode.SyntaxTree.GetRoot());
            projectMock = document.Project;
        }

        return new RoslynComponentInfoProvider(new ProjectStructure(projectMock), new string[0], new string[0]);
    }

    private static Project CreateProjectOnSolution(Solution solution, string testprojectName)
    {
        return solution.AddProject(ProjectId.CreateNewId(), testprojectName, "MockedProjectAssembly",
                    LanguageNames.CSharp).Projects.First(p => p.Name == testprojectName);
    }

    RoslynComponentInfoProvider createProviderWithPoolName(params string[] poolNames)
    {
        return new RoslynComponentInfoProvider(null, poolNames, new string[0]);
    }

    RoslynComponentInfoProvider createProviderWithBlueprintNames(params string[] blueprintNames)
    {
        return new RoslynComponentInfoProvider(null, new string[0], blueprintNames);
    }

    private void InitializingTestsProjectCompilation()
    {
        _testsWorkspace = MSBuildWorkspace.Create();
        var testsProject = _testsWorkspace.OpenProjectAsync(TESTS_PROJECT_PATH).Result;
        _testsProjectCompilation = testsProject.GetCompilationAsync().Result;
        _fixturesProjectCompilation = _testsWorkspace.CurrentSolution.Projects.Single(p => p.Name == FIXTURES_PROJECT_NAME).GetCompilationAsync().Result;
    }
    void when_providing()
    {
        InitializingTestsProjectCompilation();


        context["pool names"] = () =>
        {
            it["has no pool names if empty"] = () =>
            {
                var provider = createProviderWithPoolName();
                provider.poolNames.should_be_empty();
            };

            it["has pool names if set"] = () =>
            {
                var provider = createProviderWithPoolName("Pool1", "Pool2");
                provider.poolNames.Length.should_be(2);
                provider.poolNames[0].should_be("Pool1");
                provider.poolNames[1].should_be("Pool2");
            };
        };

        context["component infos"] = () =>
       {

           it["finds no components if there are no types which implement IComponent"] = () =>
           {
               var provider = createProviderWithTypes();
               provider.componentInfos.should_be_empty();
           };

           it["finds no components and ignores IComponent itself"] = () =>
           {
               var provider = createProviderWithTypes(typeof(IComponent));
               provider.componentInfos.should_be_empty();
           };

           it["finds no components and ignores interfaces"] = () =>
           {
               var provider = createProviderWithTypes(typeof(AnotherComponentInterface));
               provider.componentInfos.should_be_empty();
           };

           it["finds no components and ignores abstract classes"] = () =>
           {
               var provider = createProviderWithTypes(typeof(SomeClassWithWrongIComponent));
               provider.componentInfos.should_be_empty();
           };

           it["finds no components and ignores classes without IComponent implementation"] = () =>
           {
               var provider = createProviderWithTypes(typeof(AbstractComponent));
               provider.componentInfos.should_be_empty();
           };

           it["creates component info from found component"] = () =>
           {
               var provider = createProviderWithTypes(typeof(ComponentA));

               provider.componentInfos.Length.should_be(1);
               var info = provider.componentInfos[0];

               info.fullTypeName.should_be("ComponentA");
               info.typeName.should_be("ComponentA");
               info.memberInfos.should_be_empty();
               info.pools.should_be_empty();
               info.isSingleEntity.should_be_false();
               info.singleComponentPrefix.should_be("is");
               info.generateMethods.should_be(true);
               info.generateIndex.should_be(true);
               info.isSingletonComponent.should_be(true);
           };

           it["creates component info from found component with namespace"] = () =>
           {
               var provider = createProviderWithTypes(typeof(NamespaceComponent));
               provider.componentInfos.Length.should_be(1);
               var info = provider.componentInfos[0];

               info.fullTypeName.should_be("My.Namespace.NamespaceComponent");
               info.typeName.should_be("NamespaceComponent");
               info.memberInfos.should_be_empty();
               info.pools.should_be_empty();
               info.isSingleEntity.should_be_false();
               info.singleComponentPrefix.should_be("is");
               info.generateMethods.should_be(true);
               info.generateIndex.should_be(true);
               info.isSingletonComponent.should_be(true);
           };

           it["detects PoolAttribure"] = () =>
           {
               var provider = createProviderWithTypes(typeof(CComponent));
               provider.componentInfos.Length.should_be(1);
               var info = provider.componentInfos[0];

               info.fullTypeName.should_be("CComponent");
               info.typeName.should_be("CComponent");
               info.memberInfos.should_be_empty();
               info.pools.Length.should_be(3);

               info.pools.should_contain("PoolA");
               info.pools.should_contain("PoolB");
               info.pools.should_contain("PoolC");

               info.isSingleEntity.should_be_false();
               info.singleComponentPrefix.should_be("is");
               info.generateMethods.should_be(true);
               info.generateIndex.should_be(true);
               info.isSingletonComponent.should_be(true);
           };

           //    it ["detects SingleEntityAttribute "] = () => {
           //        var provider = createProviderWithTypes (typeof (AnimatingComponent));
           //        provider.componentInfos.Length.should_be (1);
           //        var info = provider.componentInfos [0];

           //        info.fullTypeName.should_be ("AnimatingComponent");
           //        info.typeName.should_be ("AnimatingComponent");
           //        info.memberInfos.should_be_empty ();
           //        info.pools.Length.should_be (0);
           //        info.isSingleEntity.should_be_true ();
           //        info.singleComponentPrefix.should_be ("is");
           //        info.generateMethods.should_be (true);
           //        info.generateIndex.should_be (true);
           //        info.isSingletonComponent.should_be (true);
           //    };

           //    it ["detects DontGenerateAttribute "] = () => {
           //        var provider = createProviderWithTypes (typeof (DontGenerateComponent));
           //        provider.componentInfos.Length.should_be (1);
           //        var info = provider.componentInfos [0];

           //        info.fullTypeName.should_be ("DontGenerateComponent");
           //        info.typeName.should_be ("DontGenerateComponent");
           //        info.memberInfos.should_be_empty ();
           //        info.pools.Length.should_be (0);
           //        info.isSingleEntity.should_be_false ();
           //        info.singleComponentPrefix.should_be ("is");
           //        info.generateMethods.should_be (false);
           //        info.generateIndex.should_be (true);
           //        info.isSingletonComponent.should_be (true);
           //    };

           //    it ["detects DontGenerateAttribute and don't generate index"] = () => {
           //        var provider = createProviderWithTypes (typeof (DontGenerateIndexComponent));
           //        provider.componentInfos.Length.should_be (1);
           //        var info = provider.componentInfos [0];

           //        info.fullTypeName.should_be ("DontGenerateIndexComponent");
           //        info.typeName.should_be ("DontGenerateIndexComponent");
           //        info.memberInfos.should_be_empty ();
           //        info.pools.Length.should_be (0);
           //        info.isSingleEntity.should_be_false ();
           //        info.singleComponentPrefix.should_be ("is");
           //        info.generateMethods.should_be (false);
           //        info.generateIndex.should_be (false);
           //        info.isSingletonComponent.should_be (true);
           //    };

           //    it ["detects CustomPrefixAttribute"] = () => {
           //        var provider = createProviderWithTypes (typeof (CustomPrefixComponent));
           //        provider.componentInfos.Length.should_be (1);
           //        var info = provider.componentInfos [0];

           //        info.fullTypeName.should_be ("CustomPrefixComponent");
           //        info.typeName.should_be ("CustomPrefixComponent");
           //        info.memberInfos.should_be_empty ();
           //        info.pools.Length.should_be (0);
           //        info.isSingleEntity.should_be_true ();
           //        info.singleComponentPrefix.should_be ("My");
           //        info.generateMethods.should_be (true);
           //        info.generateIndex.should_be (true);
           //        info.isSingletonComponent.should_be (true);
           //    };

           //            it ["sets fields"] = () => {
           //                var type = typeof (ComponentWithFieldsAndProperties);
           //                var provider = createProviderWithTypes (type);
           //                provider.componentInfos.Length.should_be (1);
           //                var info = provider.componentInfos [0];
           //
           //                info.fullTypeName.should_be (type.FullName);
           //                info.typeName.should_be (type.FullName);
           //                info.memberInfos.Count.should_be (2);
           //
           //                info.memberInfos [0].type.should_be (typeof (string));
           //                info.memberInfos [0].name.should_be ("publicField");
           //
           //                info.memberInfos [1].type.should_be (typeof (string));
           //                info.memberInfos [1].name.should_be ("publicProperty");
           //
           //                info.pools.Length.should_be (0);
           //                info.isSingleEntity.should_be_false ();
           //                info.singleComponentPrefix.should_be ("is");
           //                info.generateMethods.should_be (true);
           //                info.generateIndex.should_be (true);
           //                info.isSingletonComponent.should_be (false);
           //            };

           it["gets multiple component infos"] = () =>
          {
              var provider = createProviderWithTypes(typeof(ComponentA), typeof(ComponentB));
              provider.componentInfos.Length.should_be(2);
              provider.componentInfos[0].fullTypeName.should_be("ComponentA");
              provider.componentInfos[1].fullTypeName.should_be("ComponentB");
          };
       };

        context["blueprint names"] = () =>
        {
            it["has no blueprint names if empty"] = () =>
            {
                var provider = createProviderWithBlueprintNames();
                provider.blueprintNames.should_be_empty();
            };

            it["has blueprint names if set"] = () =>
            {
                var provider = createProviderWithBlueprintNames("My Blueprint1", "My Blueprint2");
                provider.blueprintNames.Length.should_be(2);
                provider.blueprintNames[0].should_be("My Blueprint1");
                provider.blueprintNames[1].should_be("My Blueprint2");
            };
        };

        context["roslyn provider"] = () =>
        {

            it["sets fields"] = () =>
            {
                var type = typeof(ComponentWithFields);
                var provider = createProviderWithTypes(type);
                provider.componentInfos.Length.should_be(1);
                var info = provider.componentInfos[0];

                info.fullTypeName.should_be(type.FullName);
                info.typeName.should_be(type.FullName);
                info.memberInfos.Count.should_be(1);

                info.memberInfos[0].type.should_be(typeof(string));
                info.memberInfos[0].name.should_be("publicField");

                info.pools.Length.should_be(0);
                info.isSingleEntity.should_be_false();
                info.singleComponentPrefix.should_be("is");
                info.generateMethods.should_be(true);
                info.generateIndex.should_be(true);
                info.isSingletonComponent.should_be(false);
            };
        };


    }

}

