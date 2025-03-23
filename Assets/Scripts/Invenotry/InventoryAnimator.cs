using System.Collections;
using UnityEngine;

public class InventoryAnimator : MonoBehaviour
{
    public IEnumerator AnimateSnapToPosition(Transform obj, Vector3 targetLocalPos, Quaternion targetLocalRot, float duration)
    {
        Vector3 startPos = obj.localPosition;
        Quaternion startRot = obj.localRotation;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            obj.localPosition = Vector3.Lerp(startPos, targetLocalPos, t);
            obj.localRotation = Quaternion.Slerp(startRot, targetLocalRot, t);
            yield return null;
        }
        obj.localPosition = targetLocalPos;
        obj.localRotation = targetLocalRot;

        Rigidbody rb = obj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }
    }

    public IEnumerator AnimateUnsnap(Transform obj, Vector3 targetWorldPos, Quaternion targetWorldRot, float duration)
    {
        Vector3 startPos = obj.position;
        Quaternion startRot = obj.rotation;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            obj.position = Vector3.Lerp(startPos, targetWorldPos, t);
            obj.rotation = Quaternion.Slerp(startRot, targetWorldRot, t);
            yield return null;
        }
        obj.position = targetWorldPos;
        obj.rotation = targetWorldRot;

        Rigidbody rb = obj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
        }
        Collider col = obj.GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = true;
        }
    }
}
