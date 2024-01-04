using NetCoreQt.Generator.CodeGenerators;
using NetCoreQt.Generator.CodeSaver;
using NetCoreQt.Generator.Options;
using NetCoreQt.Generator.SchemaParsers;

namespace NetCoreQt.Generator.GenerateClasses {

    /// <summary>
    /// Generate classes operation.
    /// </summary>
    public class GenerateOperation {

        /// <summary>
        /// Perform generation classes.
        /// </summary>
        /// <param name="options">Options.</param>
        public static async Task Perform ( GenerateOptions options ) {
            if ( !File.Exists ( options.Schema ) ) {
                Console.WriteLine ( "Schema file not exists or path incorrect!" );
                Environment.Exit ( 1 );
            }

            var schemaContent = "";
            try {
                schemaContent = File.ReadAllText ( options.Schema );
            } catch ( Exception e ) {
                Console.WriteLine ( $"Can't read schema file: {e.Message}" );
                Environment.Exit ( 1 );
            }

            var parser = new SchemaParser ();
            GenerateSchema? schema = null;
            try {
                schema = parser.ParseSchema ( schemaContent );
            } catch ( Exception e ) {
                Console.WriteLine ( $"Parse schema error: {e.Message}" );
                Environment.Exit ( 1 );
            }

            var codeGenerator = new CodeGenerator ();
            var generateResult = codeGenerator.GenerateClasses ( new List<string> { "cs" }, schema );

            var codeSaverOptions = new CodeSaverOptions {
                Languages = new Dictionary<string, CodeSaverLanguage> {
                    ["cs"] = new CodeSaverLanguage {
                        SaveAsSingleFile = options.CsSingleFileName.Any (),
                        SingleFileName = options.CsSingleFileName,
                        TargetFolder = options.CsFolder
                    },
                    ["cpp"] = new CodeSaverLanguage {
                        SaveAsSingleFile = options.CppSingleFileName.Any (),
                        SingleFileName = options.CppSingleFileName,
                        TargetFolder = options.CsFolder
                    }
                }
            };
            ICodeSaver codeSaver = new CodeSaverToFile ();
            try {
                await codeSaver.SaveAsync ( codeSaverOptions, generateResult );
            } catch ( Exception e ) {
                Console.WriteLine ( $"Error while saving classes: {e.Message}" );
                Environment.Exit ( 1 );
            }

            Console.WriteLine ( "Generation sucessfully" );
        }

    }

}
