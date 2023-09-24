using CommandLine;

namespace NetCoreQt.Generator.Options {

    /// <summary>
    /// Generate classes from scheme.
    /// </summary>
    [Verb ( "generate", HelpText = "Generate classes from schema file." )]
    public class GenerateOptions {

        [Option ( 's', "schema", Required = true, HelpText = "Specify schema file from which the classes will be generated." )]
        public string Schema { get; set; } = "";

        [Option ( 'f', "folder", Required = true, HelpText = "Specify folder where the classes will be generated." )]
        public string Folder { get; set; } = "";

        [Option ( 'l', "language", Required = true, HelpText = "Specify the language in which the classes will be generated. Available values: csharp, cpluplus" )]
        public string Language { get; set; } = "";

    }

}
