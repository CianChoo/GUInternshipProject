using Unity.Collections;
using Unity.InferenceEngine;
using UnityEngine;

namespace States
{
    public class StartTranscriptionState : IWhisperStates
    {
        private RunWhisper context;

        public StartTranscriptionState(RunWhisper context)
        {
            this.context = context;
        }

        public void Enter()
        {
            Debug.Log("StartTranscriptionState");
            
            context.numSamples = context.audioClip.samples * context.audioClip.channels;
            
            context.SetupWhiteSpaceShifts();
            context.GetTokens();
            
            //Token Setup
            context.outputTokens = new NativeArray<int>(RunWhisper.maxTokens, Allocator.Persistent);
            
            context.outputTokens[0] = RunWhisper.START_OF_TRANSCRIPT;
            context.outputTokens[1] = RunWhisper.ENGLISH;// GERMAN;//FRENCH;//
            context.outputTokens[2] = RunWhisper.TRANSCRIBE; //TRANSLATE;//
            context.tokenCount = 3;

            context.tokensTensor = new Tensor<int>(new TensorShape(1, RunWhisper.maxTokens));
            ComputeTensorData.Pin(context.tokensTensor);
            
            context.tokensTensor.Reshape(new TensorShape(1, context.tokenCount));
            context.tokensTensor.dataOnBackend.Upload<int>(context.outputTokens, context.tokenCount);

            context.lastToken = new NativeArray<int>(1, Allocator.Persistent);
            context.lastToken[0] = RunWhisper.NO_TIME_STAMPS;
            context.lastTokenTensor = new Tensor<int>(new TensorShape(1, 1), new[] { RunWhisper.NO_TIME_STAMPS });
            
            context.LoadAudio();
            context.EncodeAudio();
            context.transcribe = true;
            
            context.TransitionToState(new RunDecoderState(context));
            
        }

        public void Exit()
        {
            
        }

        public void Tick()
        {
            
        }
    }
}