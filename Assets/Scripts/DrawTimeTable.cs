using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Suggest
{
    /// <summary>
    /// インスペクタ表示用クラス
    /// </summary>
    [System.Serializable]
    public class DropdownArray
    {
        public TMP_Dropdown[] table;
        public int Length { get => table.Length; }

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
        public static implicit operator TMP_Dropdown[](DropdownArray value) => value.table;
    }

    public class DrawTimeTable : MonoBehaviour
    {
        public float Width { get; private set; }
        public float Height { get; private set; }
        public RectTransform rectTransform { get; private set; }
        [SerializeField] GameObject uiPrefab;
        [SerializeField] Suggest suggest;
        [SerializeField] DropdownArray[] table;
        public DropdownArray[] Table { get; private set; }

        private void Awake()
        {
            // 配列の初期化
            Table = new DropdownArray[Day.DAY_MAX - 1];
            for (int day = 0; day < Table.Length; day++)
            {
                Table[day] = new TMP_Dropdown[TimeTable.TIME_MAX - 1];

            }

        }

        // Start is called before the first frame update
        void Start()
        {
            rectTransform = GetComponent<RectTransform>();
            UpdateWindowSize();

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

                    //Debug.Log(rect.anchorMin);
                    //Debug.Log(new Vector2(xMin, yMin));

                    Table[i][j] = go.GetComponent<TMP_Dropdown>();

                    //if (1 < j) { break; }
                }
                //if (1 < i) { break; }
            }

        }

        private void UpdateWindowSize()
        {
            Width = rectTransform.rect.width;
            Height = rectTransform.rect.height;
        }

        public void Draw()
        {
            suggest.CreateSuggest();
            for (int i = 0; i < Table.Length; i++)
            {
                for (int j = 0; j < Table[i].Length; j++)
                {
                    Table[i][j].options = convert(suggest.suggestTimeTable[i][j]);
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
