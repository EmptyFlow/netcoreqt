using CommandLine;
using NetCoreQt.Generator.GenerateClasses;
using NetCoreQt.Generator.Options;

var parser = Parser.Default.ParseArguments<GenerateOptions> ( args );

await parser.WithParsedAsync ( GenerateOperation.Perform );
await parser.WithNotParsedAsync ( HandleParseError );

static Task<int> HandleParseError ( IEnumerable<Error> errors ) {
    if ( errors.IsVersion () ) {
        var version = typeof ( GenerateOptions ).Assembly.GetName ().Version;
        var displayVersion = $"{version?.Major ?? 0}.{version?.Minor ?? 0}.{version?.Build ?? 0}";
        Console.WriteLine ( $"Current version is {displayVersion}\nDownload latest version from https://github.com/EmptyFlow/netcoreqt/releases" );
        return Task.FromResult ( 0 );
    }

    if ( errors.IsHelp () ) return Task.FromResult ( 0 );

    return Task.FromResult ( 1 );
}
