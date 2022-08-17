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
        /// 推奨の時間割
        /// </summary>
        public List<int>[][][] recommendTimeTable;

        public Dictionary<int, Subject> Subjects { get; set; }


        private void Awake()
        {
            // リスト初期化
            recommendTimeTable = new List<int>[Term.HALF_MAX][][];
            suggestTimeTable= new List<int>[Term.HALF_MAX][][];
            for (int i = 0; i < Term.HALF_MAX; i++)
            {
                recommendTimeTable[i] = new List<int>[Day.DAY_MAX][];
                suggestTimeTable[i] = new List<int>[Day.DAY_MAX][];
                for (int j = 0; j < Day.DAY_MAX; j++)
                {
                    recommendTimeTable[i][j] = new List<int>[TimeTable.TIME_MAX];
                    suggestTimeTable[i][j] = new List<int>[TimeTable.TIME_MAX];
                    for (int k = 0; k < TimeTable.TIME_MAX; k++)
                    {
                        recommendTimeTable[i][j][k] = new List<int>();
                    }
                }
            }
        }

        private IEnumerator Start() {
            // 全体の時間割と辞書のロード
            yield return StartCoroutine(LoadXml());

            yield break;
        }

        public void suggest()
        {
            CreateSuggest(this.department, this.grade);
            printTimeTable();
        }

        public void printTimeTable()
        {
            TimeTablePrinter.printTimeTable(recommendTimeTable, "recommendTimeTable", Subjects);
            TimeTablePrinter.printTimeTable(timeTable, "timeTable", Subjects);
        }

        public IEnumerator LoadXml()
        {
            yield return StartCoroutine(
                TimeTableExporter.Import<Dictionary<int, Subject>>(
                    Application.streamingAssetsPath + "/xml/Syllabus.xml",
                    (result) => Subjects = result
                )
            );
            yield return StartCoroutine(
                TimeTableExporter.Import<List<int>[][][]>(
                    Application.streamingAssetsPath + "/xml/TimeTable.xml",
                    (result) => timeTable = result
                )
            );
        }

        public void CreateSuggest(string department, int grade)
        {
            // 推奨時間割を作成
            foreach ((int id, Subject subject) in Subjects)
            {
                // 学年一致
                if (subject.grade == grade)
                {
                    bool isTargetSubject = false;
                    foreach (string element in subject.department)
                    {
                        // 対象科目
                        if (department.Contains(element))
                        {
                            isTargetSubject = true;
                        }
                    }
                    // 対象科目なら推奨時間割に追加
                    if(isTargetSubject)
                    {
                        for (int i = subject.startTime - 1; i < subject.endTime; i++)
                        {
                            recommendTimeTable[subject.half][subject.day][i].Add(subject.id);
                        }
                    }

                }
            }

            // 提案時間割を作成
            for (int i = 0; i < Term.HALF_MAX; i++)
            {
                for (int j = 0; j < Day.DAY_MAX; j++)
                {
                    for (int k = 0; k < TimeTable.TIME_MAX; k++)
                    {
                        if(recommendTimeTable[i][j][k].Count != 0)
                        {
                            suggestTimeTable[i][j][k] = recommendTimeTable[i][j][k];
                        }
                        else
                        {
                            suggestTimeTable[i][j][k] = timeTable[i][j][k];
                        }
                    }
                }
            }
        }
    }
}
