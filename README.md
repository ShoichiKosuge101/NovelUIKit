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

public sealed class SampleUsage : MonoBehaviour
{
    [SerializeField] private TMP_Text text;

    private ITextPresenter presenter;

    private void Awake()
    {
        presenter = new TextPresenter(text);
    }

    private async UniTaskVoid Start()
    {
        var options = TextPresenterOptions.Default;
        await presenter.PresentAsync("Hello, NovelUIKit!", options, this.GetCancellationTokenOnDestroy());
    }
}
```
