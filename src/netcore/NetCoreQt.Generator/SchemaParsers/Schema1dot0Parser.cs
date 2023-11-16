using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace NetCoreQt.Generator.SchemaParsers {

    public class Schema1dot0Parser {

        private const string EventDefinition = "event ";

        private const string Int32Definition = "int32 ";

        private const string Int64Definition = "int64 ";

        private const string DoubleDefinition = "double ";

        private const string StringDefinition = "string ";

        private readonly HashSet<string> m_types = new HashSet<string> {
            Int32Definition, Int64Definition, DoubleDefinition, StringDefinition
        };

        private GenerateEventProperty ParseEventProperty ( string line ) {
            var span = line.AsSpan ();
            var spaceIndex = span.IndexOf ( ' ' );
            if (spaceIndex == -1) throw new Exception ( "Event property must be in format: <property type> <propertyName>. Example: int32 MyProperty" );

            var propertyType = span.Slice ( 0, spaceIndex );
            var propertyName = span.Slice ( spaceIndex + 1 );

            if ( !m_types.Contains ( propertyType.ToString() ) ) throw new Exception ( $"Event property must be with valid type! Allowed types: {Int32Definition}, {Int64Definition}, {DoubleDefinition}, {StringDefinition}" );

        }

        private GenerateEvent ParseEvent ( string startLine, SchemaParser parser ) {
            var span = startLine.AsSpan ();
            var eventSetup = span.Slice ( EventDefinition.Length );
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

            return new GenerateEvent {
                Name = name,
                HostLanguages = hostLanguages
            };
        }
        /*event TestEvent cs,cpp
        int32 Test*/

    }

}
