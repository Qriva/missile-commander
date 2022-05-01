using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu(menuName="Other/InteractionMask")]
    public class InteractionMask : ScriptableObject
    {
        [SerializeField] private LayerMask mask = -1;
        
        public bool Contains(int layer)
        {
            return mask == (mask | (1 << layer));
        }
    }
}
