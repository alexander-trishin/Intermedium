namespace Intermedium.Pipeline.Steps.Internal
{
    internal sealed class PostProcessorContext<TResult> : IPostProcessorContext<TResult>
    {
#pragma warning disable CS8618
        public TResult Result { get; set; }
#pragma warning restore CS8618
    }
}
