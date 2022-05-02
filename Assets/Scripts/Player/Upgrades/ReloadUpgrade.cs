using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Upgrades
{
    [CreateAssetMenu(menuName = "Upgrade/Reload Speed")]
    public class ReloadUpgrade : MissileLauncherUpgrade
    {
        [Range(0.5f, 1f)]
        [SerializeField] private float speedMultiplier = 0.8f;

        public override void OnAttach(MissileLauncher launcher)
        {
            launcher.reloadTime *= speedMultiplier;
        }

        public override void OnDettach(MissileLauncher launcher)
        {
            launcher.reloadTime /= speedMultiplier;
        }
    }
}
