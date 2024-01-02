namespace NetCoreQt.Generator.CodeSaver {

    /// <summary>
    /// Options for save source code.
    /// </summary>
    public record CodeSaverOptions {

        /// <summary>
        /// Languages options.
        /// </summary>
        public Dictionary<string, CodeSaverLanguage> Languages { get; init; } = [];

    }

}
