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
        public List<int>[][][] timeTable;
        public List<int>[][][] suggestTimeTable;
        public int?[][][] gradeTimeTable;

        public Dictionary<int, Subject> Subjects { get; set; }

        //public const int Japanese = 0;
        //public const int English = 1;

        private void Awake()
        {
            // リスト初期化
            suggestTimeTable = new List<int>[Term.HALF_MAX][][];
            for (int i = 0; i < Term.HALF_MAX; i++)
            {
                suggestTimeTable[i] = new List<int>[Day.DAY_MAX][];
                for (int j = 0; j < Day.DAY_MAX; j++)
                {
                    suggestTimeTable[i][j] = new List<int>[TimeTable.TIME_MAX];
                    for (int k = 0; k < TimeTable.TIME_MAX; k++)
                    {
                        suggestTimeTable[i][j][k] = new List<int>();
                    }
                }
            }

            // 学年時間割の初期化
            int? empty = null;
            gradeTimeTable = new int?[Term.HALF_MAX][][];
            for (int i = 0; i < Term.HALF_MAX; i++)
            {
                gradeTimeTable[i] = new int?[Day.DAY_MAX][];
                for (int j = 0; j < Day.DAY_MAX; j++)
                {
                    gradeTimeTable[i][j] = new int?[TimeTable.TIME_MAX];
                    for (int k = 0; k < TimeTable.TIME_MAX; k++)
                    {
                        gradeTimeTable[i][j][k] = empty;
                    }
                }
            }

            // 科目辞書の初期化
            Subjects = new Dictionary<int, Subject>();
        }

        public void suggest()
        {
            LoadXml();
            CreateSuggest(this.department, this.grade);
            printTimeTable();
        }

        public void printTimeTable()
        {
            TimeTablePrinter.printTimeTable(gradeTimeTable, "gradeTimeTable", Subjects);
            TimeTablePrinter.printTimeTable(suggestTimeTable, "suggestTimeTable", Subjects);
            TimeTablePrinter.printTimeTable(timeTable, "timeTable", Subjects);
        }

        public void LoadXml()
        {
            Subjects = TimeTableExporter.Import<Dictionary<int, Subject>>(Application.streamingAssetsPath + "/xml/Syllabus.xml");
            timeTable = TimeTableExporter.Import<List<int>[][][]>(Application.streamingAssetsPath + "/xml/TimeTable.xml");
        }

        public (int?[][][] departmentTimeTable, List<int>[][][] suggestTimeTable) CreateSuggest(string department, int grade)
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
                                    gradeTimeTable[subject.half][subject.day][i] = subject.id;
                                }
                            }
                            else // 学年単位で対象
                            {
                                for (int i = subject.startTime - 1; i < subject.endTime; i++)
                                {
                                    suggestTimeTable[subject.half][subject.day][i].Add(subject.id);
                                }
                            }
                        }
                    }

                }
            }
            return (this.gradeTimeTable, this.suggestTimeTable);
        }
    }
}
