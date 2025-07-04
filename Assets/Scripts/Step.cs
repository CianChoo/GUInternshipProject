using System;
using UnityEngine;

namespace DefaultNamespace
{
    [Serializable]
    public class Step
    {
        public GameObject stepObject;
        public string buttonText;

        public Step(GameObject stepObject, string buttonText)
        {
            this.stepObject = stepObject;
            this.buttonText = buttonText;
        }
        
    }
}