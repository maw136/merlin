using System.IO;

namespace MarWac.Merlin.SourceDrivers
{
    /// <summary>
    /// Retrieves/stores the configuration from/to some stream.
    /// </summary>
    public abstract class ConfigurationSourceDriver
    {
        /// <summary>
        /// Retrieves the configuration from the source stream.
        /// </summary>
        /// <param name="source">A source stream</param>
        /// <returns>Configuration instance filled with data from the the source</returns>
        /// <exception cref="SourceReadException">Thrown if there is a problem the configuration from a particular 
        /// source.</exception>
        public abstract Configuration Read(Stream source);

        /// <summary>
        /// Stores the configuration to the output stream.
        /// </summary>
        /// <param name="output">An output stream to store the configuration</param>
        /// <param name="configuration">Configuration instance to be stored to the stream</param>
        public abstract void Write(Stream output, Configuration configuration);
    }
}