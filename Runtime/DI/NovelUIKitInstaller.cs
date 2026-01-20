using Microsoft.Extensions.Logging;
using NovelUIKit.Effects;
using NovelUIKit.Effects.VertexModifiers;
using NovelUIKit.Runtime.Presenters;
using VContainer;
using VContainer.Unity;

namespace NovelUIKit.Runtime.DI
{
    public sealed class NovelUIKitInstaller : IInstaller
    {
        public void Install(IContainerBuilder builder)
        {
            builder.RegisterInstance<ILoggerFactory>(LoggerFactory.Create(_ => { }));
            builder.Register<ITextPresenterFactory, TextPresenterFactory>(Lifetime.Singleton);
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
