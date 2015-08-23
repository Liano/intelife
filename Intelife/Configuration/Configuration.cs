using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Intelife.Configuration
{
  public class Configuration : IConfiguration
  {
    const string CONFIG_FOLDER = "config";
    IObjectSerializer _serializer;
    Dictionary<string, ConfigurationBase> _configurations;
    static object _dicLock = new object();

    public Configuration(IObjectSerializer serializer)
    {
      if (serializer == null)
        throw new ArgumentNullException("Serializer is null");

      this._serializer = serializer;
      this._configurations = new Dictionary<string, ConfigurationBase>();
    }

    public T GetConfiguration<T>() where T : ConfigurationBase
    {
      var name = typeof(T).FullName;

      if (string.IsNullOrEmpty(name))
        throw new ArgumentNullException("Setting name is empty");

      if (this._configurations.Keys.Contains(name))
      {
        return this._configurations[name] as T;
      }
      else
      {
        T configuration = null;
        try
        {
          //get configuration from serializer
          var configFile = Configuration.GetConfigurationFilePath(name);
          var data = File.ReadAllText(configFile);
          configuration = this._serializer.Deserialize<T>(data);

          //add configuration to configurations collection
          lock (_dicLock)
          {
            this._configurations.Add(name, configuration);
          }
          
        }
        catch (FileNotFoundException)
        {
          throw new InvalidOperationException
            ("No previousely saved configuration available with the given name");
        }
        catch (Exception)
        {

          throw;
        }

        return configuration;
      }
    }

    public void RemoveConfiguration<T>()
    {
      var name = typeof(T).FullName;

      if (string.IsNullOrEmpty(name))
        throw new ArgumentNullException("configuration name is empty");

      //remove configuration from configurations collection.
      if (this._configurations.Keys.Contains(name))
      {
        lock (_dicLock)
        {
          this._configurations.Remove(name); 
        }
      }

      //remove configuration file
      var configFile = Configuration.GetConfigurationFilePath(name);
      if (File.Exists(configFile))
      {
        try
        {
          File.Delete(configFile);
        }
        catch (Exception)
        {
          throw;
        }
      }
    }

    public void SaveAllConfigurations()
    {
      //recurse all available configurations and serialize them
      foreach (var config in this._configurations)
      {
        Save(config.Key, config.Value);
      }
    }

    private void Save(string name, ConfigurationBase config)
    {
      if (string.IsNullOrEmpty(name))
        throw new ArgumentNullException("configuration name is null");

      if (config == null)
        throw new ArgumentNullException("configuration is null");

      var configFile = Configuration.GetConfigurationFilePath(name);
      var serializationData = this._serializer.Serialize(config);

      if (File.Exists(configFile))
      {
        File.Delete(configFile);
      }

      File.WriteAllText(configFile, serializationData);
    }

    public void SaveConfig(ConfigurationBase config)
    {
      if (config == null)
        throw new ArgumentNullException("configuration is null");

      if (string.IsNullOrEmpty(config.Name))
        throw new ArgumentNullException("configuration has invalid name");

      if (this._configurations.Keys.Contains(config.Name))
      {
        this._configurations[config.Name] = config;
      }

      this.Save(config.Name, config);
    }

    private static string GetConfigurationFilePath(string name)
    {
      if (string.IsNullOrEmpty(name))
        throw new ArgumentNullException("configuration name is empty");

      return string.Format(@"{0}\{1}.cfg", CONFIG_FOLDER, name);
    }
  }
}
