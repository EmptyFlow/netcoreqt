namespace NetCoreQt.Generator.SchemaParsers {

    public class Schema1dot0Parser {

        private const string EventDefinition = "event";

        private const string Int32Definition = "int32";

        private const string Int64Definition = "int64";

        private const string DoubleDefinition = "double";

        private const string StringDefinition = "string";

        private readonly HashSet<string> m_types = new () {
            Int32Definition, Int64Definition, DoubleDefinition, StringDefinition
        };

        private static PropertyType GetTypeFromString ( string type ) {
            return type switch {
                Int32Definition => PropertyType.Int32,
                Int64Definition => PropertyType.Int64,
                DoubleDefinition => PropertyType.Double,
                StringDefinition => PropertyType.String,
                _ => PropertyType.Unknown
            };
        }

        // Event parser
        // Format for event:
        //
        // event TestEvent cs,cpp
        // int32 Test

        private GenerateEventProperty ParseEventProperty ( string line ) {
            var span = line.AsSpan ();
            var spaceIndex = span.IndexOf ( ' ' );
            if ( spaceIndex == -1 ) throw new Exception ( "Event property must be in format: <property type> <propertyName>. Example: int32 MyProperty" );

            var propertyType = span[..spaceIndex];
            var propertyName = span[( spaceIndex + 1 )..];

            if ( !m_types.Contains ( propertyType.ToString () ) ) throw new Exception ( $"Event property must be with valid type! Allowed types: {Int32Definition}, {Int64Definition}, {DoubleDefinition}, {StringDefinition}" );

            return new GenerateEventProperty {
                Name = propertyName.ToString (),
                Type = GetTypeFromString ( propertyType.ToString () )
            };
        }

        internal GenerateEvent ParseEvent ( string startLine, SchemaParser parser ) {
            var span = startLine.AsSpan ();
            var eventSetup = span[EventDefinition.Length..];
            var spaceIndex = eventSetup.IndexOf ( ' ' );
            string? name;
            var hostLanguages = new List<string> ();
            if ( spaceIndex > -1 ) {
                name = eventSetup[..spaceIndex].ToString ().Trim ();
                hostLanguages.AddRange ( eventSetup[( spaceIndex + 1 )..].ToString ().Trim ().Split ( "," ) );
            } else {
                name = eventSetup.ToString ().Trim ();
            }

            if ( string.IsNullOrEmpty ( name ) ) throw new Exception ( "Name of event is empty. Format for name of event is: event <eventName>. Example: event MyEvent" );

            var properties = new List<GenerateEventProperty> ();

            while ( true ) {
                var entityLine = parser.GetNextEntityLine ();
                if ( entityLine != EntityLine.Unknown ) break;

                properties.Add ( ParseEventProperty ( parser.NextLine () ) );
            }

            return new GenerateEvent {
                Name = name,
                HostLanguages = hostLanguages,
                Properties = properties
            };
        }

        internal static bool IsEvent ( string line ) => line.StartsWith ( EventDefinition );

    }

}
