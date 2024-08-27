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
        Hit();
    }

    // Plays the hit animation sequence
    public void Hit()
    {
        // Create a sequence for the pick-up animation
        Sequence pickUpSequence = DOTween.Sequence();

        // Quickly scale from 0.7 to 1.4
        pickUpSequence.Append(transform.DOScale(1.4f, 0.07f).SetEase(Ease.InOutQuad));

        // Slowly scale down from 1.4 to 0.7
        pickUpSequence.Append(transform.DOScale(0.7f, 0.1f).SetEase(Ease.OutQuad));

        // Slowly return scale to 1
        pickUpSequence.Append(transform.DOScale(1f, 0.1f).SetEase(Ease.OutBack));

        // Start the animation
        pickUpSequence.Play();
    }

    // Sets up the grape to be collected, moving it along the specified path
    public void SetForCollect(Vector3[] cellPoints, Sequence collectSequence)
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
                // Shrink and destroy the grape after completing the movement
                transform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.Linear)
                    .OnComplete(() => Destroy(gameObject));
            });

        // Join the grape's return sequence with the main collection sequence
        collectSequence.Join(grapeReturnSequence);
    }
}
