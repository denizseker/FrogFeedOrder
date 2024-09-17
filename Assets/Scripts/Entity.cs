using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    public CellManager.Direction entityDirection; // Direction the entity is facing
    public Entity_Cell entityCell; // Cell the entity is in
    public GameManager.Colour entityColour; // Color of the entity

    // Sound effects
    public AudioClip hitSound; // Sound when hit
    public AudioClip destroySound; // Sound when destroyed

    public GameObject destroyVFX; // Visual effect when destroyed

    private void Start()
    {
        if (destroyVFX != null)
        {
            // Create a pool with 10 destroyVFX objects
            ObjectPool.Instance.CreatePool(destroyVFX, 10);
        }
    }

    // Override these methods to customize behavior
    public virtual void HitByTongue(Frog frog)
    {
        PlaySound(hitSound); // Play hit sound
    }

    public virtual void DestroyEntity()
    {
        PlaySound(destroySound); // Play destroy sound
    }

    // Show destroy visual effect
    protected void TriggerDestroyVFX()
    {
        if (destroyVFX != null)
        {
            // Get a destroyVFX object from the pool
            GameObject vfxInstance = ObjectPool.Instance.GetFromPool(destroyVFX, transform.position, Quaternion.identity);
            vfxInstance.transform.localScale = Vector3.one * 3.5f; // Scale the VFX
            // Return the VFX to the pool after 2 seconds
            CoroutineHelper.Instance.StartHelperCoroutine(ReturnVFXToPool(vfxInstance, 2f));
        }
        else
        {
            Debug.Log("destroyVFX is null"); // Log if destroyVFX is not set
        }
    }

    // Coroutine to return the VFX to the pool after a delay
    private IEnumerator ReturnVFXToPool(GameObject vfxInstance, float delay)
    {
        yield return new WaitForSeconds(delay); // Wait for delay
        ObjectPool.Instance.ReturnToPool(vfxInstance); // Return to pool
    }

    // Play a sound at the entity's position
    protected void PlaySound(AudioClip clip)
    {
        if (clip != null)
        {
            AudioSource.PlayClipAtPoint(clip, transform.position); // Play sound
        }
    }

    // Show spawn animation
    public void TriggerSpawnAnimation()
    {
        // Scale to original size over 0.45 seconds
        transform.DOScale(1f, 0.45f).SetEase(Ease.Linear);
    }
}
