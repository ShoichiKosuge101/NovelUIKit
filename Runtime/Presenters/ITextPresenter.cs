using System.Threading;
using Cysharp.Threading.Tasks;

namespace NovelUIKit.Runtime.Presenters
{
    public interface ITextPresenter
    {
        bool IsRunning { get; }
        bool IsSkipRequested { get; }

        UniTask PresentAsync(string text, TextPresenterOptions options, CancellationToken cancellationToken);
        void RequestSkip();
        void Reset();
    }
}
