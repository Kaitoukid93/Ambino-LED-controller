using Ninject.Extensions.Factory;
using Ninject.Parameters;
using System.Reflection;
using Ninject;
using Ninject.Extensions.Factory.Factory;
using System.Linq;

namespace adrilight.Ninject
{
    public class UseFirstArgumentAsNameInstanceProvider : StandardInstanceProvider
    {
        protected override string GetName(System.Reflection.MethodInfo methodInfo, object[] arguments)
        {
            return (string)arguments[0];
        }

        protected override IConstructorArgument[] GetConstructorArguments(MethodInfo methodInfo, object[] arguments)
        {
            return base.GetConstructorArguments(methodInfo, arguments).Skip(1).ToArray();
        }
    }
}