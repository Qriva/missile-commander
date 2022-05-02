using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Upgrades
{
    [CreateAssetMenu(menuName = "Upgrade/Ammo Capacity")]
    public class AmmoCapacityUpgrade : MissileLauncherUpgrade
    {
        [Range(1, 5)]
        [SerializeField] private int capacityBonus = 3;

        public override void OnAttach(MissileLauncher launcher)
        {
            launcher.ammoCapacity += capacityBonus;
        }

        public override void OnDettach(MissileLauncher launcher)
        {
            launcher.ammoCapacity -= capacityBonus;
        }
    }
}
