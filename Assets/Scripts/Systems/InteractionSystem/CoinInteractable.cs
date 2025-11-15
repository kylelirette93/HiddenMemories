using DG.Tweening;
using UnityEngine;

public class CoinPickup : MonoBehaviour
{
    public Rotate rotateScript;
    [SerializeField] ItemDataSO coinData;
    float duration = 4f;
    Sequence sequence;
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ItemDataSO itemData = coinData;
            InteractableActions.AddCash?.Invoke(itemData);
            Collider collider = GetComponent<Collider>();
            collider.enabled = false;
            if (rotateScript != null)
            {
                rotateScript.enabled = false;
            }
            Camera mainCam = Camera.main;
            RectTransform currencyRect = GameManager.Instance.hud.currencyText.GetComponent<RectTransform>();
            if (currencyRect == null)
            {
                Debug.Log("Currency rect is null!");
            }

            float duration = 1f;
            float elapsed = 0f;
            Vector3 startPos = transform.position;
            DOTween.To(() => elapsed, x => elapsed = x, duration, duration)
                .OnUpdate(() =>
                {
                    Vector3 targetPos = mainCam.ScreenToWorldPoint(new Vector3(currencyRect.position.x, currencyRect.position.y, 1));
                    transform.position = Vector3.Lerp(startPos, targetPos, DOVirtual.EasedValue(0, 1, elapsed / duration, Ease.InQuad));
                    Vector3 smallerCoin = new Vector3(0.25f, 0.25f, 0.25f);
                    transform.localScale = Vector3.Lerp(smallerCoin, Vector3.zero, elapsed / duration);
                })
                .OnComplete(() =>
                {
                    Destroy(gameObject);
                })
                .SetLink(gameObject);
        }
    }
}
