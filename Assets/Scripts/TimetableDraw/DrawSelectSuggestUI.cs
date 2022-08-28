using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Suggest;

namespace TimetableDraw
{
    public class DrawSelectSuggestUI : MonoBehaviour
    {
        public Transform[] scrollContents = new Transform[Day.DAY_MAX - 1];
        public GameObject subjectUi;
        [SerializeField] Suggester suggester;

        public void Draw()
        {
            for (int day = 0; day < Day.DAY_MAX - 1; day++)
            {
                foreach (Subject s in suggester.suggestTimeTable[day])
                {
                    GameObject go = Instantiate(subjectUi, scrollContents[day]);
                    go.GetComponent<SelectSuggestSubject>().Lecture = s;
                }

                // デバッグ
                break;
            }
        }

    }
}

