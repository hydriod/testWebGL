using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Suggest
{
    [System.Serializable]
    class DropdownArray
    {
        public TMP_Dropdown[] table;

        public DropdownArray() { }
        public DropdownArray(TMP_Dropdown[] value)
        {
            table = value;
        }

        public TMP_Dropdown this[int index]
        {
            get => this.table[index];
            set => this.table[index] = value;
        }
        public static implicit operator DropdownArray(TMP_Dropdown[] value) => new DropdownArray(value);
        public static implicit operator TMP_Dropdown[](DropdownArray value) => value;
    }

    public class DrawTimeTable : MonoBehaviour
    {
        public float wide = 900;
        public float height = 470;
        public Dropdown half;
        [SerializeField] GameObject uiPrefab;
        [SerializeField] Suggest suggest;
        [SerializeField] DropdownArray[] table;

        private void Awake()
        {
            table = new DropdownArray[Day.DAY_MAX - 1];
            table[0] = new TMP_Dropdown[TimeTable.TIME_MAX];

        }

        // Start is called before the first frame update
        void Start()
        {
            Rect r = GetComponent<RectTransform>().rect;
            wide = r.width;
            height = r.height;

            for (int i = 0; i < Day.DAY_MAX - 1; i++)
            {
                for (int j = 0; j < TimeTable.TIME_MAX - 1; j++)
                {
                    GameObject go = Instantiate(uiPrefab, transform);

                    float xMin, xMax, yMin, yMax;
                    xMin = i * (1f / (Day.DAY_MAX - 1));
                    xMax = (i + 1) * (1f / (Day.DAY_MAX - 1));
                    yMin = 1f - (j + 1) * (1f / (TimeTable.TIME_MAX - 1));
                    yMax = 1f - j * (1f / (TimeTable.TIME_MAX - 1));

                    var rect = go.GetComponent<RectTransform>();
                    rect.anchorMin = new Vector2(xMin, yMin);
                    rect.anchorMax = new Vector2(xMax, yMax);
                    rect.pivot = new Vector2(0, 1);
                    //rect.sizeDelta = new Vector2(180, 60);

                    Debug.Log(rect.anchorMin);
                    Debug.Log(new Vector2(xMin, yMin));

                    table[i][j] = go.GetComponent<TMP_Dropdown>();

                    //if (1 < j) { break; }
                }
                //if (1 < i) { break; }
            }

        }

        public void Draw()
        {
            suggest.suggest();
            for (int i = 0; i < table.Length; i++)
            {
                for (int j = 0; j < table[0].table.Length; j++)
                {
                    table[i][j].options = convert(suggest.suggestTimeTable[half.value][i][j]);
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

            stringBuilder.Append(suggest.Subjects[id].name);
            stringBuilder.Append(" (").Append(suggest.Subjects[id].timeTableId).Append(")");

            return new TMP_Dropdown.OptionData(stringBuilder.ToString());
        }
    }

}
