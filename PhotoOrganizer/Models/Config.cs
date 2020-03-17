using PhotoOrganizer.Primitives;

namespace PhotoOrganizer.Util {
    /// <summary>Structure of configuration file.</summary>
    public class Config {
        /// <summary>Path to working directory.</summary>
        public string WorkDir;
        /// <summary>Path to output directory.</summary>
        public string OutputDir;
        /// <summary>Print output to console.</summary>
        public bool TraceEnabled;
        /// <summary>Hash algorithm to use for checksum.</summary>
        public Algorithm HashAlgorithm;
        /// <summary>Type of renaming (COPY, MOVE or REPLACE).</summary>
        public string RenameType;
    }
}