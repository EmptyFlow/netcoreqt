using NetCoreQt.Generator.SchemaParsers;
using System.Text;

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

            if ( schema.Events.Any () ) {
                foreach ( var language in languages ) {
                    switch ( language ) {
                        case CsLanguage:
                            GenerateCsClasses ( schema );
                            break;
                        case CppLanguage:
                            GenerateCppClasses ( schema );
                            break;
                        default: throw new NotImplementedException ();
                    }
                }
            }

            return Task.CompletedTask;
        }

        private static string m_eventHostTemplate = "";

        private static string GetContentFromTemplate ( string templateName ) {
            if ( templateName == null ) throw new ArgumentNullException ( nameof ( templateName ) );

            var assembly = typeof ( CodeGenerator ).Assembly;
            if ( assembly == null ) return "";

            using var stream = assembly.GetManifestResourceStream ( templateName );
            if ( stream == null ) return "";

            using StreamReader reader = new ( stream );

            return reader.ReadToEnd ();
        }

        private static string GetEventHostTemplate () {
            if ( string.IsNullOrEmpty ( m_eventHostTemplate ) ) {
                m_eventHostTemplate = GetContentFromTemplate ( "NetCoreQt.Generator.Templates.HostEventBaseClassInteral.template" );
            }

            return m_eventHostTemplate;
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

        private static string GenerateCsHostEvent ( GenerateEvent item ) {
            return $$"""
        public record {{item.Name}} ({{GetEventProperties ( item.Properties )}});

        public static class {{item.Name}}External {

            private static ConcurrentDictionary<int, {{item.Name}}> m_events = new ();

            private static int m_counter;

            private delegate void FireEventDelegate ( IntPtr value );

            private static FireEventDelegate? m_fireEventDelegateHandler;

            [UnmanagedCallersOnly]
            public static void CompleteEvent ( IntPtr index ) {
                var id = index.ToInt32 ();
                if ( !m_events.ContainsKey ( id ) ) return;

                m_events.TryRemove ( id, out var _ );
            }

            [UnmanagedCallersOnly]
            public static void FireEventCallback ( IntPtr callback ) {
                fireEventDelegateHandler = Marshal.GetDelegateForFunctionPointer<FireEventDelegate> ( callback );
            }

            {{GetExternalProperties ( item.Properties )}}

            public static int Create ( {{item.Name}} newEvent ) {
                var value = Interlocked.Increment ( ref m_counter );
                if ( !m_events.TryAdd ( value, newEvent ) ) throw new Exception ( $"Can't create event with index {value}" );

                fireEventDelegateHandler?.Invoke ( value );
                return value;
            }

        }
""";

            static string GetEventProperties ( List<GenerateEventProperty> properties ) {
                return string.Join (
                    ", ",
                    properties.Select ( a => {
                        return a.Type switch {
                            PropertyType.Int32 => $"int {a.Name}",
                            PropertyType.Int64 => $"long {a.Name}",
                            PropertyType.Double => $"double {a.Name}",
                            PropertyType.String => $"string {a.Name}",
                            _ => ""
                        };
                    } )
                    .ToList ()
                );
            }

            static string GetTypeNIntReturn ( PropertyType type, string name ) {
                return type switch {
                    PropertyType.Int32 => $"return value.{name};",
                    PropertyType.Int64 => $"return value.{name};",
                    PropertyType.Double => $"return value.{name};",
                    PropertyType.String => $"return Marshal.StringToHGlobalUni ( value.{name} is string result ? result : \"\" )",
                    _ => ""
                };
            }

            static string GetExternalProperties ( List<GenerateEventProperty> properties ) {
                var builder = new StringBuilder ();
                foreach ( var property in properties ) {
                    var externalProperty = $$"""
            [UnmanagedCallersOnly]
            public static void Get{{property.Name}} ( IntPtr index ) {
                if ( m_events.TryGetValue ( index.ToInt32 (), out var value ) ) {
                    {{GetTypeNIntReturn ( property.Type, property.Name )}}
                }

                return -1;
            }
""";
                    builder.AppendLine ();
                }

                return builder.ToString ();
            }

        }

        private void GenerateCppClasses ( GenerateSchema schema ) {
            throw new NotImplementedException ();
        }

    }

}
