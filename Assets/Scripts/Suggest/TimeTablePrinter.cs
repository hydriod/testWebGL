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
        /// <summary>
        /// Suggest.Syllabusなどをログに流す
        /// </summary>
        /// <param name="syllabus"></param>
        public static void printSyllabus(Dictionary<int, Subject> syllabus)
        {
            StringBuilder logString = new StringBuilder("Syllabus\n");
            foreach (var (id, subject) in syllabus)
            {
                logString.Append(id).Append(" : ").Append(subject.name).Append("\n");
            }

            Debug.Log(logString.ToString());
        }

        /// <summary>
        /// 時間割の内容をログに流す
        /// </summary>
        /// <param name="timeTables"></param>
        /// <param name="tableName"></param>
        /// <param name="syllabus"></param>
        public static void printTimeTable(int?[][][] timeTables, string tableName, Dictionary<int, Subject> syllabus)
        {
            StringBuilder logString = new StringBuilder();

            logString.Append(tableName).Append("\n");
            foreach (int?[][] table in timeTables)
            {
                logString.Append("###########\n");
                foreach (int?[] row in table)
                {
                    logString.Append("[");
                    foreach (int? item in row)
                    {
                        logString.Append(SubjectToString(item, syllabus)).Append(", ");
                    }
                    logString.Append("]\n");
                }
                logString.Append("###########\n");
            }

            Debug.Log(logString.ToString());
        }

        /// <summary>
        /// 時間割の内容をログに流す
        /// </summary>
        /// <param name="timeTables"></param>
        /// <param name="tableName"></param>
        /// <param name="syllabus"></param>
        public static void printTimeTable(List<int>[][][] timeTables, string tableName, Dictionary<int, Subject> syllabus)
        {
            StringBuilder logString = new StringBuilder();

            logString.Append(tableName).Append("\n");
            foreach (List<int>[][] table in timeTables)
            {
                logString.Append("###########\n");
                foreach (List<int>[] row in table)
                {
                    logString.Append("[");
                    foreach (List<int> item in row)
                    {
                        logString.Append(SubjectToString(item, syllabus)).Append(", ");
                    }
                    logString.Append("]\n");
                }
                logString.Append("###########\n");
            }

            Debug.Log(logString.ToString());
        }
        public static void printTimeTable<T>(T[][][] timeTables, string tableName, Dictionary<int, Subject> syllabus)
        {
            StringBuilder logString = new StringBuilder();

            logString.Append(tableName).Append("\n");
            foreach (T[][] table in timeTables)
            {
                logString.Append("###########\n");
                foreach (T[] row in table)
                {
                    logString.Append("[");
                    foreach (T item in row)
                    {
                        logString.Append(SubjectToString(item, syllabus)).Append(", ");
                    }
                    logString.Append("]\n");
                }
                logString.Append("###########\n");
            }

            Debug.Log(logString.ToString());
        }

        // 以下,科目名に変換する内部関数

        static string SubjectToString(int? id, Dictionary<int, Subject> syllabus)
        {
            if(id is null)
            {
                return "";
            }
            return syllabus[(int)id].name;
        }
        static string SubjectToString(Subject subject, Dictionary<int, Subject> syllabus)
        {
            return subject.name;
        }
        static string SubjectToString(List<int> list, Dictionary<int, Subject> syllabus)
        {
            StringBuilder stringBuilder = new StringBuilder("(");
            foreach (int item in list)
            {
                stringBuilder.Append(syllabus[item].name).Append(", ");
            }
            stringBuilder.Append(")");

            return stringBuilder.ToString();
        }
        static string SubjectToString(List<Subject> list, Dictionary<int, Subject> syllabus)
        {
            StringBuilder stringBuilder = new StringBuilder("(");
            foreach (Subject item in list)
            {
                stringBuilder.Append(item.name).Append(", ");
            }
            stringBuilder.Append(")");

            return stringBuilder.ToString();
        }
        static string SubjectToString<T>(T obj, Dictionary<int, Subject> syllabus)
        {
            if(obj is null)
            {
                return "";
            }
            return obj.ToString();
        }
    }
}

