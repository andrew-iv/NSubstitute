using System;

namespace NSubstitute.Core
{
    public interface IProxyFactory
    {
        object GenerateProxy(ICallRouter callRouter, Type typeToProxy, Type[] additionalInterfaces, object[] constructorArguments);
        object GenerateProxyForTarget(ICallRouter callRouter, Type typeToProxy, Type[] additionalInterfaces, object target);
    }
}