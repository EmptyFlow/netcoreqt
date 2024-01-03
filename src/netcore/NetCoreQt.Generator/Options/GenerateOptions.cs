using CommandLine;

namespace NetCoreQt.Generator.Options {

    /// <summary>
    /// Generate classes from scheme.
    /// </summary>
    [Verb ( "generate", HelpText = "Generate classes from schema file." )]
    public class GenerateOptions {

        [Option ( 's', "schema", Required = true, HelpText = "Specify schema file from which the classes will be generated." )]
        public string Schema { get; set; } = "";

        [Option ( "cppfolder", Required = true, HelpText = "Specify folder where C++ classes will be generated." )]
        public string CppFolder { get; set; } = "";

        [Option ( "csfolder", Required = true, HelpText = "Specify folder where C# classes will be generated." )]
        public string CsFolder { get; set; } = "";

        [Option ( "cppsinglefile", HelpText = "If you need generate as single C++ header file, you must specify single file name." )]
        public string CppSingleFileName { get; set; } = "";

        [Option ( "cssinglefile", HelpText = "If you need generate as single C# file, you must specify single file name." )]
        public string CsSingleFileName { get; set; } = "";

    }

}
