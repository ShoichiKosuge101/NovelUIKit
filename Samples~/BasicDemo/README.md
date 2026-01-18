# Basic Demo

NovelUIKit の主要機能を順番に確認できるデモです。`BasicDemoController` が自動再生し、画面タップで次のデモへ進みます。

## セットアップ手順（Unity）

1. 新規シーンを作成します。
2. `Canvas` と `EventSystem` を作成します。
3. `Canvas` 配下に `TextMeshPro - Text` を作成し、任意のフォントとレイアウトに調整します。
4. メインカメラに `ScreenNoiseEffect` コンポーネントを追加します。
5. 空の GameObject を作成し、`DemoLifetimeScope` を追加します。
6. さらに空の GameObject を作成し、`BasicDemoController` を追加します。
7. `BasicDemoController` の `Demo Text` に手順 3 で作成した TMP テキストを割り当てます。
8. `BasicDemoController` の `Screen Noise Effect` に手順 4 のコンポーネントを割り当てます。
9. `DemoLifetimeScope` の `Demo Text` に同じ TMP テキストを割り当てます。

## 使い方

- 再生するとタイトルが表示されます。
- 画面タップ（マウスクリック）で次のデモへ進みます。
- 文字送り中にタップするとスキップされます。

## デモ内容

1. タイトル表示
2. 通常の文字送りデモ
3. Shake エフェクトデモ
4. Glitch エフェクトデモ
5. ScreenNoise エフェクトデモ
6. ルビ表示デモ（`<ruby><rt>` を括弧表記に変換して表示）
7. 完了表示
