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
        public bool rollInput;
        public bool RT, RB, LT, LB;

        [Header("Stats")]
        public float moveSpeed ;
        public float runSpeed ;
        public float rotateSpeed;
        public float toGround = 0.5f;
        public float rollSpeed; 

        [Header("States")]
        public bool run;
        public bool onGround;
        public bool lockon;
        public bool inAction;
        public bool canMove;
        public bool twoHand;
       

        [Header("Init")]
        public GameObject activeModel;

        [Header("Other")]
        public EnemyTarget lockonTarget;

        public Animator animator { get; protected set; }
        public Rigidbody rigidbody { get; protected set; }
        public AnimationHook animationHook { get; protected set; }

        public float delta { get; set; }
        [HideInInspector] public LayerMask ignoreLayers;

        float _actionDelay;

        public void Init()
        {
            SetupAnimator();
            rigidbody = GetComponent<Rigidbody>();
            rigidbody.angularDrag = 999;
            rigidbody.drag = 4;
            rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

            animationHook = activeModel.AddComponent<AnimationHook>();
            animationHook.Init(this);

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

            //if (canMove == false)
            //    return;

            DetectAction();

            if (inAction)
            {
                animator.applyRootMotion = true;
                _actionDelay += delta;
                if(_actionDelay > 0.3f)
                {
                    inAction = false;
                    _actionDelay = 0;
                }
                else
                {
                    return;
                }
            }

            canMove = animator.GetBool("canMove");
            if (!canMove)
                return;

            animationHook.rm_mul = 1;
            HandleRolls();

            animator.applyRootMotion = false;

            rigidbody.drag = moveAmount > 0 || onGround == false ? 0 : 4;

            float targetSpeed = moveSpeed;
            if (run)
            {
                targetSpeed = runSpeed;
                //lockon = false;
            }
            if (onGround)
                rigidbody.velocity = moveDirection * targetSpeed;
            else
                moveAmount = 0;

            
            HandleRotation();

            animator.SetBool("lockon", lockon);
            if (lockon)
                HandleLockonAnimations(moveDirection);
            else
                HandleMovementAnimations();
        }
        public void DetectAction()
        {
            if (canMove == false)
                return;

            if (RB == false && RT == false && LT == false && LB == false)
                return;

            string targetAnim = null;

            if (RB)
                targetAnim = "oh_attack_1";
            if (RT)
                targetAnim = "oh_attack_2";
            if (LT)
                targetAnim = "oh_attack_3";
            if (LB)
                targetAnim = "th_attack_1";

            if (string.IsNullOrEmpty(targetAnim))
                return;

            canMove = false;
            inAction = true;    
            animator.CrossFade(targetAnim, 0.2f);
            //rigidbody.velocity = Vector3.zero;
        }

        public void Tick(float dt)
        {
            delta = dt;
            onGround = OnGround();
            animator.SetBool("onGround", onGround);
        }

        public void HandleRolls()
        {
            if (rollInput == false) return;

            float v = vertical;
            float h = horizontal;
            v = moveAmount > 0.3f ? 1 : 0;
            h = 0;
            //if(!lockon)
            //{
            //    v = moveAmount > 0.3f ? 1 : 0;
            //    h = 0;
            //}
            //else
            //{
            //    if (Mathf.Abs(v) < 0.3f) v = 0;

            //    if (Mathf.Abs(h) < 0.3f) h = 0;
            //}

            if(v != 0)
            {
                if (moveDirection == Vector3.zero) moveDirection = transform.forward;
                Quaternion tr = Quaternion.LookRotation(moveDirection);
                transform.rotation = tr;
            }

            animationHook.rm_mul = rollSpeed;

            animator.SetFloat("vertical", v);
            animator.SetFloat("horizontal", h);

            canMove = false;
            inAction = true;
            animator.CrossFade("Rolls", 0.2f);
        }

        void HandleMovementAnimations()
        {
            animator.SetBool("run", run);
            animator.SetFloat("vertical", moveAmount, 0.24f, delta);
        }

        void HandleLockonAnimations(Vector3 moveDir)
        {
            Vector3 relatetiveDir = transform.InverseTransformDirection(moveDir);
            float h = relatetiveDir.x;
            float v = relatetiveDir.z;

            animator.SetFloat("vertical", v, 0.2f, delta);
            animator.SetFloat("horizontal", h, 0.2f, delta);
        }

        void HandleRotation()
        {
            Vector3 targetDir = lockon ? lockonTarget.transform.position - transform.position : moveDirection;
            targetDir.y = 0;
            if (targetDir == Vector3.zero)
                targetDir = transform.forward;
            Quaternion tr = Quaternion.LookRotation(targetDir);
            Quaternion targetRotation = Quaternion.Slerp(transform.rotation, tr, delta * moveAmount * rotateSpeed);
            transform.rotation = targetRotation;
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
    
        public void TwoHandHandler()
        {
            animator.SetBool("two_hand", twoHand);
        }
    }
}
