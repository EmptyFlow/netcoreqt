using NetCoreQt.Generator.CodeGenerators;

namespace NetCoreQt.Generator.CodeSaver {

    /// <summary>
    /// Class implement way save <see cref="GeneratedResult"/> to file.
    /// </summary>
    public class CodeSaverToFile : ICodeSaver {

        public async Task SaveAsync ( CodeSaverOptions options, GeneratedResult generatedResult ) {
            foreach ( var item in options.Languages ) {
                var languageOptions = item.Value;
                switch ( item.Key ) {
                    case CodeGenerator.CppLanguage:

                        break;
                    case CodeGenerator.CsLanguage:
                        if ( languageOptions.SaveAsSingleFile ) {
                            await GenerateSingleCsharpFile ( languageOptions.TargetFolder, languageOptions.SingleFileName, generatedResult.CsLanguage );
                        } else {
                            await GenerateCsharpFiles ( languageOptions.TargetFolder, generatedResult.CsLanguage );
                        }
                        break;
                    default: throw new NotSupportedException ( $"Saver for language {item.Key} not supported!" );
                }
            }

        }

        private static async Task GenerateSingleCsharpFile ( string targetFolder, string fileName, GeneratedResultLanguage generatedResultLanguage ) {
            if ( !Directory.Exists ( targetFolder ) ) Directory.CreateDirectory ( targetFolder );
            var resultFile = Path.Combine ( targetFolder, fileName );
            if ( File.Exists ( resultFile ) ) File.Delete ( resultFile );

            using var stream = File.OpenWrite ( resultFile );
            using var writer = new StreamWriter ( stream );

            await writer.WriteLineAsync ( "namespace NetCoreQt.Generator.CodeSaver {\n" );

            foreach ( var item in generatedResultLanguage.Events.Values ) {
                await writer.WriteAsync ( item + "\n" );
            }

            await writer.WriteLineAsync ( "\n}" );
        }

        private static async Task GenerateCsharpFiles ( string targetFolder, GeneratedResultLanguage generatedResultLanguage ) {
            if ( !Directory.Exists ( targetFolder ) ) Directory.CreateDirectory ( targetFolder );

            foreach ( var item in generatedResultLanguage.Events ) {
                var resultFile = Path.Combine ( targetFolder, $"Event.{item.Key}.cs" );
                if ( File.Exists ( resultFile ) ) File.Delete ( resultFile );

                using var stream = File.OpenWrite ( resultFile );
                using var writer = new StreamWriter ( stream );

                await writer.WriteLineAsync ( "namespace NetCoreQt.Generator.CodeSaver {\n" );
                await writer.WriteAsync ( item.Value + "\n" );
                await writer.WriteLineAsync ( "\n}" );
            }
        }

    }

}
