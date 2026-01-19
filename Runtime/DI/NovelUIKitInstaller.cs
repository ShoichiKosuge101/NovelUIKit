using NovelUIKit.Effects;
using NovelUIKit.Effects.VertexModifiers;
using VContainer;
using VContainer.Unity;

namespace NovelUIKit.Runtime.DI
{
    public sealed class NovelUIKitInstaller : IInstaller
    {
        public void Install(IContainerBuilder builder)
        {
            builder.Register<NovelUIKit.Runtime.Presenters.TextPresenter>(Lifetime.Scoped)
                .As<NovelUIKit.Runtime.Presenters.ITextPresenter>();
            builder.Register<GlitchEffectController>(Lifetime.Scoped)
                .As<IGlitchEffectController>();

            builder.Register<GlyphCorruptionModifier>(Lifetime.Transient)
                .As<IVertexModifier>();
            builder.Register<ShakeModifier>(Lifetime.Transient)
                .As<IVertexModifier>();
            builder.Register<DistortionModifier>(Lifetime.Transient)
                .As<IVertexModifier>();
        }
    }
}
