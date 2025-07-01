using Unity.Collections;
using Unity.InferenceEngine;
using UnityEngine;

namespace States
{
    public class LoadSpectroState : IWhisperStates
    {
        private RunWhisper context;

        public LoadSpectroState(RunWhisper context)
        {
            this.context = context;
        }

        public void Enter()
        {
            Debug.Log("Entering LoadSpectroState");
            
            context.spectrogram = new Worker(ModelLoader.Load(context.logMelSpectro), BackendType.GPUCompute);
            
            context.TransitionToState(new StartTranscriptionState(context));
        }

        public void Exit()
        {
            
        }

        public void Tick()
        {
            
        }
        
    }
}