using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity_Cell : MonoBehaviour
{

    public Entity entityOnCell;
    [HideInInspector] public Default_Cell ownerDefaultCell;
    public GameManager.Colour cellColour;


    public void StartDestory()
    {

        transform.DOScale(new Vector3(0,1,0), 0.30f).SetEase(Ease.Linear)
            .OnComplete(() =>
        {
            if (entityOnCell.GetType() == typeof(Arrow)) Destroy(entityOnCell.gameObject);
            Destroy(gameObject);
        });

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
