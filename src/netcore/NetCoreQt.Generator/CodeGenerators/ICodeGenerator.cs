using NetCoreQt.Generator.SchemaParsers;

namespace NetCoreQt.Generator.CodeGenerators {

    /// <summary>
    /// Code generator that generate file with source code on one or few languages.
    /// </summary>
    internal interface ICodeGenerator {

        /// <summary>
        /// Generate only classes.
        /// </summary>
        Task GenerateOnlyClasses ( IEnumerable<string> languages, GenerateSchema schema );

        /// <summary>
        /// Generate projects.
        /// </summary>
        Task GenerateClassesWithProjects ( IEnumerable<string> languages, GenerateSchema schema );

    }

}
