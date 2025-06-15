using System.Collections;
using Game.Scripts.Events;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.Environment
{
    public class DeadZone : MonoBehaviour
    {
        [SerializeField] private float timeToDie;
        [SerializeField] private float timeToHideSlider;
        [SerializeField] private Slider slider;
        [SerializeField] private float fadePower;
        [SerializeField] private float DamageTickTime;
        
        private static float _currTime;
        private static bool _isEnd;
        private static Coroutine _activeSliderCoroutine;

        private float currDamageTickTime;

        private void Awake()
        {
            slider.gameObject.SetActive(false);
            var material = GetComponent<Renderer>().material;
            material.SetFloat("_FadePower", fadePower);
            _isEnd = false;
            _currTime = 0;
            _activeSliderCoroutine = null;
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                _currTime = Mathf.Clamp(_currTime + Time.deltaTime, 0, timeToDie);
            }

            slider.value = _currTime / timeToDie;
            
            currDamageTickTime = Mathf.Clamp(currDamageTickTime + Time.deltaTime, 0, DamageTickTime);
            if (Mathf.Approximately(currDamageTickTime, DamageTickTime))
            {
                EventManager.InvokeOnDamageTick();
                currDamageTickTime = 0;
            }
            if (!Mathf.Approximately(_currTime, timeToDie) || _isEnd) return;
            _isEnd = true;
            EventManager.InvokePlayerCaught();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (_activeSliderCoroutine != null) StopCoroutine(_activeSliderCoroutine);
            slider.gameObject.SetActive(true);
            currDamageTickTime = DamageTickTime;
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (_activeSliderCoroutine != null) StopCoroutine(_activeSliderCoroutine);
            if (gameObject !=null) _activeSliderCoroutine = StartCoroutine(HideSlider());
            currDamageTickTime = 0;
        }

        private IEnumerator HideSlider()
        {
            yield return new WaitForSeconds(timeToHideSlider);
            slider.gameObject.SetActive(false);
        }

    }
}