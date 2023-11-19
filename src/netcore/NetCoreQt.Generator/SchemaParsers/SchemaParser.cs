namespace NetCoreQt.Generator.SchemaParsers {

    public class SchemaParser {

        private IEnumerable<string> m_lines = new List<string> ();

        private int m_lineIndex = 0;

        internal async Task<GenerateSchema> ParseAsync ( string fileName ) {
            var content = await File.ReadAllTextAsync ( fileName );

            m_lines = content.Split ( '\n' );

            //var versionLine = content.

            var schema = new GenerateSchema ();
            var parser = new Schema1dot0Parser ();

            while ( m_lineIndex < m_lines.Count () ) {
                var entityLine = GetNextEntityLine ();
                switch ( entityLine ) {
                    case EntityLine.Event:
                        schema.Events.Add ( parser.ParseEvent ( NextLine (), this ) );
                        break;
                }
            }

            return schema;
        }

        internal EntityLine GetNextEntityLine () {
            if (m_lineIndex == m_lines.Count() - 1) return EntityLine.EndLine;

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
