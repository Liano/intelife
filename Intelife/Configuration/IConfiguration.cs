namespace Intelife.Configuration
{
  /// <summary>
  /// Interface for interracting with a configuration handler
  /// </summary>
  public interface IConfiguration
  {
    /// <summary>
    /// Get a configuration object
    /// </summary>
    /// <typeparam name="T">Type of the configuration to get</typeparam>
    /// <returns>Configuration object</returns>
    T GetConfiguration<T>() where T : ConfigurationBase;
    /// <summary>
    /// Remove a configuration
    /// </summary>
    /// <typeparam name="T">Type of the configuration to remove</typeparam>
    void RemoveConfiguration<T>();
    /// <summary>
    /// Save a configuration object
    /// </summary>
    /// <param name="config">The configuration object to save</param>
    void SaveConfig(ConfigurationBase config);
    /// <summary>
    /// Save all configurations actually handled with this instance.
    /// </summary>
    void SaveAllConfigurations();
  }
}
