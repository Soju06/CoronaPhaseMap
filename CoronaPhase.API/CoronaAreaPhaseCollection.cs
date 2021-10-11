using System.Collections.ObjectModel;

namespace CoronaPhase.API {
    /// <summary>
    /// 지역 코로나 단계 리스트
    /// </summary>
    public class CoronaAreaPhaseCollection : Collection<CoronaAreaPhase> {
        internal CoronaAreaPhaseCollection(IEnumerable<CoronaAreaPhase> areaPhases) {
            foreach (var item in areaPhases) Add(item);
        }
    }
}
