using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Saving;
using UnityEngine;

namespace RPG.Stats
{
    public class Experience : MonoBehaviour,ISaveable
    {
        [SerializeField] private float experiencePoints = 0;

        public event Action OnExperienceGained;
        
        public void GainExperience(float experience)
        {
            experiencePoints += experience;
            OnExperienceGained?.Invoke();
        }
    
        public float GetPoints()
        {
            return experiencePoints;
        }
    
        public object CaptureState()
        {
            return experiencePoints;
        }

        public void RestoreState(object state)
        {
            experiencePoints = (float)state;
        }

    
    }
}

