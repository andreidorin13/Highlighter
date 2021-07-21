using System.Collections.Generic;

namespace Highlighter.Models {
    public sealed class ObsSettings {
        public string Path { get; set; }

        public string Uri { get; set; }

        public string Password { get; set; }

        public string RecordDir { get; set; }

        public bool DisplayWindow { get; set; }

        public string SourceName { get; set; }
    }

    public sealed class GameSettings {
        public string Name { get; set; }

        public string Exe { get; set; }
    }

    public sealed class Settings {
        public int PollInterval { get; set; }

        public ObsSettings Obs { get; set; }
      
        public IList<GameSettings> Games { get; set; }
    }
}
