using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace DefaultNamespace
{
    public class StepManagerCustom : MonoBehaviour
    {

        public Transform decisionParent;
        private int currentIndex = 0;

        private void Start()
        {
            foreach (Transform child in decisionParent)
            {
                child.gameObject.SetActive(false);
            }

            if (decisionParent.childCount > 0)
            {
                decisionParent.GetChild(0).gameObject.SetActive(true);
            }
        }

        public void ShowNextDecision()
        {
            if (currentIndex < decisionParent.childCount)
            {
                decisionParent.GetChild(currentIndex).gameObject.SetActive(false);
            }

            currentIndex++;

            if (currentIndex < decisionParent.childCount)
            {
                decisionParent.GetChild(currentIndex).gameObject.SetActive(true);
            }
            else
            {
                Debug.Log("No more decisions");
            }
        }
        
    }
}