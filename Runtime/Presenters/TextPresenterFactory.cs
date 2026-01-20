using Microsoft.Extensions.Logging;
using TMPro;

namespace NovelUIKit.Runtime.Presenters
{
    public sealed class TextPresenterFactory : ITextPresenterFactory
    {
        private readonly ILoggerFactory _loggerFactory;

        public TextPresenterFactory(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
        }

        public ITextPresenter Create(TMP_Text textComponent)
        {
            return new TextPresenter(
                textComponent,
                _loggerFactory.CreateLogger<TextPresenter>());
        }
    }
}
