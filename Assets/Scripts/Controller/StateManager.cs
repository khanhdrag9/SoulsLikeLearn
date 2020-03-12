using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{
    public class StateManager : MonoBehaviour
    {
        [Header("Inputs")]
        public float vertical;
        public float horizontal;
        public float moveAmount;
        public Vector3 moveDirection;

        [Header("Stats")]
        public float moveSpeed ;
        public float runSpeed ;
        public float rotateSpeed;
        public float toGround = 0.5f;

        [Header("States")]
        public bool run;
        public bool onGround;
        public bool lockon;

        [Header("Init")]
        public GameObject activeModel;

        public Animator animator { get; protected set; }
        public Rigidbody rigidbody { get; protected set; }

        public float delta { get; set; }
        [HideInInspector] public LayerMask ignoreLayers;

        public void Init()
        {
            SetupAnimator();
            rigidbody = GetComponent<Rigidbody>();
            rigidbody.angularDrag = 999;
            rigidbody.drag = 4;
            rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

            gameObject.layer = 8;
            ignoreLayers = ~(1 << 9);
            animator.SetBool("onGround", true);
        }

        private void SetupAnimator()
        {
            if (activeModel == null)
            {
                animator = GetComponentInChildren<Animator>();
                if (animator == null)
                {
                    Debug.Log("No model found");
                    return;
                }
                else
                {
                    activeModel = animator.gameObject;
                }
            }

            if (animator == null)
                animator = activeModel.GetComponent<Animator>();
        }
        public void FixedTick(float dt)
        {
            delta = dt;

            rigidbody.drag = moveAmount > 0 || onGround == false ? 0 : 4;

            float targetSpeed = moveSpeed;
            if (run)
            {
                targetSpeed = runSpeed;
                lockon = false;
            }
            if (onGround)
                rigidbody.velocity = moveDirection * targetSpeed;
            else
                moveAmount = 0;

            
            HandleRotation();
            HandleMovementAnimations();
        }

        public void Tick(float dt)
        {
            delta = dt;
            onGround = OnGround();
            animator.SetBool("onGround", onGround);
        }

        void HandleMovementAnimations()
        {
            animator.SetBool("run", run);
            animator.SetFloat("vertical", moveAmount, 0.24f, delta);
        }

        void HandleRotation()
        {
            if (!lockon)
            {
                Vector3 targetDir = moveDirection;
                targetDir.y = 0;
                if (targetDir == Vector3.zero)
                    targetDir = transform.forward;
                Quaternion tr = Quaternion.LookRotation(targetDir);
                Quaternion targetRotation = Quaternion.Slerp(transform.rotation, tr, delta * moveAmount * rotateSpeed);
                transform.rotation = targetRotation;
            }
        }

        public bool OnGround()
        {
            bool r = false;

            Vector3 origin = transform.position + Vector3.up * toGround;
            Vector3 dir = -Vector3.up;
            float dis = toGround + 0.2f;
            RaycastHit hit;
            if(Physics.Raycast(origin, dir, out hit, dis, ignoreLayers))
            {
                r = true;
                Vector3 targetPosition = hit.point;
                transform.position = targetPosition; 
            }
            Debug.DrawRay(origin, dir * dis, Color.red);

            return r;
        }
    }
}
