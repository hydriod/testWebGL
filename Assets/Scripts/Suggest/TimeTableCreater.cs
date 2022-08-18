using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;

namespace Suggest
{
    public class TimeTableCreater : MonoBehaviour
    {
        public List<int>[][][] timeTable;

        public Dictionary<int, Subject> Subjects { get; set; }

        static class MatchIndex
        {
            // 科目名の言語
            public const int Japanese = 0;
            public const int English = 1;

            // phpのdivの中身
            public const int DIV_CONTENT = 2;

            // divの番号
            public const int SUBJECT_NAME = 1;
            public const int TIMETABLE_ID = 3;
            public const int SUBJECT_TEACHER = 5;
            public const int TARGET_GRADE_DEPARTMENT = 9;
            public const int CREDITS_NUMBER = 13;
            public const int TIMETABLE = 15;

            // 時間割のマッチ
            public const int TERM = 1;
            public const int DAY = 2;
            public const int START_TIME = 4;
            public const int END_TIME = 5;
        }

        private void Awake()
        {
            // リスト初期化
            timeTable = new List<int>[Term.HALF_MAX][][];
            for (int i = 0; i < Term.HALF_MAX; i++)
            {
                timeTable[i] = new List<int>[Day.DAY_MAX][];
                for (int j = 0; j < Day.DAY_MAX; j++)
                {
                    timeTable[i][j] = new List<int>[TimeTable.TIME_MAX];
                    for (int k = 0; k < TimeTable.TIME_MAX; k++)
                    {
                        timeTable[i][j][k] = new List<int>();
                    }
                }
            }

            // 科目辞書の初期化
            Subjects = new Dictionary<int, Subject>();
        }

        public void printTimeTable()
        {
            TimeTablePrinter.printTimeTable(timeTable, "AllTimeTable", Subjects);
        }

        public void startCreate()
        {
            StartCoroutine(CreateTimeTable());
        }
        public void Export()
        {
            TimeTableExporter.Export(timeTable, Application.streamingAssetsPath + "/xml/TimeTable.xml");
            TimeTableExporter.Export(Subjects, Application.streamingAssetsPath + "/xml/Syllabus.xml");
        }

        public IEnumerator CreateTimeTable()
        {
            // phpのリストをロード
            string[] fileNames = default(string[]);
            yield return StartCoroutine(LoadPhpList((result) => fileNames = result));

            // 時間割に追加
            yield return StartCoroutine(PhpToTimeTable(fileNames));

            Debug.LogWarning("end");

            yield break;
        }

        /// <summary>
        /// phpファイル名の配列をロード
        /// </summary>
        /// <param name="callback">結果を返すためのコールバック</param>
        /// <returns></returns>
        IEnumerator LoadPhpList(UnityEngine.Events.UnityAction<string[]> callback)
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

        IEnumerator PhpToTimeTable(string[] fileNames)
        {

            foreach (string f in fileNames)
            {
                //Debug.Log(f);

                string filepath = Application.streamingAssetsPath + "/download/" + f;
                Debug.Log(filepath);

                string fileContent = default(string);
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
                Subjects.Add(subject.id, subject);

                // 時間割の位置を抽出
                //int half, day, startTime, endTime;
                //(half, day, (startTime, endTime)) = ExtractTimeTable(mc);
                //if (half == -1)
                //{
                //    continue;
                //}


                for (int i = subject.startTime; i <= subject.endTime; i++)
                {
                    timeTable[subject.half][subject.day][i].Add(subject.id);
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
            // ダウンロード
            UnityWebRequest request = new UnityWebRequest(filePath);
            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.ConnectionError)
            {
                // エラー処理
                Debug.Log(request.error);
                yield break;
            }
            //　ロード
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
            subject.name = matchCollection[MatchIndex.SUBJECT_NAME].Groups[MatchIndex.DIV_CONTENT].Value.Replace("&nbsp;", "").Split("<br>")[MatchIndex.Japanese];
            subject.teacher = matchCollection[MatchIndex.SUBJECT_TEACHER].Groups[MatchIndex.DIV_CONTENT].Value.Replace("&nbsp;", "");
            subject.timeTableId = int.Parse(matchCollection[MatchIndex.TIMETABLE_ID].Groups[MatchIndex.DIV_CONTENT].Value);

            // 学科と学年を抽出
            string departmentGrade = matchCollection[MatchIndex.TARGET_GRADE_DEPARTMENT].Groups[MatchIndex.DIV_CONTENT].Value.Replace("&nbsp;", "").Replace("&nbsp", "");
            Regex r = new Regex(@"(.+?)(\d)年次");
            Match match = r.Match(departmentGrade);
            subject.department = match.Groups[1].Value.Replace("<br>", "").Split("<BR> ");
            Debug.Log(match.Groups.Count);
            Debug.Log(string.Join(", ", match.Groups));
            subject.grade = int.Parse(match.Groups[2].Value);
            // 単位数を抽出
            subject.creditsNumber = int.Parse(matchCollection[MatchIndex.CREDITS_NUMBER].Groups[MatchIndex.DIV_CONTENT].Value);

            // 時間割の位置を抽出
            (subject.half, subject.day, (subject.startTime, subject.endTime)) = ExtractTimeTable(matchCollection);

            // ログに出力
            Debug.Log(subject.ToStringLog());

            return subject;
        }

        (int half, int day, (int startTime, int endTime)) ExtractTimeTable(MatchCollection matchCollection)
        {
            string timeTablePosition = matchCollection[MatchIndex.TIMETABLE].Groups[MatchIndex.DIV_CONTENT].Value.Replace("&nbsp;", "");

            Regex r = new Regex(@"(\S+) (.曜|集中)((\d+)-(\d+)限)?");
            Match match = r.Match(timeTablePosition);

            bool logFlag = false;

            // 前期後期
            int half = Term.ToTerm(match.Groups[MatchIndex.TERM].Value);
            if (half == Term.Other)
            {
                logFlag = true;
            }
            // 曜日
            int day = Day.ToDay(match.Groups[MatchIndex.DAY].Value);
            if (day == Day.Other)
            {
                logFlag = true;
            }
            // 時間
            int startTime, endTime;
            try
            {
                startTime = int.Parse(match.Groups[MatchIndex.START_TIME].Value);
                endTime = int.Parse(match.Groups[MatchIndex.END_TIME].Value);
            }
            catch (System.FormatException)
            {
                startTime = TimeTable.TIME_OTHER;
                endTime = TimeTable.TIME_OTHER;
                logFlag = true;
            }

            // エラーログ
            if(logFlag)
            {
                StringBuilder log = new StringBuilder("match.Groups = [");
                for (int i = 0; i < match.Groups.Count; i++)
                {
                    log.Append("'").Append(match.Groups[i]).Append("', ");
                }
                log.Append("]");
                Debug.LogError(log.ToString());
            }

            return (half, day, (startTime, endTime));
        }
    }
}


