using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Attributes;
using RPG.Core;
using RPG.Saving;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

namespace RPG.Movement
{
    public class Mover : MonoBehaviour,IAction,ISaveable
    {
        [SerializeField] private Transform targetTransform;
        [SerializeField] private float maxPathLength = 40f;
        private NavMeshAgent _playerNavMesh;
        private Animator _characterAnimator;
        private Health _health;
        
        

        private void Awake()
        {
            _playerNavMesh = GetComponent<NavMeshAgent>();
            _characterAnimator = GetComponent<Animator>();
            _health = GetComponent<Health>();
        }
        
        void Update()
        {
            _playerNavMesh.enabled = !_health.IsDead();
            
            UpdateAnimator();
        }
        
        // This Method Interrupts Combat and Starts Movement
        // Special for Combat -> Movement Transition.
        
        // For Cancelling Attack Operation,
        // There will be no need to reach Fighter script after using IAction Interface. Line 36. (Dependency Inversion)
        // RPG.Combat namespace will not be necessary.
        public void StartMoveAction(Vector3 destination)
        {
            GetComponent<ActionScheduler>().StartAction(this);
            //GetComponent<Fighter>().Cancel(); (inefficient line after dependency inversion)
            MoveTo(destination);
        }

        public bool CanMoveTo(Vector3 destination)
        {
            NavMeshPath path = new NavMeshPath();
            bool hasPath = NavMesh.CalculatePath(transform.position, destination, NavMesh.AllAreas, path);

            if (!hasPath) return false;
            if (path.status != NavMeshPathStatus.PathComplete) return false;
            if (GetPathLength(path) > maxPathLength) return false;

            return true;
        }
        
        // General Movement Method
        // NavMesh Motion Switches
        public void MoveTo(Vector3 destination)
        {
            _playerNavMesh.destination = destination;
            _playerNavMesh.isStopped = false;
        }

        public void Cancel()
        {
            _playerNavMesh.isStopped = true;
        }
        
        // Animation Velocity Equalized to NavMesh Velocity
        // InverseTransformDirection turns Worldspace (Global) Velocity to Local Velocity relative to NavMesh Agent.
        
        private void UpdateAnimator()
        {
            Vector3 characterVelocity = _playerNavMesh.velocity;
            Vector3 localVelocity = transform.InverseTransformDirection(characterVelocity);
            float speed = localVelocity.z;

            _characterAnimator.SetFloat("ForwardSpeed", speed);

        }
        
        private float GetPathLength(NavMeshPath path)
        {
            float total = 0;

            if (path.corners.Length < 2)
            {
                return total;
            }

            for (int i = 0; i < path.corners.Length-1; i++)
            {
                total += Vector3.Distance(path.corners[i], path.corners[i + 1]);
            }

            return total;
        }
        
        public object CaptureState()
        {
            return new SerializableVector3(transform.position);
        }

        public void RestoreState(object state)
        {
            SerializableVector3 position = (SerializableVector3) state;
            _playerNavMesh.enabled = false;
            transform.position = position.ToVector();
            _playerNavMesh.enabled = true;
        }
    }
}
