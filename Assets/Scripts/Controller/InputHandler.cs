using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{
    public class InputHandler : MonoBehaviour
    {
        private float vertical;
        private float horizontal;
        private bool B_input, A_input, X_input, Y_input;

        private bool RB_input, LB_input, RT_input, LT_input;
        private float RT_axis, LT_axis;
        private bool R_input, L_input;


        private StateManager state;
        private CameraManager camManager;

        private float dt;

        void Start()
        {
            state = GetComponent<StateManager>();
            state.Init();

            camManager = CameraManager.Instance;
            camManager.Init(transform);
        }


        void Update()
        {
            dt = Time.deltaTime;
            state.Tick(dt);
        }

        void FixedUpdate()
        {
            dt = Time.fixedDeltaTime;
            GetInput();
            UpdateStates();
            state.FixedTick(dt);
            camManager.Tick(dt);
        }

        void GetInput()
        {
            vertical = Input.GetAxis("Vertical");
            horizontal = Input.GetAxis("Horizontal");

            B_input = Input.GetButton("B_input");
            Y_input = Input.GetButtonUp("Y_input");
            X_input = Input.GetButtonDown("X_input");

            RT_input = Input.GetButton("RT_input");
            RT_axis = Input.GetAxis("RT_input");
            if (RT_axis != 0) RT_input = true;

            LT_input = Input.GetButton("LT_input");
            LT_axis = Input.GetAxis("LT_input");
            if (LT_axis != 0) LT_input = true;

            RB_input = Input.GetButton("RB_input");
            LB_input = Input.GetButton("LB_input");

            R_input = Input.GetButtonDown("R_input");
            L_input = Input.GetButtonDown("L_input");
        }
         
        void UpdateStates()
        {
            state.horizontal = horizontal;
            state.vertical = vertical;

            Vector3 v = vertical * camManager.transform.forward;
            Vector3 h = horizontal * camManager.transform.right;
            state.moveDirection = (v + h).normalized;
            float m = Mathf.Abs(horizontal) + Mathf.Abs(vertical);
            state.moveAmount = Mathf.Clamp01(m);

            state.itemInput = X_input;
            state.rollInput = B_input;
            if(B_input)
            {
                //state.run = state.moveAmount > 0;
            }
            else
            {
                //state.run = false;
            }

            state.RT = RT_input;
            state.RB = RB_input;
            state.LT = LT_input;
            state.LB = LB_input;

            if(Y_input)
            {
                Y_input = false;
                state.twoHand = !state.twoHand;
                state.TwoHandHandler();
            }

            if(R_input)
            {
                R_input = false;
                state.lockon = !state.lockon;
                if (state.lockonTarget == null)
                {
                    state.lockon = false;
                    camManager.lockon = false;
                }
                else
                { 
                    camManager.lockonTarget = state.lockonTarget.transform;
                    camManager.lockon = state.lockon;
                }
            }
        }
    }
}
