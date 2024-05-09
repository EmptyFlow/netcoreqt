namespace NetCoreQt.Generator.SchemaParsers {

    /// <summary>
    /// Parsed schema for generation.
    /// </summary>
    internal record GenerateSchema {

        /// <summary>
        /// Version of schema.
        /// </summary>
        public string Version { get; init; } = "";

        public string DefaultNamespace { get; set; } = "";

        public List<GenerateEvent> Events { get; set; } = new List<GenerateEvent>();

    }

}