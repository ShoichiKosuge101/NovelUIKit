using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace NovelUIKit.UI.PhotoStack
{
    public class PhotoStackView : MonoBehaviour
    {
        [Serializable]
        public struct CardSpec
        {
            public Vector2 position;
            public float rotationZ;
            public Vector2 scale;
            public Sprite maskSprite;
            public Sprite frameSprite;
            public Sprite shadowSprite;
            public Vector2 characterOffset;

            public static CardSpec Default
            {
                get
                {
                    return new CardSpec
                    {
                        position = Vector2.zero,
                        rotationZ = 0f,
                        scale = Vector2.one,
                        maskSprite = null,
                        frameSprite = null,
                        shadowSprite = null,
                        characterOffset = Vector2.zero
                    };
                }
            }
        }

        [Header("References")]
        [SerializeField] private Image characterFullImage;
        [SerializeField] private Transform cardsRoot;
        [SerializeField] private PhotoCardView cardPrefab;
        [SerializeField] private List<PhotoCardView> cardViews = new List<PhotoCardView>();

        public void SetCharacterSprite(Sprite character)
        {
            if (characterFullImage != null)
            {
                characterFullImage.sprite = character;
                characterFullImage.enabled = character != null;
            }

            foreach (var cardView in cardViews)
            {
                if (cardView != null)
                {
                    cardView.ApplyCharacterSprite(character);
                }
            }
        }

        public void SetCards(IReadOnlyList<CardSpec> specs)
        {
            if (specs == null)
            {
                return;
            }

            EnsureCardViews(specs.Count);

            for (int i = 0; i < cardViews.Count; i++)
            {
                var cardView = cardViews[i];
                if (cardView == null)
                {
                    continue;
                }

                bool active = i < specs.Count;
                cardView.gameObject.SetActive(active);

                if (!active)
                {
                    continue;
                }

                var spec = specs[i];
                if (spec.scale == Vector2.zero)
                {
                    spec.scale = Vector2.one;
                }

                cardView.ApplySpec(spec);
            }
        }

        private void EnsureCardViews(int count)
        {
            if (cardsRoot == null)
            {
                return;
            }

            if (cardViews == null)
            {
                cardViews = new List<PhotoCardView>();
            }

            for (int i = cardViews.Count; i < count; i++)
            {
                PhotoCardView instance = null;
                if (cardPrefab != null)
                {
                    instance = Instantiate(cardPrefab, cardsRoot);
                }
                else if (i < cardsRoot.childCount)
                {
                    instance = cardsRoot.GetChild(i).GetComponent<PhotoCardView>();
                }

                if (instance != null)
                {
                    cardViews.Add(instance);
                }
            }
        }
    }
}
