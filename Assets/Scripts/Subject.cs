using System.Runtime.Serialization;

namespace Suggest
{
    [DataContract]
    public class Subject
    {
        /// <summary>
        /// ファイル番号
        /// </summary>
        [DataMember]
        public int id { get; set; }
        /// <summary>
        /// 科目名前
        /// </summary>
        [DataMember]
        public string name { get; set; }
        /// <summary>
        /// 時間割番号
        /// </summary>
        [DataMember]
        public int timeTableId { get; set; }
        /// <summary>
        /// 担当教員
        /// </summary>
        [DataMember]
        public string teacher { get; set; }
        /// <summary>
        /// 対象学科
        /// </summary>
        [DataMember]
        public string[] department { get; set; }
        /// <summary>
        /// 学年
        /// </summary>
        [DataMember]
        public int grade { get; set; }
        /// <summary>
        /// 科目区分
        /// </summary>
        [DataMember]
        public int creditsNumber { get; set; }

        public override string ToString()
        {
            return this.name;
        }
    }
}
