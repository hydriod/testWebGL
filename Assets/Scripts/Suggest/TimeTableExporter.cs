using System.Collections;
using System.IO;
using System.Runtime.Serialization;
using UnityEngine.Networking;
using UnityEngine.Events;

namespace Suggest
{
    public static class TimeTableExporter
    {
        public static void Export<T>(T data, string filePath)
        {
            string directoryPath = Path.GetDirectoryName(filePath);
            if (directoryPath is not null && !Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            using (FileStream stream = new FileStream(filePath, FileMode.Create))
            {
                DataContractSerializer serializer = new DataContractSerializer(typeof(T));
                serializer.WriteObject(stream, data);
            }
        }

        public static IEnumerator Import<T>(string xmlPath, UnityAction<T> callback)
        {
            T result;
            
#if UNITY_EDITOR
#else
            UnityWebRequest request = new UnityWebRequest(xmlPath);
            request.downloadHandler = new DownloadHandlerFile(xmlPath); 
            yield return request.SendWebRequest();
#endif

            using (FileStream stream = new FileStream(xmlPath, FileMode.Open))
            {
                DataContractSerializer deserializer = new DataContractSerializer(typeof(T));
                result = (T)deserializer.ReadObject(stream);
            }

            callback(result);

            yield break;
        }
    }
}
