using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{
    public class CameraManager : MonoBehaviour
    {
        public bool lockon;
        public float followSpeed = 9f;
        public float mouseSpeed = 2f;
        public float controllerSpeed = 7f;
        public Transform target;


        public Transform pivot { get; set; }
        public Transform camTrans { get; set; }

        private float turnSmoothing = .1f;
        public float minAngle = -35;
        public float maxAngle = 35;

        private float smoothX;
        private float smoothY;
        private float smoothXVec;
        private float smoothYVec;

        public float lookAngle;
        public float titlAngle;

        public void Init(Transform t)
        {
            target = t;
            camTrans = Camera.main.transform;
            pivot = camTrans.parent;
        }

        public void Tick(float dt)
        {
            float h = Input.GetAxis("Mouse X");
            float v = Input.GetAxis("Mouse Y");

            float targetSpeed = mouseSpeed;

            float c_h = Input.GetAxis("RightAxis X");
            float c_v = Input.GetAxis("RightAxis Y");

            if (c_h != 0 || c_v != 0)
            {
                h = c_h;
                v = c_v;
                targetSpeed = controllerSpeed;
            }


            FollowTarget(dt);
            HandleRotations(dt, v, h, targetSpeed);
        }

        void FollowTarget(float dt)
        {
            float speed = dt * followSpeed;
            Vector3 targetPosition = Vector3.Lerp(transform.position, target.position, speed);
            transform.position = targetPosition;
        }

        void HandleRotations(float dt, float v, float h, float targetSpeed)
        {
            if(turnSmoothing > 0)
            {
                smoothX = Mathf.SmoothDamp(smoothX, h, ref smoothXVec, turnSmoothing);
                smoothY = Mathf.SmoothDamp(smoothY, v, ref smoothYVec, turnSmoothing);
            }
            else
            {
                smoothX = h;
                smoothY = h;
            }

            if(lockon)
            {

            }

            lookAngle += smoothX * targetSpeed;
            transform.rotation = Quaternion.Euler(0, lookAngle, 0);

            titlAngle += smoothY * targetSpeed;
            titlAngle = Mathf.Clamp(titlAngle, minAngle, maxAngle);
            pivot.localRotation = Quaternion.Euler(titlAngle, 0, 0);
        }

        public static CameraManager Instance;
        void Awake()
        {
            Instance = this;
        }
    }
}
