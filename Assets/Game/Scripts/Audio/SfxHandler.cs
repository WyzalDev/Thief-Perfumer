using Game.Scripts.Events;
using Game.Scripts.SceneManagement;
using UnityEngine;

namespace Game.Scripts.Audio
{
    public class SfxHandler : MonoBehaviour
    {
        private void Awake()
        {
            EventManager.OnStep += Step;
            EventManager.OnDash += Dash;
            EventManager.OnDamageTick += Scorch;
            EventManager.OnDetect += Alert;
            EventManager.PlayerCaught += CritAlert;
            EventManager.OnGuardStep += GuardStep;
            EventManager.PlayerGetTarget += TargetGetted;
            EventManager.OnInteract += Interact;
        }

        
        
        private void OnDestroy()
        {
            EventManager.OnStep -= Step;
            EventManager.OnDash -= Dash;
            EventManager.OnDamageTick -= Scorch;
            EventManager.OnDetect -= Alert;
            EventManager.PlayerCaught -= CritAlert;
            EventManager.OnGuardStep -= GuardStep;
            EventManager.PlayerGetTarget -= TargetGetted;
            EventManager.OnInteract -= Interact;
        }

        private void Interact()
        {
            AudioStorage.PlayGlobalSfx("Interact");
        }
        
        private void TargetGetted(SceneName name)
        {
            AudioStorage.PlayGlobalSfx("TargetGetted");
        }
        
        private void GuardStep(AudioSource audio)
        {
            AudioStorage.PlayGlobalSfx("GuardStep", audio);
        }

        private void Alert()
        {
            AudioStorage.PlayGlobalSfx("Alert");
        }
        
        private void CritAlert()
        {
            AudioStorage.PlayGlobalSfx("CritAlert");
        }
        
        private void Scorch()
        {
            AudioStorage.PlayGlobalSfx("Scorch");
        }

        private void Dash()
        {
            AudioStorage.PlayGlobalSfx("Dash");
        }

        private void Step()
        {
            AudioStorage.PlayGlobalSfx("Step");
        }
    }
}