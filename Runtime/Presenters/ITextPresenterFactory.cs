using TMPro;

namespace NovelUIKit.Runtime.Presenters
{
    /// <summary>
    /// TextPresenterのファクトリインターフェース
    /// シーン上のTMP_Textからプレゼンターを生成する
    /// </summary>
    public interface ITextPresenterFactory
    {
        /// <summary>
        /// 指定されたTMP_Textに対するITextPresenterを生成
        /// </summary>
        ITextPresenter Create(TMP_Text textComponent);
    }
}
