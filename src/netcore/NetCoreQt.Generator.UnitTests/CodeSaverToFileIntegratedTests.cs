using NetCoreQt.Generator.CodeGenerators;
using NetCoreQt.Generator.CodeSaver;

namespace NetCoreQt.Generator.UnitTests {

    public class CodeSaverToFileIntegratedTests {

        [Fact]
        public async Task SaveAsync_Completed_CSharp_Events_SingleFile () {
            //arrange
            var saver = new CodeSaverToFile ();
            var options = new CodeSaverOptions {
                Languages = new Dictionary<string, CodeSaverLanguage> {
                    ["cs"] = new CodeSaverLanguage {
                        SaveAsSingleFile = true,
                        SingleFileName = "CsFileTestSingleEvent.cs",
                        TargetFolder = "SingleGeneratedClasses"
                    }
                }
            };
            var generatedResult = new GeneratedResult {
                CsLanguage = new GeneratedResultLanguage {
                    Events = new Dictionary<string, string> {
                        ["TestEvent"] = "public record TestEvent (int Price, long Count, double Quantity, string Title);",
                        ["TestEvent2"] = "public record TestEvent2 (int Price, long Count, double Quantity, string Title);"
                    }
                }
            };

            //act
            await saver.SaveAsync ( options, generatedResult );

            //assert
            var fileExists = File.Exists ( "SingleGeneratedClasses/CsFileTestSingleEvent.cs" );
            Assert.True ( fileExists );

            var lines = ( await File.ReadAllTextAsync ( "SingleGeneratedClasses/CsFileTestSingleEvent.cs" ) ).Split ( "\n" );

            Assert.Equal ( "namespace NetCoreQt.Generator.CodeSaver {", lines[0] );
            Assert.Equal ( "\r", lines[1] );
            Assert.Equal ( "public record TestEvent (int Price, long Count, double Quantity, string Title);", lines[2] );
            Assert.Equal ( "public record TestEvent2 (int Price, long Count, double Quantity, string Title);", lines[3] );
            Assert.Equal ( "", lines[4] );
            Assert.Equal ( "}\r", lines[5] );
            Assert.Equal ( "", lines[6] );
        }

    }

}
