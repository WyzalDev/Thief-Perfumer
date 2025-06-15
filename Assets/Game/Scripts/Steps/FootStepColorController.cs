using System.Collections;
using System.Collections.Generic;
using Game.Scripts.Events;
using Game.Scripts.SceneManagement;
using UnityEngine;

namespace Game.Scripts.Steps
{
    public class FootStepColorController : MonoBehaviour
    {
        [ColorUsage(true, true)] [SerializeField]
        public Color _grayColor;

        [ColorUsage(true, true)] [SerializeField]
        private List<Color> possibleColors;

        [SerializeField] private float rememberTime = 1f;

        private int _possibleColorIndex = 0;
        private Coroutine _firstRemember;
        private Coroutine _secondRemember;

        private Material _firstRememberMaterial;
        private Material _secondRememberMaterial;

        private void Start()
        {
            EventManager.PlayerCaught += ResetMaterialColor;
            EventManager.PlayerGetTarget += ResetMaterialColor;
        }

        public void MemorizeFootStep(Renderer footStep)
        {
            if (_firstRemember != null)
            {
                if (_firstRememberMaterial != null && _firstRememberMaterial == footStep.sharedMaterial)
                {
                    StopCoroutine(_firstRemember);
                    _firstRemember = null;
                    _firstRememberMaterial.SetColor("_Color", _grayColor);
                }
                else if (_secondRemember != null)
                {
                    StopCoroutine(_secondRemember);
                    _secondRemember = _firstRemember;
                    _secondRememberMaterial.SetColor("_Color", _grayColor);
                    _secondRememberMaterial = _firstRememberMaterial;
                }
                else
                {
                    _secondRemember = _firstRemember;
                    _secondRememberMaterial = _firstRememberMaterial;
                }
            }

            _firstRememberMaterial = footStep.sharedMaterial;
            _firstRemember = StartCoroutine(MemorizeColor());
        }

        private IEnumerator MemorizeColor()
        {
            var color = possibleColors[_possibleColorIndex % possibleColors.Count];
            var material = _firstRememberMaterial;
            _possibleColorIndex++;
            material.SetColor("_Color", color);
            
            float elapsed = 0f;
            while (elapsed < rememberTime)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / rememberTime);
                material.SetColor("_Color", Color.Lerp(color, _grayColor, t));
                yield return null;
            }
        }

        private void ResetMaterialColor(SceneName sceneName)
        {
            ResetMaterialColor();
        }
        
        private void ResetMaterialColor()
        {
            if(_firstRemember != null) StopCoroutine(_firstRemember);
            if(_secondRemember != null) StopCoroutine(_secondRemember);
            
            _firstRememberMaterial?.SetColor("_Color", _grayColor);
            _secondRememberMaterial?.SetColor("_Color", _grayColor);
        }

        private void OnDestroy()
        {
            EventManager.PlayerCaught -= ResetMaterialColor;
            EventManager.PlayerGetTarget -= ResetMaterialColor;
        }
    }
}