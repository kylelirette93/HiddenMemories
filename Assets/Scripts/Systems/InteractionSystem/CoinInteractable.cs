using DG.Tweening;
using UnityEngine;

public class CoinPickup : MonoBehaviour
{
    public Rotate rotateScript;
    [SerializeField] ItemDataSO coinData;
    float duration = 4f;
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

            float coinZ = mainCam.WorldToScreenPoint(transform.position).z;
            Vector3 screenPos = RectTransformUtility.WorldToScreenPoint(null, currencyRect.position);

            transform.eulerAngles = new Vector3(-90, 0, 0);

            Vector3 worldPos = mainCam.ScreenToWorldPoint(new Vector3(screenPos.x - 200, screenPos.y, 5000f));
            Sequence sequence = DOTween.Sequence();
            sequence.Append(transform.DOMove(worldPos, duration).SetEase(Ease.InQuad));
            sequence.Join(transform.DOScale(Vector3.zero, duration).SetEase(Ease.InQuad));
             sequence.OnComplete(() =>
             {
                 Destroy(gameObject);
             });
        }
    }
}
