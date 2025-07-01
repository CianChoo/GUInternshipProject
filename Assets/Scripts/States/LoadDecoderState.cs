using Unity.InferenceEngine;
using UnityEngine;

namespace States
{
    public class LoadDecoderState : IWhisperStates
    {
        private RunWhisper context;

        public LoadDecoderState(RunWhisper context)
        {
            this.context = context;
        }

        public void Enter()
        {
            Debug.Log("Loading Decoder Models");
            
            context.decoder1 = new Worker(ModelLoader.Load(context.audioDecoder1), BackendType.GPUCompute);
            context.decoder2 = new Worker(ModelLoader.Load(context.audioDecoder2), BackendType.GPUCompute);
            
            FunctionalGraph graph = new FunctionalGraph();
            var input = graph.AddInput(DataType.Float, new DynamicTensorShape(1, 1, 51865));
            var amax = Functional.ArgMax(input, -1, false);
            var selectTokenModel = graph.Compile(amax);
            context.argmax = new Worker(selectTokenModel, BackendType.GPUCompute);
            
            context.TransitionToState(new LoadEncoderState(context));
        }

        public void Exit()
        {
            
        }

        public void Tick()
        {
            
        }
        
    }
}