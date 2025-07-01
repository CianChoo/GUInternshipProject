using Unity.Properties;

namespace States
{
    public class ReadyState : IWhisperStates
    {
        private RunWhisper context;

        public ReadyState(RunWhisper context)
        {
            this.context = context;
        }

        public void Enter()
        {
            context.transcribe = false;
        }

        public void Exit()
        {
            
        }

        public void Tick()
        {
            
        }
        
    }
}