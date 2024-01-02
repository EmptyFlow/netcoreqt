namespace NetCoreQt.Generator.CodeSaver {

    public record CodeSaverLanguage {

        public string TargetFolder { get; set; } = "";

        public bool SaveAsSingleFile { get; set; }

        public string SingleFileName { get; set; } = "";

    }

}