using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    [System.AttributeUsage(System.AttributeTargets.Field)]
    public class ReadOnlyAttribute : PropertyAttribute
    {
        public Mode mode;

        public ReadOnlyAttribute(Mode mode = Mode.Always)
        {
            this.mode = mode;
        }

        public enum Mode
        {
            Always, PlaymodeOnly, EditorOnly
        }
    }
}