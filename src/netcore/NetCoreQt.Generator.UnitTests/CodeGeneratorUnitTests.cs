using NetCoreQt.Generator.CodeGenerators;
using NetCoreQt.Generator.SchemaParsers;

namespace NetCoreQt.Generator.UnitTests {

    public class CodeGeneratorUnitTests {

        [Fact]
        public void GenerateOnlyClasses_Completed_Csharp_Event () {
            //arrange
            var generator = new CodeGenerator ();
            var schema = new GenerateSchema {
                Events = [
                    new GenerateEvent {
                        HostLanguages = ["cs"],
                        Name = "TestEvent",
                        Properties = [
                            new GenerateEventProperty {
                                Name = "Price",
                                Type = PropertyType.Int32,
                            },
                            new GenerateEventProperty {
                                Name = "Count",
                                Type = PropertyType.Int64,
                            },
                            new GenerateEventProperty {
                                Name = "Quantity",
                                Type = PropertyType.Double,
                            },
                            new GenerateEventProperty {
                                Name = "Title",
                                Type = PropertyType.String,
                            }
                        ]
                    }
                ]
            };

            //act
            var result = generator.GenerateClasses ( ["cs"], schema );

            //assert
            Assert.Empty ( result.CppLanguage.Events );
            Assert.NotEmpty ( result.CsLanguage.Events );
            Assert.Contains ( result.CsLanguage.Events, a => a.Key == "TestEvent" );
            var eventContent = result.CsLanguage.Events["TestEvent"];
            Assert.Contains ( "public record TestEvent (int Price, long Count, double Quantity, string Title);", eventContent );
            Assert.Contains ( "public static class TestEventExternal {", eventContent );
            Assert.Contains ( "public static void GetPrice ( IntPtr index ) {", eventContent );
            Assert.Contains ( "public static void GetCount ( IntPtr index ) {", eventContent );
            Assert.Contains ( "public static void GetQuantity ( IntPtr index ) {", eventContent );
            Assert.Contains ( "public static void GetTitle ( IntPtr index ) {", eventContent );
        }

    }

}
