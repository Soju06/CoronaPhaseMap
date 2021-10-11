namespace CoronaPhase.API {
    /// <summary>
    /// 코로나 지역 단계
    /// </summary>
    public class CoronaAreaPhase {

        /// <summary>
        /// 도시 이름 토큰
        /// </summary>
        public const string CityNameToken = "//";

        public CoronaAreaPhase(bool caution, byte phase, string content, string cityName, DateTime? startTime = null, DateTime? endTime = null) {
            Caution = caution;

            if ((Phase = phase) < 0) throw new IndexOutOfRangeException(
                "단계는 음수가 될 수 없습니다.");

            Content = content;
            if (string.IsNullOrWhiteSpace(CityName = cityName)) throw new FormatException(
                "도시 이름이 없습니다.");

            StartTime = startTime;
            EndTime = endTime;
        }

        /// <summary>
        /// 콘텐츠 경고
        /// </summary>
        public bool Caution { get; set; }

        /// <summary>
        /// 단계
        /// </summary>
        public byte Phase { get; set; }

        /// <summary>
        /// 콘텐츠
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 시작일
        /// </summary>
        public DateTime? StartTime { get; private set; }

        /// <summary>
        /// 종료일
        /// </summary>
        public DateTime? EndTime { get; private set; }

        /// <summary>
        /// 도시 이름
        /// </summary>
        public string CityName { get; private set; }
    }
}
