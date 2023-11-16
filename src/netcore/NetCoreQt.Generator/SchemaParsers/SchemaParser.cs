using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCoreQt.Generator.SchemaParsers {

    public class SchemaParser {

        public async Task<GenerateSchema> ParseAsync(string fileName ) {
            var content = await File.ReadAllTextAsync( fileName );

            var versionLine = content.

            return new GenerateSchema {

            };
        }

        public GenerateSchema Parse ( string fileName ) {
            return null;
        }

        public string NextLine () {
            return "";
        }

    }

}
