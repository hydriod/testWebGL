
namespace Suggest
{
    /// <summary>
    /// 期間
    /// </summary>
    public static class Term
    {
        /// <summary>
        /// 前期
        /// </summary>
        public const int Early = 0;
        /// <summary>
        /// 後期
        /// </summary>
        public const int Late = 1;
        public const int Other = 2;
        public const int HALF_MAX = 3;

        public static int ToTerm(string s)
        {
            int half;
            switch (s)
            {
                case "前":
                    half = Term.Early;
                    break;
                case "後":
                    half = Term.Late;
                    break;
                default:
                    half = Term.Other;
                    break;
            }

            return half;
        }

    }
}
