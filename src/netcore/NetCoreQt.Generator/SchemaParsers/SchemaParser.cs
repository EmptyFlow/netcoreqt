using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo ( "NetCoreQt.Generator.UnitTests" )]
namespace NetCoreQt.Generator.SchemaParsers {

    public class SchemaParser {

        private IEnumerable<string> m_lines = new List<string> ();

        private int m_lineIndex = 0;

        internal async Task<GenerateSchema> ParseAsync ( string fileName ) => ParseSchema ( await File.ReadAllTextAsync ( fileName ) );

        internal GenerateSchema ParseSchema ( string content ) {
            m_lines = content
                .Replace ( "\r", "" ) // fix for Windows \n\r format
                .Split ( '\n' );

            if ( !m_lines.Any () ) throw new Exception ( "Schema is empty! I guess if you need something you need to fill out the schema first!" );

            var versionLine = m_lines.First ().Split ( " " );
            if ( versionLine.Length != 2 ) throw new Exception ( "Schema version not filled or not relevant! Version defined in format: version X.X. Example: version 1.0" );

            var schema = new GenerateSchema {
                Version = versionLine.Last ().Trim ()
            };
            var parser = new Schema1dot0Parser ();

            while ( m_lineIndex < m_lines.Count () ) {
                var entityLine = GetNextEntityLine ();
                if ( entityLine == EntityLine.EndLine ) break;

                switch ( entityLine ) {
                    case EntityLine.Event:
                        schema.Events.Add ( parser.ParseEvent ( NextLine (), this ) );
                        break;
                }
            }

            return schema;
        }

        internal EntityLine GetNextEntityLine () {
            if ( m_lineIndex == m_lines.Count () - 1 ) return EntityLine.EndLine;

            var line = m_lines.ElementAt ( m_lineIndex + 1 );
            if ( Schema1dot0Parser.IsEvent ( line ) ) return EntityLine.Event;

            return EntityLine.Unknown;
        }

        internal string GetCurrentLine () => m_lines.ElementAt ( m_lineIndex );

        internal string NextLine () {
            m_lineIndex += 1;
            return m_lines.ElementAt ( m_lineIndex );
        }

    }

}
