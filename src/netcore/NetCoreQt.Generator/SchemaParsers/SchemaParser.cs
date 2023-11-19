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

            //var versionLine = content.

            var schema = new GenerateSchema ();
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
