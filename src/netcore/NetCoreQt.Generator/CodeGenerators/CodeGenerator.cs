using NetCoreQt.Generator.SchemaParsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCoreQt.Generator.CodeGenerators {

    internal class CodeGenerator : ICodeGenerator {

        private const string CsLanguage = "cs";

        private const string CppLanguage = "cpp";

        private readonly Dictionary<string, string> m_events = new ();

        public Task GenerateClassesWithProjects ( IEnumerable<string> languages, GenerateSchema schema ) {
            throw new NotImplementedException ();
        }

        public Task GenerateOnlyClasses ( IEnumerable<string> languages, GenerateSchema schema ) {
            m_events.Clear ();

            return Task.CompletedTask;
        }

        private Task GenerateCsClasses ( GenerateSchema schema ) {
            foreach ( var item in schema.Events ) {
                string generatedEvent;
                if ( item.HostLanguages.Contains ( CsLanguage ) ) {
                    generatedEvent = GenerateCsHostEvent ( item );
                } else {
                    generatedEvent = GenerateCsGuestEvent ( item );
                }
                m_events.Add ( CsLanguage, generatedEvent );
            }

            return Task.CompletedTask;
        }

        private string GenerateCsGuestEvent ( GenerateEvent item ) {
            return "";
        }

        private string GenerateCsHostEvent ( GenerateEvent item ) {
            throw new NotImplementedException ();
        }
    }

}
