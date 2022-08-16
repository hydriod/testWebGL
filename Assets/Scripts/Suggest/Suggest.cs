using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace Suggest
{
    public class Suggest : MonoBehaviour
    {
        public string department;
        public int grade;
        public string xmlPath;
        /// <summary>
        /// 全体の時間割
        /// </summary>
        public List<int>[][][] timeTable;
        /// <summary>
        /// 提案時間割
        /// </summary>
        public List<int>[][][] suggestTimeTable;
        /// <summary>
        /// 学年対象の時間割
        /// </summary>
        public List<int>[][][] gradeTimeTable;
        /// <summary>
        /// 学科対象の時間割
        /// </summary>
        public int?[][][] departmentTimeTable;

        public Dictionary<int, Subject> Subjects { get; set; }

        //public const int Japanese = 0;
        //public const int English = 1;

        private void Awake()
        {
            // リスト初期化
            gradeTimeTable = new List<int>[Term.HALF_MAX][][];
            for (int i = 0; i < Term.HALF_MAX; i++)
            {
                gradeTimeTable[i] = new List<int>[Day.DAY_MAX][];
                for (int j = 0; j < Day.DAY_MAX; j++)
                {
                    gradeTimeTable[i][j] = new List<int>[TimeTable.TIME_MAX];
                    for (int k = 0; k < TimeTable.TIME_MAX; k++)
                    {
                        gradeTimeTable[i][j][k] = new List<int>();
                    }
                }
            }

            // 学年時間割の初期化
            int? empty = null;
            departmentTimeTable = new int?[Term.HALF_MAX][][];
            for (int i = 0; i < Term.HALF_MAX; i++)
            {
                departmentTimeTable[i] = new int?[Day.DAY_MAX][];
                for (int j = 0; j < Day.DAY_MAX; j++)
                {
                    departmentTimeTable[i][j] = new int?[TimeTable.TIME_MAX];
                    for (int k = 0; k < TimeTable.TIME_MAX; k++)
                    {
                        departmentTimeTable[i][j][k] = empty;
                    }
                }
            }

            // 科目辞書の初期化
            Subjects = new Dictionary<int, Subject>();
        }

        public void startSuggests()
        {
            StartCoroutine(suggest());
        }

        public IEnumerator suggest()
        {
            yield return StartCoroutine(LoadXml());
            CreateSuggest(this.department, this.grade);
            printTimeTable();
        }

        public void printTimeTable()
        {
            TimeTablePrinter.printTimeTable(departmentTimeTable, "departmentTimeTable", Subjects);
            TimeTablePrinter.printTimeTable(gradeTimeTable, "gradeTimeTable", Subjects);
            TimeTablePrinter.printTimeTable(timeTable, "timeTable", Subjects);
        }

        public IEnumerator LoadXml()
        {
            yield return StartCoroutine(TimeTableExporter.Import<Dictionary<int, Subject>>(Application.streamingAssetsPath + "/xml/Syllabus.xml", (result) => Subjects = result));
            yield return StartCoroutine(TimeTableExporter.Import<List<int>[][][]>(Application.streamingAssetsPath + "/xml/TimeTable.xml", (result) => timeTable = result));
        }

        public void CreateSuggest(string department, int grade)
        {
            foreach ((int id, Subject subject) in Subjects)
            {
                // 学年一致
                if (subject.grade == grade)
                {
                    foreach (string element in subject.department)
                    {
                        // 対象科目
                        if (department.Contains(element))
                        {
                            // 学科単位で対象
                            if (department == element)
                            {
                                for (int i = subject.startTime - 1; i < subject.endTime; i++)
                                {
                                    departmentTimeTable[subject.half][subject.day][i] = subject.id;
                                }
                            }
                            else // 学年単位で対象
                            {
                                for (int i = subject.startTime - 1; i < subject.endTime; i++)
                                {
                                    gradeTimeTable[subject.half][subject.day][i].Add(subject.id);
                                }
                            }
                        }
                    }

                }
            }
        }
    }
}
