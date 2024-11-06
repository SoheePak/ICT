using DG.Tweening;
using UnityEngine;

public class Card : MonoBehaviour
{
    [SerializeField] private Sprite animalSprite;
    [SerializeField] private SpriteRenderer cardRenderer;
    [SerializeField] private Sprite backSprite;

    private bool isFlipped = false;
    private bool isFlipping = false;
    private bool isMatched = false;
    public int cardID { get; private set; }

    // 개선: 애니메이션 지속 시간을 상수로 정의
    private const float FLIP_DURATION = 0.2f;

    public void SetCardID(int id) => cardID = id;
    public void SetMatched() => isMatched = true;
    public void SetAnimalSprite(Sprite sprite) => animalSprite = sprite;

    public void FlipCard()
    {
        if (isFlipping) return;

        AudioManager.Instance.PlayOneShot("cardopen");
        isFlipping = true;

        Vector3 originalScale = transform.localScale;
        Vector3 targetScale = new Vector3(0f, originalScale.y, originalScale.z);

        transform.DOScale(targetScale, FLIP_DURATION).OnComplete(() =>
        {
            isFlipped = !isFlipped;
            cardRenderer.sprite = isFlipped ? animalSprite : backSprite;

            transform.DOScale(originalScale, FLIP_DURATION).OnComplete(() =>
            {
                isFlipping = false;
            });
        });
    }

    private void OnMouseDown()
    {
        if (!isFlipping && !isMatched && !isFlipped)
        {
            CardManager.instance.CardClicked(this);
        }
    }
}