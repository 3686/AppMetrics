#addin Cake.Coveralls

#tool "nuget:?package=xunit.runner.console"
#tool "nuget:https://www.nuget.org/api/v2?package=OpenCover&version=4.6.519"
#tool "nuget:https://www.nuget.org/api/v2?package=ReportGenerator&version=2.4.5"
#tool "nuget:?package=GitVersion.CommandLine"
#tool coveralls.io

//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////
var target          = Argument("target", "Default");
var configuration   = HasArgument("Configuration") ? Argument<string>("Configuration") :
                      EnvironmentVariable("Configuration") != null ? EnvironmentVariable("Configuration") : "Release";
var skipOpenCover   = Argument("skipOpenCover", false);

//////////////////////////////////////////////////////////////////////
// DEFINE DIRECTORIES
//////////////////////////////////////////////////////////////////////
var packDirs                    = new [] { Directory("./src/App.Metrics"), Directory("./src/App.Metrics.Concurrency"), Directory("./src/App.Metrics.Extensions.Middleware"), Directory("./src/App.Metrics.Extensions.Mvc"), Directory("./src/App.Metrics.Formatters.Json") };
var artifactsDir                = (DirectoryPath) Directory("./artifacts");
var testResultsDir              = (DirectoryPath) artifactsDir.Combine("test-results");
var testCoverageOutputFilePath  = testResultsDir.CombineWithFilePath("OpenCover.xml");
var packagesDir                 = artifactsDir.Combine("packages");

//////////////////////////////////////////////////////////////////////
// DEFINE PARAMS
//////////////////////////////////////////////////////////////////////
var isAppVeyorBuild             = AppVeyor.IsRunningOnAppVeyor;
var coverallsToken              = Context.EnvironmentVariable("COVERALLS_REPO_TOKEN");
var preReleaseSuffix =
    HasArgument("PreReleaseSuffix") ? Argument<string>("PreReleaseSuffix") :
	(AppVeyor.IsRunningOnAppVeyor && AppVeyor.Environment.Repository.Tag.IsTag) ? null :
    EnvironmentVariable("PreReleaseSuffix") != null ? EnvironmentVariable("PreReleaseSuffix") :	"ci";
var buildNumber =
    HasArgument("BuildNumber") ? Argument<int>("BuildNumber") :
    AppVeyor.IsRunningOnAppVeyor ? AppVeyor.Environment.Build.Number :
    TravisCI.IsRunningOnTravisCI ? TravisCI.Environment.Build.BuildNumber :
    EnvironmentVariable("BuildNumber") != null ? int.Parse(EnvironmentVariable("BuildNumber")) : 0;

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean")
    .Does(() =>
{
    CleanDirectory(artifactsDir);
});

Task("Restore")
    .IsDependentOn("Clean")    
    .Does(() =>
{
    DotNetCoreRestore("./", new DotNetCoreRestoreSettings
    {
        Verbose = true,
        Verbosity = DotNetCoreRestoreVerbosity.Verbose,
        Sources = new [] {            
            "https://api.nuget.org/v3/index.json",
        }
    });
});

Task("Build")    
    .IsDependentOn("Restore")
    .Does(() =>
{
    var projects = GetFiles("./**/project.json");
    
    foreach(var project in projects)
    {
        Context.Information("Project: " + project.GetDirectory().FullPath);

        DotNetCoreBuild(project.GetDirectory().FullPath, new DotNetCoreBuildSettings {
            Configuration = configuration
        });
    }    
});

// Task("Version")
//     .WithCriteria(() => !BuildSystem.IsLocalBuild)
//     .Does(() => {        
//         foreach(var packDir in packDirs)
//         {
//             var projectDotJson = packDir.ToString() + "/project.json";
//             var updatedProjectJson = System.IO.File.ReadAllText(projectDotJson, Encoding.UTF8)
//                 .Replace("1.0.0-*", gitVersionInfo.NuGetVersion);

//             System.IO.File.WriteAllText(projectDotJson, updatedProjectJson, Encoding.UTF8);
//         }                 
//     });

Task("Pack")
    .IsDependentOn("Restore")    
    .IsDependentOn("Clean")
    .Does(() =>
{
    string versionSuffix = null;
    if (!string.IsNullOrEmpty(preReleaseSuffix))
    {
        versionSuffix = preReleaseSuffix + "-" + buildNumber.ToString("D4");
    }
    var settings = new DotNetCorePackSettings
    {
        Configuration = configuration,
        OutputDirectory = packagesDir,
        VersionSuffix = versionSuffix
    };
    
    foreach(var packDir in packDirs)
    {
        DotNetCorePack(packDir, settings);
    }    
});

Task("RunTests")
    .IsDependentOn("Build")
    .Does(() =>
{
    var projects = GetFiles("./test/**/project.json");

    CreateDirectory(testResultsDir);

    Context.Information("Found " + projects.Count() + " projects");

    foreach (var project in projects)
    {
        if (IsRunningOnWindows())
        {
            var apiUrl = EnvironmentVariable("APPVEYOR_API_URL");

            try
            {
                if (!string.IsNullOrEmpty(apiUrl))
                {
                    // Disable XUnit AppVeyorReporter see https://github.com/cake-build/cake/issues/1200
                    System.Environment.SetEnvironmentVariable("APPVEYOR_API_URL", null);
                }

                Action<ICakeContext> testAction = tool => {

                    tool.DotNetCoreTest(project.GetDirectory().FullPath, new DotNetCoreTestSettings {
                        Configuration = configuration,
                        NoBuild = true,
                        Verbose = false,
                        ArgumentCustomization = args =>
                            args.Append("-xml").Append(testResultsDir.CombineWithFilePath(project.GetFilenameWithoutExtension()).FullPath + ".xml")
                    });
                };

                if (!skipOpenCover) {
                    OpenCover(testAction,
                        testCoverageOutputFilePath,
                        new OpenCoverSettings {                            
                            ArgumentCustomization = args => args.Append("-mergeoutput -hideskipped:All -safemode:off -oldStyle")
                        }
                        .WithFilter("+[App.Metrics*]* -[xunit.*]* -[*.Facts]*")
                        .ExcludeByAttribute("*.AppMetricsExcludeFromCodeCoverage*")
                        .ExcludeByFile("*/*Designer.cs;*/*.g.cs;*/*.g.i.cs"));
                } 
                else 
                {
                    testAction(Context);
                }
            }
            finally
            {
                if (!string.IsNullOrEmpty(apiUrl))
                {
                    System.Environment.SetEnvironmentVariable("APPVEYOR_API_URL", apiUrl);
                }
            }
        }
        else
        {
            var settings = new DotNetCoreTestSettings
            {
                Configuration = configuration, 
                Framework = "netcoreapp1.1"
            };

            DotNetCoreTest(project.GetDirectory().FullPath, settings);
        }
    }    
});

Task("HtmlCoverageReport")    
    .WithCriteria(() => FileExists(testCoverageOutputFilePath))
    .WithCriteria(() => BuildSystem.IsLocalBuild)    
    .IsDependentOn("RunTests")
    .Does(() => 
{
    ReportGenerator(testCoverageOutputFilePath, testResultsDir);
});

Task("PublishCoverage")
    // .WithCriteria(() => !BuildSystem.AppVeyor.Environment.PullRequest.IsPullRequest)
    .WithCriteria(() => FileExists(testCoverageOutputFilePath))
    .WithCriteria(() => !BuildSystem.IsLocalBuild)
    .WithCriteria(() => !string.IsNullOrEmpty(coverallsToken))
    .IsDependentOn("RunTests")
    .Does(() => 
{
    CoverallsIo(testCoverageOutputFilePath, new CoverallsIoSettings()
    {
        RepoToken = coverallsToken
    });
});

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("Build")
    .IsDependentOn("RunTests")
    .IsDependentOn("Pack")
    .IsDependentOn("HtmlCoverageReport");

Task("AppVeyor")
    .IsDependentOn("Build")
    .IsDependentOn("RunTests")
    .IsDependentOn("Pack")
    .IsDependentOn("PublishCoverage");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);