﻿using System.Collections.Generic;

namespace MarWac.Merlin
{
    /// <summary>
    /// Represents configuration of some entity/system.
    /// </summary>
    public class Configuration
    {
        /// <summary>
        /// Creates a configuration instance with empty parameter list.
        /// </summary>
        public Configuration()
        {
            Parameters = new List<ConfigurationParameter>();    
        }

        /// <summary>
        /// All parameters defined in the configuration
        /// </summary>
        public IList<ConfigurationParameter> Parameters { get; }
    }
}