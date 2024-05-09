namespace NetCoreQt.Generator.SchemaParsers {

    /// <summary>
    /// Respresent event for generation.
    /// </summary>
    internal record GenerateEvent {

        public string Name { get; init; } = "";

        public IEnumerable<string> HostLanguages { get; init; } = new List<string>();

        public List<GenerateEventProperty> Properties { get; internal set; } = new List<GenerateEventProperty>();
    }

}