using DefaultNamespace;
using UnityEngine;

namespace States
{
    public class RunDecoderState : IWhisperStates
    {
        private RunWhisper context;
        private SimpleAwaitable awaitable;
        private bool transcriptionComplete = false;
        public RunDecoderState(RunWhisper context)
        {
            this.context = context;
        }

        public void Enter()
        {
            Debug.Log("RunDecoderState entered");
            awaitable = context.TranscriptionLoop();
        }

        public void Exit()
        {
            
        }

        public void Tick()
        {
            if (!transcriptionComplete && awaitable != null)
            {
                if (awaitable.IsCompleted)
                {
                    transcriptionComplete = true;
                    Debug.Log("Transcription Complete; moving onto Ready stage");
                    context.TransitionToState(new ReadyState(context));
                }
            }
        }
    }
}