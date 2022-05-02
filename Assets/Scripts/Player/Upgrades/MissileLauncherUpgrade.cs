using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Upgrades
{
    // [CreateAssetMenu(menuName="Upgrade")]
    public abstract class MissileLauncherUpgrade : ScriptableObject
    {
        public abstract void OnAttach(MissileLauncher launcher);
        public abstract void OnDettach(MissileLauncher launcher);
    }
}

