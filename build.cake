
#tool "nuget:?package=NUnit.ConsoleRunner"

///////////////////////////////////////////////////////////////////////////////
// ARGUMENTS
///////////////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");



///////////////////////////////////////////////////////////////////////////////
// TASKS
///////////////////////////////////////////////////////////////////////////////

Task ("Clean")
.Does(()=>
{
   CleanDirectories("./artifacts");
   DotNetCoreClean(".");
}
   
   
);
Task("Restore")
.Does(()=>
{
   DotNetCoreRestore();
});

Task ("Build")
   .IsDependentOn("Clean")
   .IsDependentOn("Restore")
   .Does(()=>{

      DotNetCoreBuild(".");
   });

Task("Run-Unit-Test")
   .Does(() =>
{
   var settings = new DotNetCoreTestSettings
    {
        Configuration = configuration,
        
    };
    var projects = GetFiles("./test/**/*.csproj");
    foreach(var project in projects)
    {
        DotNetCoreTest(project.FullPath, settings);
    }
});
Task("Pack")
   .Does(() =>
{
   var settings = new DotNetCorePackSettings
    {
        Configuration = configuration,
        OutputDirectory = "./artifacts"
    };
   DotNetCorePack("./src/VerbalExpressions/VerbalExpressions.csproj",settings);
});

Task("Default")
    .IsDependentOn("Build")

.Does(() => {
   
});

RunTarget(target);