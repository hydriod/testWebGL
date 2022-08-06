using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;

public class LoadText : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI debugText;
    private string result;

    void Start()
    {
        StartCoroutine (textLoad());
    }

    IEnumerator textLoad() 
    { 
        string filepath = Application.streamingAssetsPath + "/test.txt";
        if (filepath.Contains ("://") || filepath.Contains (":///"))
        {
            WWW www = new WWW (filepath);
            yield return www;
            result = www.text;
        } else {
            result = File.ReadAllText (filepath);
        }
        debugText.text = result;
    }
}
