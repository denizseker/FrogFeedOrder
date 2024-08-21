using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Grape : Entity
{

    public override void Interact()
    {
        Debug.Log("Grape");
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


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
