using UnityEngine;
using DG.Tweening;

public class ArrowBounce : MonoBehaviour
{
  private Tween bounceTween;
  private Vector3 initialPos;

  void Awake()
  {
    initialPos = transform.localPosition;
  }

  void OnEnable()
  {
    transform.localPosition = initialPos;

    bounceTween = transform
        .DOLocalMoveY(initialPos.y + 20f, 0.6f)
        .SetLoops(-1, LoopType.Yoyo)
        .SetEase(Ease.InOutSine);
  }

  void OnDisable()
  {
    bounceTween?.Kill();
  }
}
