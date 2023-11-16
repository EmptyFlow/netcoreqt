using CommandLine;

namespace NetCoreQt.Generator.Options {

    /// <summary>
    /// Generate classes from scheme.
    /// </summary>
    [Verb ( "generate", HelpText = "Generate classes from schema file." )]
    public class GenerateOptions {

        [Option ( 's', "schema", Required = true, HelpText = "Specify schema file from which the classes will be generated." )]
        public string Schema { get; set; } = "";

        [Option ( 'f', "cppfolder", HelpText = "Specify folder where C++ classes will be generated." )]
        public string TargetFolder { get; set; } = "";

        [Option ( 'f', "csfolder", HelpText = "Specify folder where C# classes will be generated." )]
        public string SourceFolder { get; set; } = "";

    }

}
