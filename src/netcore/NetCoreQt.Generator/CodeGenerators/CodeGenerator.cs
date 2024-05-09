using NetCoreQt.Generator.SchemaParsers;
using System.Runtime.CompilerServices;
using System.Text;

[assembly: InternalsVisibleTo ( "NetCoreQt.Generator.UnitTests" )]
namespace NetCoreQt.Generator.CodeGenerators {

    internal class CodeGenerator : ICodeGenerator {

        public const string CsLanguage = "cs";

        public const string CppLanguage = "cpp";

        public Task GenerateClassesWithProjects ( IEnumerable<string> languages, GenerateSchema schema ) {
            throw new NotImplementedException ();
        }

        public GeneratedResult GenerateClasses ( IEnumerable<string> languages, GenerateSchema schema ) {
            var result = new GeneratedResult ();

            if ( schema.Events.Any () ) {
                foreach ( var language in languages ) {
                    switch ( language ) {
                        case CsLanguage:
                            GenerateCsClasses ( schema, result.CsLanguage );
                            break;
                        case CppLanguage:
                            GenerateCppClasses ( schema, result.CppLanguage );
                            break;
                        default: throw new NotImplementedException ();
                    }
                }
            }

            return result;
        }

        private Task GenerateCsClasses ( GenerateSchema schema, GeneratedResultLanguage result ) {
            foreach ( var item in schema.Events ) {
                string generatedEvent;
                if ( item.HostLanguages.Contains ( CsLanguage ) ) {
                    generatedEvent = GenerateCsHostEvent ( item );
                } else {
                    generatedEvent = GenerateCsGuestEvent ( item );
                }

                if ( result.Events.ContainsKey ( item.Name ) ) continue;

                result.Events.Add ( item.Name, generatedEvent );
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

            private static System.Collections.Concurrent.ConcurrentDictionary<int, {{item.Name}}> m_events = new ();

            private static int m_counter;

            private delegate void FireEventDelegate ( nint value );

            private static FireEventDelegate? m_fireEventDelegateHandler;

            [System.Runtime.InteropServices.UnmanagedCallersOnly]
            public static void CompleteEvent ( nint index ) {
                var id = index.ToInt32 ();
                if ( !m_events.ContainsKey ( id ) ) return;

                m_events.TryRemove ( id, out var _ );
            }

            [System.Runtime.InteropServices.UnmanagedCallersOnly]
            public static void FireEventCallback ( nint callback ) {
                m_fireEventDelegateHandler = System.Runtime.InteropServices.Marshal.GetDelegateForFunctionPointer<FireEventDelegate> ( callback );
            }

{{GetExternalProperties ( item.Properties )}}

            public static int Create ( {{item.Name}} newEvent ) {
                var value = System.Threading.Interlocked.Increment ( ref m_counter );
                if ( !m_events.TryAdd ( value, newEvent ) ) throw new System.Exception ( $"Can't create event with index {value}" );

                m_fireEventDelegateHandler?.Invoke ( value );
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
                    PropertyType.Int64 => $"return (nint)value.{name};",
                    PropertyType.Double => $"return (nint)value.{name};",
                    PropertyType.String => $"return System.Runtime.InteropServices.Marshal.StringToHGlobalUni ( value.{name} is string result ? result : \"\" )",
                    _ => ""
                };
            }

            static string GetExternalProperties ( List<GenerateEventProperty> properties ) {
                var builder = new StringBuilder ();
                foreach ( var property in properties ) {
                    var externalProperty = $$"""
            [System.Runtime.InteropServices.UnmanagedCallersOnly]
            public static nint Get{{property.Name}} ( nint index ) {
                if ( m_events.TryGetValue ( index.ToInt32 (), out var value ) ) {
                    {{GetTypeNIntReturn ( property.Type, property.Name )}}
                }

                return 0;
            }
""";
                    builder.AppendLine ( externalProperty );
                }

                return builder.ToString ();
            }

        }

        private Task GenerateCppClasses ( GenerateSchema schema, GeneratedResultLanguage result ) {
            foreach ( var item in schema.Events ) {
                string generatedEvent;
                if ( item.HostLanguages.Contains ( CsLanguage ) ) {
                    generatedEvent = GenerateCppHostEvent ( item );
                } else {
                    generatedEvent = GenerateCppGuestEvent ( item, schema.DefaultNamespace );
                }

                if ( result.Events.ContainsKey ( item.Name ) ) continue;

                result.Events.Add ( item.Name, generatedEvent );
            }

            return Task.CompletedTask;
        }

        private static string GenerateCppGuestEvent ( GenerateEvent item, string defaultNamespace ) => CppEventCodeGenerator.GenerateGuestEvent ( item, defaultNamespace );

        private string GenerateCppHostEvent ( GenerateEvent item ) {
            throw new NotImplementedException ();
        }
    }

}
