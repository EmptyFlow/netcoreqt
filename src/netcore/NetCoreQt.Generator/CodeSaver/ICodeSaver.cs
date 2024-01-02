using NetCoreQt.Generator.CodeGenerators;

namespace NetCoreQt.Generator.CodeSaver {

    /// <summary>
    /// Code saver.
    /// </summary>
    internal interface ICodeSaver {

        /// <summary>
        /// Save sources files asynchronized.
        /// </summary>
        /// <param name="options">Options.</param>
        /// <param name="generatedResult">Generated result.</param>
        Task SaveAsync ( CodeSaverOptions options, GeneratedResult generatedResult );

    }

}
