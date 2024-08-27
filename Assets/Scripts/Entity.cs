using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    public CellManager.Direction entityDirection;
    public Entity_Cell entityCell;
    public GameManager.Colour entityColour;
    // Ses efektleri i�in referanslar
    public AudioClip hitSound;
    public AudioClip destroySound;

    // Bu metodlar� override ederek �zelle�tirin
    public virtual void HitByTongue(Frog frog)
    {
        PlaySound(hitSound);
    }

    public virtual void DestroyEntity()
    {
        PlaySound(destroySound);
    }

    // Ses �alma metodunu genel bir �ekilde tan�mla
    protected void PlaySound(AudioClip clip)
    {
        if (clip != null)
        {
            AudioSource.PlayClipAtPoint(clip, transform.position);
        }
    }

    public void TriggerSpawnAnimation()
    {
        transform.DOScale(1f, 0.45f).SetEase(Ease.Linear);
    }
}
