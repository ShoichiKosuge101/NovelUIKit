# PhotoStack UI

## セットアップ手順 (Canvas に置く)
1. Canvas 配下に空の GameObject を作成し `PhotoStackView` を追加します。
2. `PhotoStackView` 直下に以下の階層を作成します。
   - `Background` (任意 Image)
   - `CharacterFull` (Image)
   - `CardsRoot` (RectTransform)
3. `CardsRoot` の子として `Card` を作成し、以下の構造にします。
   - `Card` (RectTransform + `PhotoCardView`)
     - `MaskShape` (Image + Mask)
       - `CharacterInPhoto` (Image)
     - `Frame` (Image)
     - `Shadow` (Image)
4. `PhotoCardView` の参照を埋めます。
   - `Card Root` → `Card` の RectTransform
   - `Mask Shape Image` → `MaskShape`
   - `Mask` → `MaskShape` の Mask
   - `Character In Photo Image` → `CharacterInPhoto`
   - `Character In Photo Rect` → `CharacterInPhoto` の RectTransform
   - `Frame Image` → `Frame`
   - `Shadow Image` → `Shadow`
5. `PhotoStackView` の参照を埋めます。
   - `Character Full Image` → `CharacterFull`
   - `Cards Root` → `CardsRoot`
   - `Card Prefab` → 必要なら `Card` を Prefab 化して設定
   - `Card Views` → 既存の `Card` をリストに入れる (Prefab を使う場合は空でもOK)

## Sprite の割り当て
- `CharacterFull` にキャラの Sprite を設定します（はみ出し用）。
- `MaskShape` は写真の切り抜き形状 (長方形でもOK)。
- `Frame` は写真枠の Sprite。
- `Shadow` は影の Sprite。
- `CharacterInPhoto` は同じキャラ Sprite を使います（`SetCharacterSprite` が設定）。

## CardSpec 例
```csharp
var specs = new List<PhotoStackView.CardSpec>
{
    new PhotoStackView.CardSpec
    {
        position = new Vector2(-120f, 10f),
        rotationZ = -6f,
        scale = Vector2.one,
        maskSprite = maskSpriteA,
        frameSprite = frameSpriteA,
        shadowSprite = shadowSpriteA,
        characterOffset = new Vector2(0f, -20f)
    },
    new PhotoStackView.CardSpec
    {
        position = new Vector2(40f, -20f),
        rotationZ = 8f,
        scale = new Vector2(0.95f, 0.95f),
        maskSprite = maskSpriteB,
        frameSprite = frameSpriteB,
        shadowSprite = shadowSpriteB,
        characterOffset = new Vector2(10f, -10f)
    }
};
photoStackView.SetCharacterSprite(characterSprite);
photoStackView.SetCards(specs);
```

## 補足
- `CharacterFull` は `CardsRoot` より前に描画されるように、階層の順序を保ってください。
- `Mask` は `MaskShape` に設定し、`CharacterInPhoto` はその子にします。
