namespace CoronaPhase.API {
    /// <summary>
    /// 코로나 단계
    /// </summary>
    public class CoronaPhase {
        /// <summary>
        /// 데이터 시작점
        /// </summary>
        public const string DataStartToken = "RSS_DATA = [";
        /// <summary>
        /// 데이터 끝점
        /// </summary>
        public const string DataEndToken = "];";

        /// <summary>
        /// 전처리
        /// </summary>
        /// <param name="data">데이터</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="FormatException"></exception>
        internal CoronaPhase(string data) {
            int s = data.LastIndexOf(DataStartToken),
                e = data.LastIndexOf(DataEndToken);
            if (s < 0 || e < 0) throw new ArgumentOutOfRangeException(nameof(data), 
                "파싱 포멧이 올바르지 않습니다. (데이터 포인트가 올바르지 않음)");

            var pos = s + DataStartToken.Length;
            var pdata = data[pos..(e - 1)].Replace("\t", "").Trim();

            if (string.IsNullOrWhiteSpace(pdata)) throw new FormatException(
                "파싱 포멧이 올바르지 않습니다. (데이터가 올바르지 않음)");
            ParseData(pdata);
        }

        /// <summary>
        /// 후처리
        /// </summary>
        void ParseData(string data) {
            Console.WriteLine(data);
        }

        /// <summary>
        /// 코로나 단계를 가져옵니다.
        /// </summary>
        public static async Task<CoronaPhase> GetAsync() => await GetAsync(default);
        /// <summary>
        /// 코로나 단계를 가져옵니다.
        /// </summary>
        /// <param name="token">취소 토큰</param>
        public static async Task<CoronaPhase> GetAsync(CancellationToken token) {
            var http = Http;
            if (http == null) {
                http = Http ??= new();
                http.DefaultRequestHeaders.Add("User-Agent",  "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/94.0.4606.71 Safari/537.36 Edg/94.0.992.38");
            }

            using var res = await http.GetAsync("http://ncov.mohw.go.kr/regSocdisBoardView.do", token);
            res.EnsureSuccessStatusCode();

            var html = await res.Content.ReadAsStringAsync(token);
            if (string.IsNullOrWhiteSpace(html)) throw new FormatException();
            return new(html);
        }

        /// <summary>
        /// http
        /// </summary>
        public static HttpClient? Http { get; set; } 
    }
}