using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

/// <summary>
/// Class containing the method to clone a serializable type to another object to prevent shallow copying
/// </summary>
public static class ObjectCloner
{
    ///<summary>
    ///Provides a deep copy of an object via binary serialization
    /// </summary>
    ///<typeparam name="T">Type of the object being copied</typeparam>
    ///<param name="source">The object instance to copy</param>
    ///<returns>A copy of the object</returns>
    public static T Clone<T>(this T source)
    {
        if (!typeof(T).IsSerializable)
        {
            throw new ArgumentException("Not a serializable type", nameof(source));
        }

        //Ensure object is not null
        if (source == null) { return default; }
        
        //Create binary formatter
        Stream stream = new MemoryStream();
        IFormatter formatter = new BinaryFormatter();
        formatter.Serialize(stream, source);
        stream.Seek(0, SeekOrigin.Begin);
        return (T)formatter.Deserialize(stream);
        
    }
}
