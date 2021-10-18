namespace Intermedium.Pipeline
{
    /// <summary>
    /// A processor that specifies the relative order it should run.
    /// </summary>
    public interface IOrderedProcessor
    {
        /// <summary>
        /// Gets the order value for determining the order of execution of processors.
        /// Processors execute in ascending numeric value of the <see cref="Order"/> property.
        /// </summary>
        int Order { get; init; }
    }
}
