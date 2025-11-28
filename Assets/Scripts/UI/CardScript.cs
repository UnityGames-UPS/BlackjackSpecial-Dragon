using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CardScript : MonoBehaviour
{
  [SerializeField] private Image Card_Image;
  [SerializeField] internal LayoutElement layoutElement;
  [SerializeField] private Transform Card_transform;
  [SerializeField] private BJController bjManager;
  private Sprite csprite = null;

  private void Start()
  {
    layoutElement = GetComponent<LayoutElement>();
    bjManager = GameObject.FindWithTag("GameController").GetComponent<BJController>();
  }

  internal void OnFlipMethod(Sprite cardSprite, int value, Card card = null)
  {
    csprite = cardSprite;
    Card_transform.localEulerAngles = new Vector3(0, 180, 0);
    Card_transform.DOLocalRotate(new Vector3(0, 0, 0), 0.3f, RotateMode.FastBeyond360).OnComplete(delegate
    {
      layoutElement.ignoreLayout = false;
      bjManager.AfterCardFlip(value, card);
    });
    DOVirtual.DelayedCall(0.1f, ChangeSprite);
  }

  private void ChangeSprite()
  {
    Card_Image.sprite = csprite;
  }
}
