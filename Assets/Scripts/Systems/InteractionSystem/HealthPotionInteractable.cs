using DG.Tweening;
using UnityEngine;

public class HealthPotionInteractable : Interactable
{
    public override void OnInteract()
    {
        InteractableActions.AddPotion?.Invoke(itemData);
        GameManager.Instance.audioManager.PlaySound("key_pickup");
        Collider collider = GetComponent<Collider>();
        collider.enabled = false;

        Camera mainCam = Camera.main;
        RectTransform potionRect = GameManager.Instance.hud.potionText.GetComponent<RectTransform>();
        if (potionRect == null)
        {
            Debug.Log("Potion rect is null!");
        }

        float duration = 1f;
        float elapsed = 0f;
        Vector3 startPos = transform.position;
        transform.localRotation = Quaternion.Euler(-90f, 0f, 0f);
        DOTween.To(() => elapsed, x => elapsed = x, duration, duration)
            .OnUpdate(() =>
            {
                Vector3 targetPos = mainCam.ScreenToWorldPoint(new Vector3(potionRect.position.x, potionRect.position.y, 1000f));
                transform.position = Vector3.Lerp(startPos, targetPos, DOVirtual.EasedValue(0, 1, elapsed / duration, Ease.InQuad));
                Vector3 smallerPotion = new Vector3(0.25f, 0.25f, 0.25f);
                transform.localScale = Vector3.Lerp(smallerPotion, Vector3.zero, elapsed / duration);
            })
            .OnComplete(() =>
            {
                Destroy(gameObject);
            })
            .SetLink(gameObject);

    }
}