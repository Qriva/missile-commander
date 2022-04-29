using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public static class Yields
    {
        public static readonly WaitForEndOfFrame WaitForEndOfFrame = new WaitForEndOfFrame();
        public static readonly WaitForFixedUpdate WaitForFixedUpdate = new WaitForFixedUpdate();
        public static readonly WaitForSeconds WaitForTenthOfSecond = new WaitForSeconds(0.1f);
        public static readonly WaitForSeconds WaitForQuarterSecond = new WaitForSeconds(0.25f);
        public static readonly WaitForSeconds WaitForHalfSecond = new WaitForSeconds(0.5f);
        public static readonly WaitForSeconds WaitForOneSecond = new WaitForSeconds(1f);
        public static readonly WaitForSeconds WaitForTwoSeconds = new WaitForSeconds(2f);
        public static readonly WaitForSeconds WaitForFiveSeconds = new WaitForSeconds(5f);

        public enum Instruction
        {
            WaitForEndOfFrame, WaitForFixedUpdate, 
            WaitForTenthOfSecond, WaitForQuarterSecond, 
            WaitForHalfSecond, WaitForOneSecond, 
            WaitForTwoSeconds, WaitForFiveSeconds
        }

        public static YieldInstruction Get(Instruction i)
        {
            switch (i)
            {
                case Instruction.WaitForEndOfFrame: return WaitForEndOfFrame;
                case Instruction.WaitForFixedUpdate: return WaitForFixedUpdate;
                case Instruction.WaitForTenthOfSecond: return WaitForTenthOfSecond;
                case Instruction.WaitForQuarterSecond: return WaitForQuarterSecond;
                case Instruction.WaitForHalfSecond: return WaitForHalfSecond;
                case Instruction.WaitForOneSecond: return WaitForOneSecond;
                case Instruction.WaitForTwoSeconds: return WaitForTwoSeconds;
                case Instruction.WaitForFiveSeconds: return WaitForFiveSeconds;
                default: return null;
            }
        }
    }
}