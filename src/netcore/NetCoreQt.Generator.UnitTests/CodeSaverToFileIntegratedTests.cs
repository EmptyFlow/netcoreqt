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

        [Fact]
        public async Task SaveAsync_Completed_CSharp_Events_MultipleFile () {
            //arrange
            var saver = new CodeSaverToFile ();
            var options = new CodeSaverOptions {
                Languages = new Dictionary<string, CodeSaverLanguage> {
                    ["cs"] = new CodeSaverLanguage {
                        SaveAsSingleFile = false,
                        TargetFolder = "MultipleGeneratedClasses"
                    }
                }
            };
            var generatedResult = new GeneratedResult {
                CsLanguage = new GeneratedResultLanguage {
                    Events = new Dictionary<string, string> {
                        ["MultiEventTest1"] = "public record TestEvent (int Price, long Count, double Quantity, string Title);",
                        ["MultiEventTest2"] = "public record TestEvent2 (int Price, long Count, double Quantity, string Title);"
                    }
                }
            };

            //act
            await saver.SaveAsync ( options, generatedResult );

            //assert
            var fileExists1 = File.Exists ( "MultipleGeneratedClasses/Event.MultiEventTest1.cs" );
            Assert.True ( fileExists1 );
            var fileExists2 = File.Exists ( "MultipleGeneratedClasses/Event.MultiEventTest2.cs" );
            Assert.True ( fileExists2 );

            var linesFile1 = ( await File.ReadAllTextAsync ( "MultipleGeneratedClasses/Event.MultiEventTest1.cs" ) ).Split ( "\n" );
            var linesFile2 = ( await File.ReadAllTextAsync ( "MultipleGeneratedClasses/Event.MultiEventTest2.cs" ) ).Split ( "\n" );

            Assert.Equal ( "namespace NetCoreQt.Generator.CodeSaver {", linesFile1[0] );
            Assert.Equal ( "\r", linesFile1[1] );
            Assert.Equal ( "public record TestEvent (int Price, long Count, double Quantity, string Title);", linesFile1[2] );
            Assert.Equal ( "", linesFile1[3] );
            Assert.Equal ( "}\r", linesFile1[4] );

            Assert.Equal ( "namespace NetCoreQt.Generator.CodeSaver {", linesFile2[0] );
            Assert.Equal ( "\r", linesFile2[1] );
            Assert.Equal ( "public record TestEvent2 (int Price, long Count, double Quantity, string Title);", linesFile2[2] );
            Assert.Equal ( "", linesFile2[3] );
            Assert.Equal ( "}\r", linesFile2[4] );
        }

    }

}
