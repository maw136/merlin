using System.IO;

namespace MarWac.Merlin.SourceDrivers
{
    /// <summary>
    /// Retrieves/stores the configSettings from/to some stream.
    /// </summary>
    public abstract class ConfigurationSourceDriver
    {
        /// <summary>
        /// Retrieves the configSettings from the source stream.
        /// </summary>
        /// <param name="source">A source stream</param>
        /// <returns>ConfigSettings instance filled with data from the the source</returns>
        /// <exception cref="SourceReadException">Thrown if there is a problem the configSettings from a particular 
        /// source.</exception>
        public abstract ConfigSettings Read(Stream source);

        /// <summary>
        /// Stores the configSettings to the output stream.
        /// </summary>
        /// <param name="output">An output stream to store the configSettings</param>
        /// <param name="configSettings">ConfigSettings instance to be stored to the stream</param>
        public abstract void Write(Stream output, ConfigSettings configSettings);
    }
}