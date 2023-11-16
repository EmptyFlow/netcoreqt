namespace NetCoreQt.Generator.SchemaParsers {

    /// <summary>
    /// Parsed schema for generation.
    /// </summary>
    public record GenerateSchema {

        /// <summary>
        /// Version of schema.
        /// </summary>
        public string Version { get; init; } = "";

    }

}