using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Suggest
{
    /// <summary>
    /// UI 描画
    /// </summary>
    public class DrawTimeTable : MonoBehaviour
    {
        public RectTransform rectTransform { get; private set; }
        [SerializeField] GameObject[] dayFrame = new GameObject[Day.DAY_MAX - 1];
        [SerializeField] GameObject subjectUiPrefab;
        [SerializeField] Suggest suggest;

        private void Awake()
        {
            // 配列の初期化
            /*
            tablePanel = new HashSet<GameObject>[Day.DAY_MAX - 1];
            for (int day = 0; day < tablePanel.Length; day++)
            {
                tablePanel[day] = new HashSet<GameObject>();

            }
            */
        }

        // Start is called before the first frame update
        void Start()
        {
            rectTransform = GetComponent<RectTransform>();
        }

        public void Draw()
        {
            suggest.CreateSuggest();
            /*
            for (int day = 0; day < Day.DAY_MAX - 1; day++)
            {
                for (int j = 0; j < TimeTable.TIME_MAX - 1; j++)
                {
                    tablePanel[day] = convert(suggest.suggestTimeTable[day][j]);
                }
            }
            */

            for (int day = 0; day < Day.DAY_MAX - 1; day++)
            {
                foreach (Subject s in suggest.uiDrawTimeTable[day])
                {
                    GameObject go = Instantiate(subjectUiPrefab, dayFrame[day].transform);
                    RectTransform t = go.GetComponent<RectTransform>();
                    //t.anchorMax = 
                    break;
                }
            }
        }

        private List<TMP_Dropdown.OptionData> convert(IReadOnlyList<int> idList)
        {
            var options = new List<TMP_Dropdown.OptionData>();

            foreach (int id in idList)
            {
                options.Add(IdToOption(id));
            }

            return options;
        }
        private TMP_Dropdown.OptionData IdToOption(int id)
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.Append(suggest.Syllabus[id].name);
            stringBuilder.Append(" (").Append(suggest.Syllabus[id].timeTableId).Append(")");

            return new TMP_Dropdown.OptionData(stringBuilder.ToString());
        }
    }

}
