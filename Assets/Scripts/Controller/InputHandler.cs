using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{
    public class InputHandler : MonoBehaviour
    {
        private float vertical;
        private float horizontal;
        private bool run;

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
            run = Input.GetButton("RunInput");
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

            if(run)
            {
                state.run = state.moveAmount > 0;
            }
            else
            {
                state.run = false;
            }
        }
    }
}
