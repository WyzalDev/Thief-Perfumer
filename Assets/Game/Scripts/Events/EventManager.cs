using System;
using Game.Scripts.SceneManagement;
using UnityEngine;

namespace Game.Scripts.Events
{
    public static class EventManager
    {
        //Player Detection events
        public static Action<Vector3, int> OnPlayerDetect;
        public static Action<Vector3> OnCriticalPlayerDetect;

        //Game flow Events
        public static Action PlayerCaught;
        public static Action<SceneName> PlayerGetTarget;
        
        //Audio Events
        public static Action OnStep;
        public static Action OnDash;
        public static Action OnDamageTick;
        public static Action<AudioSource> OnGuardStep;
        public static Action OnInteract;
        public static Action OnDetect;
        
        //AudioEvents Handle
        public static void InvokeOnStep() => OnStep?.Invoke();
        public static void InvokeOnDash() => OnDash?.Invoke();
        public static void InvokeOnDamageTick() => OnDamageTick?.Invoke();
        
        public static void InvokeOnGuardStep(AudioSource audio) => OnGuardStep?.Invoke(audio);
        
        public static void InvokeOnInteract() => OnInteract?.Invoke();
        
        public static void InvokeOnDetect() => OnDetect?.Invoke();
        
        
        //Handle Player Detection events
        public static void InvokeOnPlayerDetect(Vector3 playerPoint, int guardID) => OnPlayerDetect?.Invoke(playerPoint, guardID);
        public static void InvokeOnCriticalPlayerDetect(Vector3 playerPoint) => OnCriticalPlayerDetect?.Invoke(playerPoint);
        
        //Handle Game flow Events
        public static void InvokePlayerCaught() => PlayerCaught?.Invoke();
        public static void InvokePlayerGetTarget(SceneName sceneName) => PlayerGetTarget?.Invoke(sceneName);
    }
}