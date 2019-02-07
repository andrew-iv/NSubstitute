using System;

namespace NSubstitute.Core
{
    public interface ISubstituteFactory
    {
        object Create(Type[] typesToProxy, object[] constructorArguments);

        /// <summary>
        /// Create an instance of the given types, with calls configured to call the base implementation
        /// where possible. Parts of the instance can be substituted using 
        /// <see cref="SubstituteExtensions.Returns{T}(T,T,T[])">Returns()</see>.
        /// </summary>
        /// <param name="typesToProxy"></param>
        /// <param name="constructorArguments"></param>
        /// <returns></returns>
        object CreatePartial(Type[] typesToProxy, object[] constructorArguments);

        object CreateProxy(Type[] typesToProxy, object target);
    }
}