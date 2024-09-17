using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;

public class Grape : Entity
{
    // Duration for each segment in the movement sequence
    float segmentDuration = 0.3f;

    // Triggered when hit by Frog's tongue
    public override void HitByTongue(Frog frog)
    {
        Hit(); // Play hit animation
    }

    // Plays the hit animation sequence
    public void Hit()
    {
        PlaySound(hitSound); // Play hit sound
        Vector3 originalScale = transform.localScale;

        // Scale animation
        transform.DOScale(new Vector3(1.5f, 0.5f, 1.5f), 0.1f)
            .OnComplete(() =>
            {
                // Return to original scale
                transform.DOScale(originalScale, 0.1f);
            });
    }

    // Sets up the grape to be collected, moving it along the specified path
    public void SetForCollect(Vector3[] cellPoints, Sequence collectSequence, Frog frog)
    {
        if (cellPoints == null || cellPoints.Length == 0)
        {
            Debug.LogWarning("cellPoints array is empty or null.");
            return;
        }

        // Reverse the array of points for the return path
        Vector3[] reverseArray = cellPoints.Reverse().ToArray();

        // Create a sequence for the grape's return movement
        Sequence grapeReturnSequence = DOTween.Sequence();

        // Append each point in the reversed array to the sequence
        foreach (Vector3 point in reverseArray)
        {
            grapeReturnSequence.Append(transform.DOMove(point, segmentDuration).SetEase(Ease.Linear));
        }

        // Set up actions on sequence start and completion
        grapeReturnSequence
            .OnStart(() =>
            {
                // Detach the grape from its parent to move independently
                transform.parent = null;
            })
            .OnComplete(() =>
            {
                // Increase frog size
                Vector3 currentScale = frog.transform.localScale;
                Vector3 newScale = currentScale + new Vector3(0.1f, 0.1f, 0.1f);
                frog.transform.DOScale(newScale, 0.1f).SetEase(Ease.InOutQuad);
                PlaySound(destroySound); // Play destroy sound
                TriggerDestroyVFX(); // Play destroy visual effect
            });

        // Start shrinking the grape when the last DOMove starts
        grapeReturnSequence.Insert(grapeReturnSequence.Duration() - segmentDuration, transform.DOScale(Vector3.zero, segmentDuration).SetEase(Ease.Linear)
            .OnComplete(() => Destroy(gameObject))); // Destroy grape

        // Join the grape's return sequence with the main collection sequence
        if (collectSequence.IsActive()) collectSequence.Join(grapeReturnSequence);
    }
}
