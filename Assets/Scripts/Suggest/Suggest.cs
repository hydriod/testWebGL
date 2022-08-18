using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Suggest
{
    public class Suggest : MonoBehaviour
    {
        //public TMP_Dropdown departmentDropdown;
        //public TMP_Dropdown gradeDropdown;
        [SerializeField] int half;
        [SerializeField] string department;
        [SerializeField] int grade;
        //public string xmlPath;
        /// <summary>
        /// 全体の時間割
        /// </summary>
        public List<int>[][][] timeTable;
        /// <summary>
        /// 提案時間割
        /// </summary>
        public List<int>[][] suggestTimeTable;
        /// <summary>
        /// 推奨の時間割
        /// </summary>
        public List<int>[][] recommendTimeTable;
        public Dictionary<int, Subject> Subjects { get; set; }


        private void Awake()
        {
            // リスト初期化
            /*
            recommendTimeTable = new List<int>[Term.HALF_MAX][][];
            suggestTimeTable = new List<int>[Term.HALF_MAX][][];
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
            */
            recommendTimeTable = new List<int>[Day.DAY_MAX][];
            suggestTimeTable = new List<int>[Day.DAY_MAX][];
            for (int i = 0; i < Day.DAY_MAX; i++)
            {
                recommendTimeTable[i] = new List<int>[TimeTable.TIME_MAX];
                suggestTimeTable[i] = new List<int>[TimeTable.TIME_MAX];
                for (int j = 0; j < TimeTable.TIME_MAX; j++)
                {
                    recommendTimeTable[i][j] = new List<int>();
                }
            }

        }

        private IEnumerator Start()
        {
            //department = departmentDropdown.options[departmentDropdown.value].text;
            //grade = gradeDropdown.value;

            // 全体の時間割と辞書のロード
            yield return StartCoroutine(LoadXml());

            yield break;
        }

        public void printTimeTable()
        {
            TimeTablePrinter.printTimeTable(new List<int>[][][] { recommendTimeTable }, "recommendTimeTable", Subjects);
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
            Debug.Log("Load end");
        }

        public void CreateSuggest(int half, string department, int grade)
        {
            // 全体の時間割から対象科目の時間割を作成
            // 曜日を全走査
            for (int i = 0; i < timeTable[half].Length; i++)
            {
                // 時間を全走査
                for (int j = 0; j < timeTable[half][0].Length; j++)
                {
                    // 科目
                    foreach (int id in timeTable[half][i][j])
                    {
                        // 学年一致
                        if (Subjects[id].grade == grade)
                        {
                            bool isTargetSubject = false;
                            foreach (string element in Subjects[id].department)
                            {
                                //Debug.Log($"{element} in {department}");
                                // 対象科目
                                if (department.Contains(element))
                                {
                                    isTargetSubject = true;
                                }
                            }
                            // 対象科目なら推奨時間割に追加
                            if (isTargetSubject)
                            {
                                recommendTimeTable[i][j].Add(id);
                            }

                        }
                    }
                }
            }


            // 提案時間割を作成
            for (int i = 0; i < Day.DAY_MAX; i++)
            {
                for (int j = 0; j < TimeTable.TIME_MAX; j++)
                {
                    // 対象科目があればこれを提案
                    if (recommendTimeTable[i][j].Count != 0)
                    {
                        suggestTimeTable[i][j] = recommendTimeTable[i][j];
                    }
                    else // 無ければ全体のものを使用
                    {
                        suggestTimeTable[i][j] = timeTable[half][i][j];
                    }

                }
            }
        }
        public void CreateSuggest()
        {
            CreateSuggest(this.half, this.department, this.grade);
            printTimeTable();
        }

        public void ChangedDepartmentDropdown(TMP_Dropdown dropdown)
        {
            department = dropdown.options[dropdown.value].text;
        }
        public void ChangedGradeDropdown(TMP_Dropdown dropdown)
        {
            grade = dropdown.value;
        }
        public void ChangedHalfDropdown(TMP_Dropdown dropdown)
        {
            half = dropdown.value;
        }
    }
}
