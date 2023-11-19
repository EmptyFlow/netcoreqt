namespace NetCoreQt.Generator.SchemaParsers {

    internal record GenerateEventProperty {

        public string Name { get; init; } = "";

        public PropertyType Type { get; init; } = PropertyType.Unknown;

    }

}