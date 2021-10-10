using CoronaPhaseMap.Services;
using Microsoft.JSInterop;

namespace CoronaPhaseMap.Services {
    /// <summary>
    /// 아침 토스트
    /// </summary>
    public class ToastService {
        IJSRuntime JS;

        public ToastService(IJSRuntime js) {
            JS = js;
            Current = this;
            Task.Factory.StartNew(async () => await JS.InvokeVoidAsync("toast.init",
                DotNetObjectReference.Create(this)));
        }

        /// <summary>
        /// 토스트를 띄웁니다
        /// </summary>
        public void Show(int time, params string[] messages) {
            if (messages == null || messages.Length <= 0) return;
            ShowEvent?.Invoke(this, new() { ShownTime = time, Messages = messages });
        }

        [JSInvokable]
        public void Toast(string message, double time = 1000) =>
            Show((int)time, message.Split('\n'));

        /// <summary>
        /// 토스트를 띄웁니다.
        /// </summary>
        public event EventHandler<ToastMessage> ShowEvent;

        public static ToastService? Current { get; private set; }
    }

    /// <summary>
    /// 토스트 메시지
    /// </summary>
    public struct ToastMessage {
        public int ShownTime { get; set; }
        public string[] Messages { get; set; }
    }

}

public static class StaticToast {
    public static void Toast(this string messages, int shownTime = 1000) {
        ToastService.Current?.Show(shownTime, messages.Split('\n'));
    }
}
