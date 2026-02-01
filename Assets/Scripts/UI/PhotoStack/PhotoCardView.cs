using UnityEngine;
using UnityEngine.UI;

namespace NovelUIKit.UI.PhotoStack
{
    public class PhotoCardView : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private RectTransform cardRoot;
        [SerializeField] private Image maskShapeImage;
        [SerializeField] private Mask mask;
        [SerializeField] private Image characterInPhotoImage;
        [SerializeField] private RectTransform characterInPhotoRect;
        [SerializeField] private Image frameImage;
        [SerializeField] private Image shadowImage;

        public void ApplyCharacterSprite(Sprite character)
        {
            if (characterInPhotoImage != null)
            {
                characterInPhotoImage.sprite = character;
                characterInPhotoImage.enabled = character != null;
            }
        }

        public void ApplySpec(PhotoStackView.CardSpec spec)
        {
            if (cardRoot != null)
            {
                cardRoot.anchoredPosition = spec.position;
                cardRoot.localRotation = Quaternion.Euler(0f, 0f, spec.rotationZ);
                cardRoot.localScale = new Vector3(spec.scale.x, spec.scale.y, 1f);
            }

            if (maskShapeImage != null)
            {
                if (spec.maskSprite != null)
                {
                    maskShapeImage.sprite = spec.maskSprite;
                }

                maskShapeImage.enabled = true;
            }

            if (mask != null)
            {
                mask.showMaskGraphic = true;
            }

            if (frameImage != null)
            {
                frameImage.sprite = spec.frameSprite;
                frameImage.enabled = spec.frameSprite != null;
            }

            if (shadowImage != null)
            {
                shadowImage.sprite = spec.shadowSprite;
                shadowImage.enabled = spec.shadowSprite != null;
            }

            if (characterInPhotoRect != null)
            {
                characterInPhotoRect.anchoredPosition = spec.characterOffset;
            }
        }
    }
}
