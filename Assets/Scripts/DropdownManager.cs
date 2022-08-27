using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Suggest;

namespace TimeTableUI
{    
    public class DropdownManager : MonoBehaviour {
        //DropdownArray[] timeTable;
        [SerializeField] Suggest.Suggest suggest;
        [SerializeField] TMP_Dropdown half;
        [SerializeField] DrawTimeTable drawer;

        private void Start() {
            //timeTable = drawer.Table;
        }

        /*
        public void Clear(TMP_Dropdown dropdown)
        {
            int id = suggest.recommendTimeTable[half.value][][][dropdown.value];
            Subject clearSubject = suggest.Subjects[id];

            for (int i = clearSubject.startTime; i <= clearSubject.endTime; i++)
            {
                
            }
        }
        */
        
    }
}