using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Intelife.Configuration
{
  /// <summary>
  /// Contract for object serializers
  /// </summary>
  public interface IObjectSerializer
  {
    /// <summary>
    /// serialize an object
    /// </summary>
    /// <param name="obj">the object to serialize</param>
    /// <returns>serialized object as string data</returns>
    string Serialize(object obj);

    /// <summary>
    /// Desrialize a string to an object of the given type
    /// </summary>
    /// <typeparam name="T">Deserialize the given data to this type</typeparam>
    /// <param name="str">contents to deserialize</param>
    /// <return>deserialization object</returns>
    T Deserialize<T>(string str);

    /// <summary>
    /// Deserializes the specified string.
    /// </summary>
    /// <param name="str">deserialization data</param>
    /// <param name="type">Deserialization type</param>
    /// <returns>Deserialized type.</returns>
    object Deserialize(string str, Type type);
  }

}
