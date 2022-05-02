using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Upgrades
{
    [CreateAssetMenu(menuName = "Upgrade/Missile Speed")]
    public class MissileSpeedUpgrade : MissileLauncherUpgrade
    {
        [Range(1f, 2f)]
        [SerializeField] private float speedMultiplier = 1.2f;

        public override void OnAttach(MissileLauncher launcher)
        {
            launcher.spawner.speed *= speedMultiplier;
        }

        public override void OnDettach(MissileLauncher launcher)
        {
            launcher.spawner.speed /= speedMultiplier;
        }
    }
}
