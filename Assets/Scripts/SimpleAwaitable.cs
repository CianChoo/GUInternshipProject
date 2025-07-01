namespace DefaultNamespace
{
    public class SimpleAwaitable
    {
        public bool IsCompleted { get; private set; }

        public void Complete()
        {
            IsCompleted = true;
        }

    }
}