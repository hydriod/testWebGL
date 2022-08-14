using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using UnityEngine;

namespace Suggest
{
    public class TimeTableExporter : MonoBehaviour
    {
        [SerializeField]
        Suggest suggest;
        public void ExportTimeTable()
        {
            Export(suggest.suggestTimeTable, "TimeTable.xml");
            Export(suggest.Subjects, "Syllabus.xml");
        }
        public void Export<T>(T data, string fileName)
        {
            using (FileStream stream = new FileStream(fileName, FileMode.Create))
            {
                DataContractSerializer serializer = new DataContractSerializer(typeof(T));
                serializer.WriteObject(stream, data);
            }
        }

    }
}
