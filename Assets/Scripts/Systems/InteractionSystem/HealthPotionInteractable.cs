using DG.Tweening;
using UnityEngine;

public class HealthPotionInteractable : Interactable
{
    float duration = 1f;
    public override void OnInteract()
    {
        InteractableActions.AddPotion.Invoke(itemData);
        GameManager.Instance.audioManager.PlaySound("key_pickup");
        Collider collider = GetComponent<Collider>();
        collider.enabled = false;
        Camera mainCam = Camera.main;
        RectTransform potionRect = GameManager.Instance.hud.potionText.GetComponent<RectTransform>();

        float potionZ = mainCam.WorldToScreenPoint(transform.position).z;
        Vector3 screenPos = RectTransformUtility.WorldToScreenPoint(mainCam, potionRect.position);

        transform.eulerAngles = Vector3.zero;

        Vector3 worldPos = mainCam.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, potionZ));
        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DOMove(worldPos, duration).SetEase(Ease.InQuad));
        sequence.Join(transform.DOScale(Vector3.zero, duration).SetEase(Ease.InQuad));

        sequence.OnComplete(() =>
        {
            Destroy(gameObject);
        });
        sequence.SetLink(gameObject);

    }
}
