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
        public List<int>[] suggestTimeTable;
        /// <summary>
        /// 推奨の時間割
        /// </summary>
        public HashSet<int>[] recommendTimeTable;
        /// <summary>
        /// <para>シラバスの情報</para>
        /// <para>phpファイルの番号がキー</para>
        /// </summary>
        public Dictionary<int, Subject> Syllabus { get; set; }


        private void Awake()
        {
            // リスト初期化
            recommendTimeTable = new HashSet<int>[Day.DAY_MAX];
            suggestTimeTable = new List<int>[Day.DAY_MAX];
            for (int day = 0; day < Day.DAY_MAX; day++)
            {
                recommendTimeTable[day] = new HashSet<int>();
                suggestTimeTable[day] = new List<int>();
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
            //TimeTablePrinter.printTimeTable(new List<int>[][][] { recommendTimeTable }, "recommendTimeTable", Syllabus);
            TimeTablePrinter.printTimeTable(timeTable, "timeTable", Syllabus);
        }

        public IEnumerator LoadXml()
        {
            yield return StartCoroutine(
                TimeTableExporter.Import<Dictionary<int, Subject>>(
                    Application.streamingAssetsPath + "/xml/Syllabus.xml",
                    (result) => Syllabus = result
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
            bool[,] is_empty = new bool[5, 14];
            // 全体の時間割から対象科目の時間割を作成
            // 曜日を全走査
            for (int day = 0; day < timeTable[half].Length; day++)
            {
                // 時間を全走査
                for (int j = 0; j < timeTable[half][day].Length; j++)
                {
                    // 科目
                    foreach (int id in timeTable[half][day][j])
                    {
                        // 学年一致
                        if (Syllabus[id].grade == grade)
                        {
                            foreach (string element in Syllabus[id].department)
                            {
                                //Debug.Log($"{element} in {department}");
                                // 対象科目なら追加
                                if (department.Contains(element))
                                {
                                    recommendTimeTable[day].Add(id);
                                    if(is_empty[day, j])
                                    {
                                        suggestTimeTable[day].Add(id);
                                        for(int i=Syllabus[id].startTime;i<=Syllabus[id].endTime;i++)
                                        {
                                            is_empty[day, i] = false;
                                        }
                                    }
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }
        public void CreateSuggest()
        {
            CreateSuggest(this.half, this.department, this.grade);
            printTimeTable();
        }

        // 以下UIイベント用
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
