using System.Collections.Generic;

namespace MarWac.Merlin
{
    public class Configuration
    {
        public Configuration()
        {
            Parameters = new List<ConfigurationParameter>();    
        }

        public IList<ConfigurationParameter> Parameters { get; }
    }
}