using System;
using Castle.DynamicProxy;
using NSubstitute.Core;

namespace NSubstitute.Proxies.CastleDynamicProxy
{
    public class CastleForwardingInterceptor : IInterceptor
    {
        private readonly CastleInvocationMapper _invocationMapper;
        private readonly ICallRouter _callRouter;
        private bool _fullDispatchMode;

        public CastleForwardingInterceptor(CastleInvocationMapper invocationMapper, ICallRouter callRouter)
        {
            _invocationMapper = invocationMapper ?? throw new ArgumentNullException(nameof(invocationMapper));
            _callRouter = callRouter ?? throw new ArgumentNullException(nameof(callRouter));
        }

        public void Intercept(IInvocation invocation)
        {
            var mappedInvocation = _invocationMapper.Map(invocation);

            if (_fullDispatchMode)
            {
                invocation.ReturnValue = _callRouter.Route(mappedInvocation);
                return;
            }

            // Fallback to the base value until the full dispatch mode is activated.
            // Useful to ensure that object is initialized properly.
            if (_callRouter.CallBaseByDefault)
            {
                invocation.ReturnValue = mappedInvocation.TryCallBase().ValueOrDefault();
            }
        }

        /// <summary>
        /// Switches interceptor to dispatch calls via the full pipeline.
        /// </summary>
        public void SwitchToFullDispatchMode()
        {
            _fullDispatchMode = true;
        }
    }


    public class ProxyCastleForwardingInterceptor : IInterceptor
    {
        private readonly ICallFactory _callFactory;
        private readonly IArgumentSpecificationDequeue _argSpecificationDequeue;
        private readonly ICallRouter _callRouter;
        private bool _fullDispatchMode;

        public ProxyCastleForwardingInterceptor(ICallFactory callFactory,
            IArgumentSpecificationDequeue argSpecificationDequeue, ICallRouter callRouter)
        {
            _callFactory = callFactory;
            _argSpecificationDequeue = argSpecificationDequeue;
            _callRouter = callRouter ?? throw new ArgumentNullException(nameof(callRouter));
        }


        public virtual ICall Map(IInvocation castleInvocation)
        {
            Func<object> baseMethod = null;
            if (castleInvocation.InvocationTarget != null)
            {
                Func<object> baseResult = () =>
                {
                    castleInvocation.Proceed();
                    return castleInvocation.ReturnValue;
                };
                var result = new Lazy<object>(baseResult);
                baseMethod = () => result.Value;
            }

            var queuedArgSpecifications =
                _argSpecificationDequeue.DequeueAllArgumentSpecificationsForMethod(castleInvocation.Method);
            return _callFactory.Create(castleInvocation.Method, castleInvocation.Arguments, castleInvocation.Proxy,
                queuedArgSpecifications, baseMethod);
        }

        public void Intercept(IInvocation invocation)
        {
            var mappedInvocation = Map(invocation);

            if (_fullDispatchMode)
            {
                invocation.ReturnValue = _callRouter.Route(mappedInvocation);
                return;
            }

            // Fallback to the base value until the full dispatch mode is activated.
            // Useful to ensure that object is initialized properly.
            if (_callRouter.CallBaseByDefault)
            {
                invocation.ReturnValue = mappedInvocation.TryCallBase().ValueOrDefault();
            }
        }

        /// <summary>
        /// Switches interceptor to dispatch calls via the full pipeline.
        /// </summary>
        public void SwitchToFullDispatchMode()
        {
            _fullDispatchMode = true;
        }
    }
}