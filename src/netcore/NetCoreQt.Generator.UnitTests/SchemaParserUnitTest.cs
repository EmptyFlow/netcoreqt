using NetCoreQt.Generator.SchemaParsers;

namespace NetCoreQt.Generator.UnitTests {

    public class SchemaParserUnitTest {

        [Fact, Trait ( "Category", "UnitTest" )]
        public void ParseSchema_Success_Event_Empty () {
            //arrange
            var parser = new SchemaParser ();
            var schema =
"""
version 1.0
event TestEvent
""";

            //act
            var result = parser.ParseSchema ( schema );

            //assert
            Assert.NotNull ( result );
            Assert.NotEmpty ( result.Events );
            var eventSchema = result.Events.First ();
            Assert.Equal ( "TestEvent", eventSchema.Name );
            Assert.Empty ( eventSchema.HostLanguages );
            Assert.Empty ( eventSchema.Properties );
        }

        [Fact, Trait ( "Category", "UnitTest" )]
        public void ParseSchema_Success_Event_WithHostLanguages () {
            //arrange
            var parser = new SchemaParser ();
            var schema =
"""
version 1.0
event TestEvent cs,cpp
""";

            //act
            var result = parser.ParseSchema ( schema );

            //assert
            Assert.NotNull ( result );
            Assert.NotEmpty ( result.Events );
            var eventSchema = result.Events.First ();
            Assert.Equal ( "TestEvent", eventSchema.Name );
            Assert.NotEmpty ( eventSchema.HostLanguages );
            Assert.Equal ( 2, eventSchema.HostLanguages.Count () );
            Assert.Equal ( "cs", eventSchema.HostLanguages.First () );
            Assert.Equal ( "cpp", eventSchema.HostLanguages.Last () );
            Assert.Empty ( eventSchema.Properties );
        }

        [Fact, Trait ( "Category", "UnitTest" )]
        public void ParseSchema_Success_Event_TwoProperties () {
            //arrange
            var parser = new SchemaParser ();
            var schema =
"""
version 1.0
event TestEvent
int32 Prop1
int64 Prop2
""";

            //act
            var result = parser.ParseSchema ( schema );

            //assert
            Assert.NotNull ( result );
            Assert.NotEmpty ( result.Events );
            var eventSchema = result.Events.First ();
            Assert.Equal ( "TestEvent", eventSchema.Name );
            Assert.Empty ( eventSchema.HostLanguages );
            Assert.NotEmpty ( eventSchema.Properties );
            Assert.Equal ( 2, eventSchema.Properties.Count );
            var firstProperty = eventSchema.Properties.First ();
            Assert.Equal ( "Prop1", firstProperty.Name );
            Assert.Equal ( PropertyType.Int32, firstProperty.Type );
            var secondProperty = eventSchema.Properties.Last ();
            Assert.Equal ( "Prop2", secondProperty.Name );
            Assert.Equal ( PropertyType.Int64, secondProperty.Type );
        }

        [Fact, Trait ( "Category", "UnitTest" )]
        public void ParseSchema_Success_TwoEvents () {
            //arrange
            var parser = new SchemaParser ();
            var schema =
"""
version 1.0
event TestEvent
int32 Prop1
int64 Prop2

event TestEvent2
string Prop3
double Prop4
""";

            //act
            var result = parser.ParseSchema ( schema );

            //assert
            Assert.NotNull ( result );
            Assert.NotEmpty ( result.Events );
            Assert.Equal (2, result.Events.Count );

            var firstEventSchema = result.Events.First ();
            Assert.Equal ( "TestEvent", firstEventSchema.Name );
            Assert.Empty ( firstEventSchema.HostLanguages );
            Assert.NotEmpty ( firstEventSchema.Properties );
            Assert.Equal ( 2, firstEventSchema.Properties.Count );
            var firstProperty = firstEventSchema.Properties.First ();
            Assert.Equal ( "Prop1", firstProperty.Name );
            Assert.Equal ( PropertyType.Int32, firstProperty.Type );
            var secondProperty = firstEventSchema.Properties.Last ();
            Assert.Equal ( "Prop2", secondProperty.Name );
            Assert.Equal ( PropertyType.Int64, secondProperty.Type );

            var secondEventSchema = result.Events.Last ();
            Assert.Equal ( "TestEvent2", secondEventSchema.Name );
            Assert.Empty ( secondEventSchema.HostLanguages );
            Assert.NotEmpty ( secondEventSchema.Properties );
            Assert.Equal ( 2, secondEventSchema.Properties.Count );
            var thirdProperty = secondEventSchema.Properties.First ();
            Assert.Equal ( "Prop3", thirdProperty.Name );
            Assert.Equal ( PropertyType.String, thirdProperty.Type );
            var fooProperty = secondEventSchema.Properties.Last ();
            Assert.Equal ( "Prop4", fooProperty.Name );
            Assert.Equal ( PropertyType.Double, fooProperty.Type );
        }

    }

}