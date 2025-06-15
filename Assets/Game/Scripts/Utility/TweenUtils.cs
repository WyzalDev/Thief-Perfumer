using DG.Tweening;
using Unity.Cinemachine;

namespace Game.Scripts.Utility
{
    public static class TweenUtils
    {
        public static Tweener DOOrthoSize(
            this CinemachineCamera camera,
            float endValue,
            float duration
        )
        {
            return DOTween.To(
                () => camera.Lens.OrthographicSize,
                x => {
                    var lens = camera.Lens;
                    lens.OrthographicSize = x;
                    camera.Lens = lens;
                },
                endValue,
                duration
            );
        }
    }
}