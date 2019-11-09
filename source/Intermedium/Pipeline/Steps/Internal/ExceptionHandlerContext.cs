using System;

namespace Intermedium.Pipeline.Steps.Internal
{
    internal sealed class ExceptionHandlerContext : IExceptionHandlerContext, IDisposable
    {
        private readonly IExceptionHandlerContext<VoidUnit> _context;

        public ExceptionHandlerContext(IExceptionHandlerContext<VoidUnit> context)
        {
            _context = context;
        }

        public bool Handled { get; set; }

        public void Dispose()
        {
            if (Handled)
            {
                _context.ExceptionWasHandled(VoidUnit.Value);
            }
        }
    }

    internal sealed class ExceptionHandlerContext<TResponse> : IExceptionHandlerContext<TResponse>
    {
        public bool Handled { get; private set; }

        public TResponse Result { get; private set; }

        public void ExceptionWasHandled(TResponse response)
        {
            Result = response;
            Handled = true;
        }
    }
}
