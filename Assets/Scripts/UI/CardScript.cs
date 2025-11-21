using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CardScript : MonoBehaviour
{
  [SerializeField] private Image Card_Image;
  [SerializeField] private LayoutElement Card_LE;
  [SerializeField] private Transform Card_transform;
  [SerializeField] private BJController bjManager;
  private Sprite csprite = null;

  private void Start()
  {
    bjManager = GameObject.FindWithTag("GameController").GetComponent<BJController>();
  }

  internal void OnFlipMethod(Sprite cardSprite, int value)
  {
    csprite = cardSprite;
    Card_transform.localEulerAngles = new Vector3(0, 180, 0);
    Card_transform.DORotate(new Vector3(0, 0, 0), 0.5f, RotateMode.FastBeyond360).OnComplete(delegate
    {
      Card_LE.ignoreLayout = false;
      bjManager.AfterCardFlip(value);
    });
    DOVirtual.DelayedCall(0.15f, ChangeSprite);
  }

  private void ChangeSprite()
  {
    Card_Image.sprite = csprite;
  }
}
