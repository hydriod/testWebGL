using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;


namespace Suggest
{
    /// <summary>
    /// デバッグログに出力するための関数を集めたクラス
    /// </summary>
    public class TimeTablePrinter
    {
        public static void printTimeTable(int?[][][] timeTables, string tableName, Dictionary<int, Subject> Subjects)
        {
            StringBuilder log = new StringBuilder();

            log.Append(tableName).Append("\n");
            foreach (int?[][] table in timeTables)
            {
                log.Append("###########\n");
                foreach (int?[] row in table)
                {
                    log.Append("[");
                    foreach (int? item in row)
                    {
                        log.Append(SubjectToString(item, Subjects)).Append(", ");
                    }
                    log.Append("]\n");
                }
                log.Append("###########\n");
            }

            Debug.Log(log.ToString());
        }
        public static void printTimeTable(List<int>[][][] timeTables, string tableName, Dictionary<int, Subject> Subjects)
        {
            StringBuilder log = new StringBuilder();

            log.Append(tableName).Append("\n");
            foreach (List<int>[][] table in timeTables)
            {
                log.Append("###########\n");
                foreach (List<int>[] row in table)
                {
                    log.Append("[");
                    foreach (List<int> item in row)
                    {
                        log.Append(SubjectToString(item, Subjects)).Append(", ");
                    }
                    log.Append("]\n");
                }
                log.Append("###########\n");
            }

            Debug.Log(log.ToString());
        }
        public static void printTimeTable<T>(T[][][] timeTables, string tableName, Dictionary<int, Subject> Subjects)
        {
            StringBuilder log = new StringBuilder();

            log.Append(tableName).Append("\n");
            foreach (T[][] table in timeTables)
            {
                log.Append("###########\n");
                foreach (T[] row in table)
                {
                    log.Append("[");
                    foreach (T item in row)
                    {
                        log.Append(SubjectToString(item, Subjects)).Append(", ");
                    }
                    log.Append("]\n");
                }
                log.Append("###########\n");
            }

            Debug.Log(log.ToString());
        }

        static string SubjectToString(int? id, Dictionary<int, Subject> Subjects)
        {
            if(id is null)
            {
                return "";
            }
            return Subjects[(int)id].name;
        }
        static string SubjectToString(Subject subject, Dictionary<int, Subject> Subjects)
        {
            return subject.name;
        }
        static string SubjectToString(List<int> list, Dictionary<int, Subject> Subjects)
        {
            StringBuilder stringBuilder = new StringBuilder("(");
            foreach (int item in list)
            {
                stringBuilder.Append(Subjects[item].name).Append(", ");
            }
            stringBuilder.Append(")");

            return stringBuilder.ToString();
        }
        static string SubjectToString(List<Subject> list, Dictionary<int, Subject> Subjects)
        {
            StringBuilder stringBuilder = new StringBuilder("(");
            foreach (Subject item in list)
            {
                stringBuilder.Append(item.name).Append(", ");
            }
            stringBuilder.Append(")");

            return stringBuilder.ToString();
        }
        static string SubjectToString<T>(T obj, Dictionary<int, Subject> Subjects)
        {
            if(obj is null)
            {
                return "";
            }
            return obj.ToString();
        }
    }
}

