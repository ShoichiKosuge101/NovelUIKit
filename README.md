# NovelUIKit

Unity向けノベルゲームの演出重視テキスト表示システムです。UPM (Unity Package Manager) 形式で提供され、サブモジュール参照にも対応します。

## 依存パッケージ

- TextMeshPro
- UniTask
- UniRx
- VContainer
- ZLogger v2

## セットアップ

1. UPMの`manifest.json`に依存パッケージを追加します。
2. `NovelUIKit`をサブモジュールとして追加するか、UPMローカルパッケージとして参照します。

## 前提条件

このパッケージを使用するには、メインプロジェクトで以下が必要です：

1. **OpenUPM の scoped registry** を追加（UniRx の取得に必要）
1. **NuGetForUnity** をインストール
2. **ZLogger** をNuGet経由でインストール
3. C# 10を有効化するため `Assets/csc.rsp` に `-langVersion:10 -nullable` を記載

## 使用方法

### 1. 前提条件

メインプロジェクトで以下を事前に登録してください：

- `ILoggerFactory` (ZLogger, Microsoft.Extensions.Logging等)

### 2. Installer登録

```csharp
public class RootLifetimeScope : LifetimeScope
{
    protected override void Configure(IContainerBuilder builder)
    {
        // 1. インフラストラクチャを先に登録
        builder.RegisterInstance(loggerFactory).As<ILoggerFactory>();

        // 2. NovelUIKitを登録
        new NovelUIKitInstaller().Install(builder);
    }
}
```

### OpenUPM registry の設定例

Unity の `Packages/manifest.json` に以下を追加してください。

```json
{
  "scopedRegistries": [
    {
      "name": "OpenUPM",
      "url": "https://package.openupm.com",
      "scopes": [
        "com.neuecc",
        "com.cysharp"
      ]
    }
  ]
}
```

## 基本的な使い方

```csharp
using Cysharp.Threading.Tasks;
using NovelUIKit.Runtime.Presenters;
using TMPro;
using UnityEngine;
using VContainer;

public sealed class SampleUsage : MonoBehaviour
{
    [SerializeField] private TMP_Text text;

    [Inject] private ITextPresenterFactory presenterFactory;
    private ITextPresenter presenter;

    private void Awake()
    {
        presenter = presenterFactory.Create(text);
    }

    private async UniTaskVoid Start()
    {
        var options = TextPresenterOptions.Default;
        await presenter.PresentAsync("Hello, NovelUIKit!", options, this.GetCancellationTokenOnDestroy());
    }
}
```
