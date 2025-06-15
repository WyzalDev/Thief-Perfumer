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
        
        //Handle Player Detection events
        public static void InvokeOnPlayerDetect(Vector3 playerPoint, int guardID) => OnPlayerDetect?.Invoke(playerPoint, guardID);
        public static void InvokeOnCriticalPlayerDetect(Vector3 playerPoint) => OnCriticalPlayerDetect?.Invoke(playerPoint);
        
        //Handle Game flow Events
        public static void InvokePlayerCaught() => PlayerCaught?.Invoke();
        public static void InvokePlayerGetTarget(SceneName sceneName) => PlayerGetTarget?.Invoke(sceneName);
    }
}