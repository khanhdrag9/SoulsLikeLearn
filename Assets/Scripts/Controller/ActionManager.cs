using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{ 
    [System.Serializable]
    public class InputAction
    {
        public InputType input;
        public string animation;
    }

    public enum InputType
    {
        RB, RT, LB, LT, COUNT
    }

    public class ActionManager : MonoBehaviour
    {
        public List<InputAction> actions;

        private StateManager state;

        public ActionManager()
        {
            actions = new List<InputAction>();
            for (int i = 0; i < (int)InputType.COUNT; i++)
            {
                actions.Add(new InputAction
                {
                    input = (InputType)i
                });
            }
        }
        public void Init(StateManager st)
        {
            state = st;
            UpdateOneHand();
        }

        public void UpdateOneHand()
        {
            Weapon cur = state.inventory.curWeapon;
            for(int i = 0; i < cur.oneHand.Count; i++)
            {
                InputAction ac = GetAction(cur.oneHand[i].input);
                ac.animation = cur.oneHand[i].animation;
            }
        }

        public void UpdateTwoHand()
        {
            Weapon cur = state.inventory.curWeapon;
            for (int i = 0; i < cur.twoHand.Count; i++)
            {
                InputAction ac = GetAction(cur.twoHand[i].input);
                ac.animation = cur.twoHand[i].animation;
            }
        }

        public InputAction GetAction()
        {
            if (state.RB) return GetAction(InputType.RB);
            if (state.RT) return GetAction(InputType.RT);
            if (state.LB) return GetAction(InputType.LB);
            if (state.LT) return GetAction(InputType.LT);

            return GetAction(InputType.RB);
        }

        public InputAction GetAction(InputType type)
        {
            foreach(var e in actions)
            {
                if(e.input == type)
                {
                    return e;
                }
            }
            return null;
        }
    }
}
