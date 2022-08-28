using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Suggest;
using TMPro;

namespace TimetableDraw
{
    /// <summary>
    /// UI 描画
    /// </summary>
    public class DrawTimeTable : MonoBehaviour
    {
        public RectTransform rectTransform { get; private set; }
        [SerializeField] GameObject[] dayFrame = new GameObject[Day.DAY_MAX - 1];
        [SerializeField] GameObject subjectUiPrefab;
        [SerializeField] Suggester suggester;
        [SerializeField] List<GameObject>[] SubjectPanels;

        private void Awake()
        {
            // 配列の初期化
            SubjectPanels = new List<GameObject>[Day.DAY_MAX - 1];
            for (int day = 0; day < Day.DAY_MAX - 1; day++)
            {
                SubjectPanels[day] = new List<GameObject>();
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            rectTransform = GetComponent<RectTransform>();
        }

        /// <summary>
        /// 提案時間割のUIを描画
        /// </summary>
        public void Draw()
        {
            //suggester.CreateSuggest();

            for (int day = 0; day < Day.DAY_MAX - 1; day++)
            {
                foreach (Subject s in suggester.uiDrawTimeTable[day])
                {
                    GameObject go = Instantiate(subjectUiPrefab, dayFrame[day].transform);
                    // 位置調整
                    RectTransform t = go.GetComponent<RectTransform>();
                    t.anchorMax = new Vector2(1f, (14 - (s.startTime - 1)) / 14f);
                    t.anchorMin = new Vector2(0f, (14 - s.endTime) / 14f);
                    // 科目名を格納
                    go.GetComponentInChildren<TMP_Text>().text = s.name;
                    //
                    SubjectPanels[day].Add(go);
                    //break;
                }
            }
        }

        /// <summary>
        /// UIイベント用
        /// </summary>
        /// <param name="day">削除する曜日</param>
        public void ClearDayTimetable(int day)
        {
            foreach (GameObject go in SubjectPanels[day])
            {
                Destroy(go);
            }
        }
    }

}
