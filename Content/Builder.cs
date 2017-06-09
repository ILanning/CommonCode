using System.IO;
using System.Xml.Serialization;

namespace CommonCode.Content
{
    /// <summary>
    /// Base class for all serializeable objects.
    /// </summary>
    /// <typeparam name="T">Type of the child class.</typeparam>
    public abstract class Builder<T> where T : Builder<T>
    {
        static public T BuilderRead(string filePath)
        {
            T newObject;
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
            StreamReader streamReader = new StreamReader(filePath);
            newObject = (T)xmlSerializer.Deserialize(streamReader);
            streamReader.Close();
            return newObject;
        }

        static public void Create(object toBeSerialized, string filePath)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(toBeSerialized.GetType());            
            StreamWriter streamWriter = new StreamWriter(File.Create(filePath));
            xmlSerializer.Serialize(streamWriter, toBeSerialized);
            streamWriter.Close();
        }
    }
}
