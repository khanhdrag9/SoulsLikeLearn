using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{
    [System.Serializable]
    public class Weapon
    {
        public List<InputAction> oneHand;
        public List<InputAction> twoHand;
    }

    public class Inventory : MonoBehaviour
    {
        public Weapon curWeapon;
        public void Init()
        {

        }
    }
}
