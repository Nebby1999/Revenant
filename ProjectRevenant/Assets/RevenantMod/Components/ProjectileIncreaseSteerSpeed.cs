using RoR2.Projectile;
using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RevenantMod
{
    [RequireComponent(typeof(ProjectileSteerTowardTarget))]
    public class ProjectileIncreaseSteerSpeed : MonoBehaviour
    {
        public ProjectileSteerTowardTarget projectileSteerTowardTarget { get; private set; }

        [SerializeField]
        private float addedSpeedPerSecond;

        private void Start()
        {
            if (!NetworkServer.active)
            {
                base.enabled = false;
                return;
            }
            projectileSteerTowardTarget = GetComponent<ProjectileSteerTowardTarget>();
        }

        private void FixedUpdate()
        {
            projectileSteerTowardTarget.rotationSpeed += addedSpeedPerSecond * Time.fixedDeltaTime;
        }
    }
}