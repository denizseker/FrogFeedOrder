using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;
using System;

public class Grape : Entity
{
    float segmentDuration = 0.3f;
    public Tween pathTween;
    public override void HitByTongue(Frog frog)
    {
        Hit();
    }

    public void Hit()
    {
        // Animasyon sýrasý oluþtur
        Sequence pickUpSequence = DOTween.Sequence();
        // Scale'i 0.7'den 1.7'ye hýzlýca artýr
        pickUpSequence.Append(transform.DOScale(1.4f, 0.07f).SetEase(Ease.InOutQuad));
        // Scale'i 1'den 0.7'ye yavaþça azalt
        pickUpSequence.Append(transform.DOScale(0.7f, 0.1f).SetEase(Ease.OutQuad));
        // Scale'i 1.7'den 1'e yavaþça geri getir
        pickUpSequence.Append(transform.DOScale(1f, 0.1f).SetEase(Ease.OutBack));
        // Animasyonu baþlat
        pickUpSequence.Play();
    }

    public void StartCollect()
    {
        pathTween.Play();
    }
    public void SetForCollect(Vector3[] cellPoints)
    {
        if (cellPoints == null || cellPoints.Length == 0)
        {
            Debug.LogWarning("cellPoints array is empty or null.");
            return;
        }

        //Debug.Log($"{gameObject} cellpoint last item: {cellPoints[^1]}");

        float totalDuration = segmentDuration * cellPoints.Length;

        Vector3[] reverseArray = cellPoints.Reverse().ToArray();

        pathTween = gameObject.transform.DOPath(reverseArray, totalDuration, PathType.Linear).Pause().OnComplete(() =>
        {

            transform.DOScale(new Vector3(0, 1, 0), 0.10f).SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                Destroy(gameObject);
            });

        }); ;
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
