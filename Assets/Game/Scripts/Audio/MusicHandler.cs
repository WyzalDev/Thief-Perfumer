using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace Game.Scripts.Audio
{
    public class MusicHandler : MonoBehaviour
    {
        private IEnumerator Start()
        {
            yield return AudioStorage.PlayGlobalMusic("BackMusic", AudioStorage.fadeSettings).WaitForCompletion();
        }
    }
}