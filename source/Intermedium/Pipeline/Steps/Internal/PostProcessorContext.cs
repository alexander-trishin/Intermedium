namespace Intermedium.Pipeline.Steps.Internal
{
    internal sealed class PostProcessorContext<TResult> : IPostProcessorContext<TResult>
    {
        public TResult Result { get; set; }
    }
}
