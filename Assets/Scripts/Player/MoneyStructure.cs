using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class MoneyStructure : Building
    {
        [Tooltip("Amount of money generated per wave")]
        public int moneyGeneration = 10;
    }
}