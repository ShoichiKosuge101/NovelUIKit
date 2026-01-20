using NovelUIKit.Effects;
using NovelUIKit.Effects.VertexModifiers;
using NovelUIKit.Runtime.Presenters;
using VContainer;
using VContainer.Unity;

namespace NovelUIKit.Runtime.DI
{
    /// <summary>
    /// NovelUIKitのDI登録を行うInstaller
    ///
    /// 【設計原則】
    /// - このInstallerはサブモジュールとして使用されることを前提とする
    /// - ILoggerFactory等のインフラストラクチャは登録しない（メインプロジェクトが提供）
    /// - NovelUIKit固有のサービスのみを登録する
    /// </summary>
    public sealed class NovelUIKitInstaller : IInstaller
    {
        public void Install(IContainerBuilder builder)
        {
            // === Presenters ===
            // FactoryパターンでTextPresenterを生成
            // ILoggerFactoryはメインプロジェクトから注入される前提
            builder.Register<ITextPresenterFactory, TextPresenterFactory>(Lifetime.Singleton);

            // === Effects ===
            builder.Register<GlitchEffectController>(Lifetime.Scoped)
                .As<IGlitchEffectController>();

            // === VertexModifiers ===
            builder.Register<GlyphCorruptionModifier>(Lifetime.Transient)
                .As<IVertexModifier>();
            builder.Register<ShakeModifier>(Lifetime.Transient)
                .As<IVertexModifier>();
            builder.Register<DistortionModifier>(Lifetime.Transient)
                .As<IVertexModifier>();
        }
    }
}
