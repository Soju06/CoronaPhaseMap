using System.Text.RegularExpressions;

namespace CoronaPhase.API {
    /// <summary>
    /// 코로나 단계
    /// </summary>
    public class CoronaPhase {
        /// <summary>
        /// 데이터 정규식
        /// </summary>
        public static readonly Regex Regex = new(
@".*{\s{0,}caution\s{0,}:\s{0,}'([Y|N]).*value\s{0,}:\s{0,}'(\d).*[DES|des|description].*:[ ]{0,}'(?:-[ ]?)?(.*)\(([\d]{2,4})\.([\d]{1,2})\.([\d]{1,2})\s{0,}~\s{0,}(?:([\d]{2,4}).)?([\d]{1,2}).([\d]{1,2})\.\)'.*}.*\/{2}\s{0,}([ㄱ-ㅎ|가-힣|A-z]{1,})",
RegexOptions.Compiled);
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
            var matchs = Regex.Matches(data);

        }

        IEnumerable<CoronaAreaPhase> GetAreaPhases(Match[] matches) {
            foreach (var matche in matches) {
                if (!matche.Success) continue;
                if (matche.Groups.Count != 11) throw new FormatException("정규식이 올바르지 않습니다.");
                bool caution = matche.Groups[1].Value == "Y";
                byte phase = byte.Parse(matche.Groups[2].Value);
                string content = matche.Groups[3].Value.Trim();
                string cityName;
                DateTime? startTime = null, endTime = null;
                yield return new CoronaAreaPhase(caution, phase, 
                    matche.Groups[3].Value,
                    matche.Groups[11].Value
                );
            }
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