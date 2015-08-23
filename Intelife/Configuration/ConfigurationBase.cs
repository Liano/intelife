using System.ComponentModel;

namespace Intelife.Configuration
{
  /// <summary>
  /// Base class of all configuration entities
  /// </summary>
  public abstract class ConfigurationBase
  {
    /// <summary>
    /// Get the name of the configuration
    /// </summary>
    public abstract string Name { get; set; }

    /// <summary>
    /// Clones the descriptions.
    /// </summary>
    /// <param name="configBase">The configuration base.</param>
    public abstract void CloneDescriptions(ConfigurationBase configBase);
  }

}
