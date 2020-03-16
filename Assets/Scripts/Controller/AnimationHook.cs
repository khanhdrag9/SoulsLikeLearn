using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{
    public class AnimationHook : MonoBehaviour
    {
        Animator animator;
        StateManager state;
        public float rm_mul;

        public void Init(StateManager st)
        {
            state = st;
            animator = st.animator;
        }

        void OnAnimatorMove()
        {
            if (state.canMove)
                return;

            state.rigidbody.drag = 0;
            if (rm_mul == 0)
                rm_mul = 1;

            Vector3 dt = animator.deltaPosition;
            dt.y = 0;
            Vector3 v = (dt * rm_mul) / state.delta;
            state.rigidbody.velocity = v;
        }

        public void LateTick()
        {
            
        }
    }
}
