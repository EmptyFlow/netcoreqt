namespace NetCoreQt.Generator.CodeGenerators {

    /// <summary>
    /// Result of generation.
    /// </summary>
    public record GeneratedResultLanguage {

        /// <summary>
        /// Generated events.
        /// </summary>
        public Dictionary<string, string> Events { get; init; } = new Dictionary<string, string> ();

    }

}