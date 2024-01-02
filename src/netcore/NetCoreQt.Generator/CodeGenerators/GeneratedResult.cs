namespace NetCoreQt.Generator.CodeGenerators {

    /// <summary>
    /// Generated result.
    /// </summary>
    public sealed record GeneratedResult {

        /// <summary>
        /// Objects for C# language.
        /// </summary>
        public GeneratedResultLanguage CsLanguage { get; init; } = new GeneratedResultLanguage();

        /// <summary>
        /// Object to C++ language.
        /// </summary>
        public GeneratedResultLanguage CppLanguage { get; init; } = new GeneratedResultLanguage ();

    }

}
