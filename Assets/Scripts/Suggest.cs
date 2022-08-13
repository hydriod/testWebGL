using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;

namespace Suggest
{
    /// <summary>
    /// 学年
    /// </summary>
    enum Grade
    {
        B1,
        B2,
        B3,
        B4,
        M1,
        M2
    }
    /// <summary>
    /// 期間
    /// </summary>
    enum Term
    {
        /// <summary>
        /// 前期
        /// </summary>
        Early,
        /// <summary>
        /// 後期
        /// </summary>
        Late

    }

    public class Subject
    {
        /// <summary>
        /// ファイル番号
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// 科目名前
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 時間割番号
        /// </summary>
        public int timeTableId { get; set; }
        /// <summary>
        /// 担当教員
        /// </summary>
        public string teacher { get; set; }
        public string[] department { get; set; }
        public int grade { get; set; }
        /// <summary>
        /// 科目区分
        /// </summary>
        public int creditsNumber { get; set; }

        public override string ToString()
        {
            return this.name;
        }
    }

    public class Suggest : MonoBehaviour
    {
        public string department;
        public int grade;
        public List<Subject>[][][] suggestTimeTable;
        public Subject[][][] gradeTimeTable;

        public const int Japanese = 0;
        public const int English = 1;
        public const int HALF = 2;
        public const int DAY = 5;
        public const int TIME = 12;

        private void Awake()
        {
            // リスト初期化
            suggestTimeTable = new List<Subject>[HALF][][];
            for (int i = 0; i < HALF; i++)
            {
                suggestTimeTable[i] = new List<Subject>[DAY][];
                for (int j = 0; j < DAY; j++)
                {
                    suggestTimeTable[i][j] = new List<Subject>[TIME];
                    for (int k = 0; k < TIME; k++)
                    {
                        suggestTimeTable[i][j][k] = new List<Subject>();
                    }
                }
            }

            // 学年時間割の初期化
            Subject empty = new Subject();
            gradeTimeTable = new Subject[HALF][][];
            for (int i = 0; i < HALF; i++)
            {
                gradeTimeTable[i] = new Subject[DAY][];
                for (int j = 0; j < DAY; j++)
                {
                    gradeTimeTable[i][j] = new Subject[TIME];
                    for (int k = 0; k < TIME; k++)
                    {
                        gradeTimeTable[i][j][k] = empty;
                    }
                }
            }
        }

        public void printTimeTable()
        {
            StringBuilder gTableLog = new StringBuilder();
            foreach (Subject[] row in gradeTimeTable[0])
            {
                gTableLog.Append("[");
                foreach (Subject e in row)
                {
                    gTableLog.Append(e);
                    gTableLog.Append(", ");
                }
                gTableLog.Append("]\n");
            }
            Debug.Log(gTableLog.ToString());

            StringBuilder sTableLog = new StringBuilder();
            foreach (List<Subject>[] row in suggestTimeTable[0])
            {
                sTableLog.Append("[");
                foreach (List<Subject> e in row)
                {
                    sTableLog.Append("(");
                    sTableLog.Append(string.Join(", ", e));
                    sTableLog.Append("), ");
                }
                sTableLog.Append("]\n");
            }
            Debug.Log(sTableLog.ToString());
        }



        public void startSuggest()
        {
            StartCoroutine(suggest(department, grade));
        }

        IEnumerator suggest(string department, int grade)
        {
            // phpのリストをロード
            string[] fileNames = new string[0];
            yield return StartCoroutine(LoadSyllabus((result) => fileNames = result));

            // 
            yield return StartCoroutine(AddList(fileNames, department, grade));

            Debug.LogWarning("end");

            yield break;
        }

        IEnumerator LoadSyllabus(UnityEngine.Events.UnityAction<string[]> callback)
        {
            string url = Application.streamingAssetsPath + "/download/";
#if UNITY_EDITOR
            // Unityエディタ上での処理
            DirectoryInfo dir = new DirectoryInfo(url);
            FileInfo[] info = dir.GetFiles("*.php");
            string[] fName = new string[info.Length];
            for (int i = 0; i < info.Length; i++)
            {
                fName[i] = info[i].Name;
            }
#else // ブラウザ用の処理
            UnityWebRequest request = UnityWebRequest.Get(url + "list.txt");
            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.ConnectionError)
            {
                // エラー処理
                Debug.Log(request.error);
                yield break;
            }
            string content = request.downloadHandler.text;
            string[] fName = content.Split(System.Environment.NewLine);
#endif
            callback(fName);

            yield break;
        }

        IEnumerator AddList(string[] fileNames, string department, int grade)
        {

            foreach (string f in fileNames)
            {
                //Debug.Log(f);

                string filepath = Application.streamingAssetsPath + "/download/" + f;
                Debug.Log(filepath);

                string fileContent = "";
                yield return StartCoroutine(LoadPhpFile(filepath, (result) => fileContent = result));

                //正規表現パターンとオプションを指定してRegexオブジェクトを作成
                Regex r = new Regex(
                    @"<(div).+?>(.*?)</\1>",
                    RegexOptions.IgnoreCase | RegexOptions.Singleline
                );
                //TextBox1.Text内で正規表現と一致する対象をすべて検索
                MatchCollection mc = r.Matches(fileContent);
                Debug.Log($"( {mc.Count} )");

                // 科目のデータを抽出
                Subject subject = ExtractSubject(f, mc);

                // 時間割の位置を抽出
                int half, day, startTime, endTime;
                (half, day, (startTime, endTime)) = ExtractTimeTable(mc);
                if (half == -1)
                {
                    continue;
                }

                // 時間割の配列,提案リストに入力
                bool departmentFlag = false;
                foreach (string e in subject.department)
                {
                    if (department.Contains(e))
                    {
                        departmentFlag = true;
                    }
                }
                if (grade == subject.grade && departmentFlag)
                {
                    for (int i = startTime - 1; i < endTime; i++)
                    {
                        gradeTimeTable[half][day][i] = subject;
                    }

                }
                else
                {
                    for (int i = startTime - 1; i < endTime; i++)
                    {
                        suggestTimeTable[half][day][i].Add(subject);
                    }
                }

                yield return null;
            }
        }

        IEnumerator LoadPhpFile(string filePath, UnityEngine.Events.UnityAction<string> callback)
        {
            string fileContent;
#if UNITY_EDITOR
            fileContent = File.ReadAllText(filePath);

#else
            UnityWebRequest request = new UnityWebRequest(filePath);
            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.ConnectionError)
            {
                // エラー処理
                Debug.Log(request.error);
                yield break;
            }
            fileContent = request.downloadHandler.text;

#endif

            // 結果をコールバックで呼び出し元に伝える
            callback(fileContent);

            yield break;
        }

        /// <summary>
        /// 科目の情報を抽出する
        /// </summary>
        /// <param name="fileName">phpファイルの名前</param>
        /// <param name="matchCollection">正規表現の結果</param>
        /// <returns>科目インスタンス</returns>
        Subject ExtractSubject(string fileName, MatchCollection matchCollection)
        {
            Subject subject = new Subject();
            subject.id = int.Parse(fileName.Replace(".php", ""));
            subject.name = matchCollection[1].Groups[2].Value.Replace("&nbsp;", "").Split("<br>")[Japanese];
            subject.teacher = matchCollection[5].Groups[2].Value.Replace("&nbsp;", "");
            subject.timeTableId = int.Parse(matchCollection[3].Groups[2].Value);

            string departmentGrade = matchCollection[9].Groups[2].Value.Replace("&nbsp;", "");
            //subject.grade = int.Parse(departmentGrade.Split(" ")[1][0].ToString());
            Regex r = new Regex(@"(.+?)(\d)年次");
            Match match = r.Match(departmentGrade);
            subject.department = match.Groups[0].Value.Split("<BR> ");
            Debug.Log(match.Groups.Count);
            Debug.Log(string.Join(", ", match.Groups));
            subject.grade = int.Parse(match.Groups[2].Value);

            subject.creditsNumber = int.Parse(matchCollection[13].Groups[2].Value);

            StringBuilder log = new StringBuilder();
            log.Append("name:").Append(subject.name).Append("\n");
            log.Append("teacher:").Append(subject.teacher).Append("\n");
            log.Append("creditsNum:").Append(subject.creditsNumber).Append("\n");
            log.Append("timeTableId:").Append(subject.timeTableId).Append("\n");
            log.Append("id:").Append(subject.id).Append("\n");
            Debug.Log(log.ToString());

            return subject;
        }

        (int half, int day, (int startTime, int endTime)) ExtractTimeTable(MatchCollection matchCollection)
        {
            string timeTablePosition = matchCollection[15].Groups[2].Value.Replace("&nbsp;", "");

            // 前期後期
            int half;
            switch (timeTablePosition[0])
            {
                case '前':
                    half = 0;
                    break;
                case '後':
                    half = 1;
                    break;
                default:
                    Debug.LogError($"{timeTablePosition}[0] = {timeTablePosition[0]}");
                    return (-1, -1, (-1, -1));
            }
            // 曜日
            int day;
            switch (timeTablePosition[3])
            {
                case '月':
                    day = 0;
                    break;
                case '火':
                    day = 1;
                    break;
                case '水':
                    day = 2;
                    break;
                case '木':
                    day = 3;
                    break;
                case '金':
                    day = 4;
                    break;
                default:
                    Debug.LogError($"{timeTablePosition}[3] = {timeTablePosition[3]}");
                    return (-1, -1, (-1, -1));
            }
            // 時間
            Regex r = new Regex(@"(\d)-(\d)");
            Match match = r.Match(timeTablePosition);
            int startTime = int.Parse(match.Groups[1].Value);
            int endTime = int.Parse(match.Groups[2].Value);

            return (half, day, (startTime, endTime));
        }
    }
}


