using UnityEngine;
using Unity.InferenceEngine;

namespace States
{
    public class LoadEncoderState : IWhisperStates
    {

        private RunWhisper context;

        public LoadEncoderState(RunWhisper context)
        {
            this.context = context;
        }

        public void Enter()
        {
            Debug.Log("Entering LoadEncoderState");

            context.encoder = new Worker(ModelLoader.Load(context.audioEncoder), BackendType.GPUCompute);
            
            context.TransitionToState(new LoadSpectroState(context));
        }

        public void Exit()
        {
            
        }

        public void Tick()
        {
            
        }
        
    }
    
}