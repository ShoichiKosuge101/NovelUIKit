using NovelUIKit.Runtime.DI;
using TMPro;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace NovelUIKit.Samples.BasicDemo
{
    public sealed class DemoLifetimeScope : LifetimeScope
    {
        [SerializeField] private TMP_Text demoText;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.Install(new NovelUIKitInstaller());

            if (demoText != null)
            {
                builder.RegisterInstance(demoText);
            }

            builder.RegisterComponentInHierarchy<BasicDemoController>();
        }
    }
}
