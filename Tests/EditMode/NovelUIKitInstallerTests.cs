using System.Linq;
using NovelUIKit.Effects;
using NovelUIKit.Effects.VertexModifiers;
using NovelUIKit.Runtime.DI;
using NovelUIKit.Runtime.Presenters;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using VContainer;

namespace NovelUIKit.Tests.EditMode
{
    public class NovelUIKitInstallerTests
    {
        [Test]
        public void Installer_Registers_ScopedServices()
        {
            var builder = new ContainerBuilder();
            var installer = new NovelUIKitInstaller();
            installer.Install(builder);
            using var container = builder.Build();

            var textObject = new GameObject("TMP_Text");
            var textComponent = textObject.AddComponent<TextMeshPro>();

            try
            {
                using var scope = container.CreateScope(scopeBuilder =>
                {
                    scopeBuilder.RegisterInstance<TMP_Text>(textComponent);
                });

                var presenter = scope.Resolve<ITextPresenter>();
                var glitchController = scope.Resolve<IGlitchEffectController>();

                Assert.IsNotNull(presenter);
                Assert.IsNotNull(glitchController);
            }
            finally
            {
                Object.DestroyImmediate(textObject);
            }
        }

        [Test]
        public void Installer_Registers_VertexModifiers()
        {
            var builder = new ContainerBuilder();
            var installer = new NovelUIKitInstaller();
            installer.Install(builder);
            using var container = builder.Build();

            using var scope = container.CreateScope();
            var modifiers = scope.Resolve<IVertexModifier[]>().Select(modifier => modifier.GetType()).ToList();

            Assert.Contains(typeof(GlyphCorruptionModifier), modifiers);
            Assert.Contains(typeof(ShakeModifier), modifiers);
            Assert.Contains(typeof(DistortionModifier), modifiers);
        }
    }
}
