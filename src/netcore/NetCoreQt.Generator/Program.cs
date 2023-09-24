using CommandLine;
using NetCoreQt.Generator.GenerateClasses;
using NetCoreQt.Generator.Options;

var parser = Parser.Default.ParseArguments<GenerateOptions> ( args );

await parser.WithParsedAsync<GenerateOptions> ( GenerateOperation.Perform );
await parser.WithNotParsedAsync ( HandleParseError );

static Task<int> HandleParseError ( IEnumerable<Error> errors ) {
    if ( errors.IsVersion () ) {
        Console.WriteLine ( $"Current version is {typeof ( GenerateOptions ).Assembly.GetName ().Version}\nDownload latest version from https://github.com/EmptyFlow/netcoreqt/releases" );
        return Task.FromResult ( 0 );
    }

    if ( errors.IsHelp () ) return Task.FromResult ( 0 );

    return Task.FromResult ( 1 );
}
