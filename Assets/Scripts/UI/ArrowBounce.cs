using UnityEngine;
using DG.Tweening;

public class ArrowBounce : MonoBehaviour
{
  private Tween bounceTween;

  void OnEnable()
  {
    bounceTween = transform
        .DOLocalMoveY(transform.localPosition.y + 20f, 0.6f)
        .SetLoops(-1, LoopType.Yoyo)
        .SetEase(Ease.InOutSine);
  }

  void OnDisable()
  {
    bounceTween?.Kill();
  }
}
