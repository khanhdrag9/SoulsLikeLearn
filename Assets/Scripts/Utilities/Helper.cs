using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{
    public class Helper : MonoBehaviour
    {
        [Range(-1f, 1f)] public float vertical;
        [Range(-1f, 1f)] public float horizontal;
        public string[] oh_attacks;
        public string[] th_attacks;

        [Header("Options")]
        public bool twoHand;

        [Header("Buttons")]
        public bool attack;
        public bool useItem;
        public bool lockOn;
        
        private bool enableRM;
        private bool interacting;

        private Animator animator;

        void Start()
        {
            animator = GetComponent<Animator>();
        }

        void Update()
        {
            enableRM = !animator.GetBool("canMove");
            animator.applyRootMotion = enableRM;

            if(!lockOn)
            {
                horizontal = 0;
                vertical = Mathf.Clamp01(vertical);
            }

            animator.SetBool("lockon", lockOn);

            if (enableRM) return;

            if(useItem)
            {
                UseItem();
                useItem = false;
            }

            interacting = animator.GetBool("interacting");
            if (interacting)
            {
                attack = false;
                vertical = Mathf.Clamp(vertical, 0, .5f);
                twoHand = false;
            }

            animator.SetBool("two_hand", twoHand);
            if(attack)
            {
                Attack();
                attack = false;
            }

            animator.SetFloat("vertical", vertical);
            animator.SetFloat("horizontal", horizontal);
        }

        private void UseItem()
        {
            animator.Play("use_item");
        }

        private void Attack()
        {
            string targetanim;

            if (!twoHand)
            {
                int r = Random.Range(0, oh_attacks.Length);
                targetanim = oh_attacks[r];

                if (vertical > 0.5f) targetanim = "oh_attack_3";
            }
            else
            {
                int r = Random.Range(0, th_attacks.Length);
                targetanim = th_attacks[r];
            }

            vertical = 0;
            animator.CrossFade(targetanim, 0.5f);
        }
    }
}
