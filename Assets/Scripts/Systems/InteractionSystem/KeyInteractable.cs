using DG.Tweening;
using UnityEngine;

public class KeyInteractable : Interactable
{
    public Rotate rotateScript;
    float duration = 4f;
    public override void OnInteract()
    {
        InteractableActions.AddKey?.Invoke(itemData);
        GameManager.Instance.audioManager.PlaySound("key_pickup");
        Collider collider = GetComponent<Collider>();
        collider.enabled = false;
        if (rotateScript != null)
        {
            rotateScript.enabled = false;
        }
        Camera mainCam = Camera.main;
        RectTransform keyRect = GameManager.Instance.hud.keyIcon.GetComponent<RectTransform>();

        float keyZ = mainCam.WorldToScreenPoint(transform.position).z;
        Vector3 screenPos = RectTransformUtility.WorldToScreenPoint(null, keyRect.position);

        transform.eulerAngles = Vector3.zero;

        Vector3 worldPos = mainCam.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, 5000f));
        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DOMove(worldPos, duration).SetEase(Ease.InQuad));
        sequence.Join(transform.DOScale(Vector3.zero, duration).SetEase(Ease.InQuad).OnComplete(() =>
        {
            Destroy(gameObject);
        }));
    }
}
