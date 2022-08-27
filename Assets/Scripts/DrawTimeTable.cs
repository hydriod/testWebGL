using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Suggest
{
    public class DrawTimeTable : MonoBehaviour
    {
        public RectTransform rectTransform { get; private set; }
        [SerializeField] GameObject[] dayFrame = new GameObject[5];
        [SerializeField] GameObject uiPrefab;
        [SerializeField] Suggest suggest;
        [SerializeField] HashSet<GameObject>[] tablePanel;
        public IReadOnlyCollection<GameObject>[] TablePanel { get => tablePanel; }

        private void Awake()
        {
            // 配列の初期化
            tablePanel = new HashSet<GameObject>[Day.DAY_MAX - 1];
            for (int day = 0; day < tablePanel.Length; day++)
            {
                tablePanel[day] = new HashSet<GameObject>();

            }
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

            stringBuilder.Append(suggest.Subjects[id].name);
            stringBuilder.Append(" (").Append(suggest.Subjects[id].timeTableId).Append(")");

            return new TMP_Dropdown.OptionData(stringBuilder.ToString());
        }
    }

}
