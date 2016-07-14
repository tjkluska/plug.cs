﻿using System;
using System.Linq;
using System.Linq.Expressions;

namespace Plug.Core
{
    public delegate object ActivatorInstance(params object[] args);

    public static class ObjectActivator
    {
        public static ActivatorInstance GetInstance(Type instanceType)
        {
            var constructorInfo = instanceType.GetConstructors().First();
            var parameters = constructorInfo.GetParameters();

            var parameterExpression = Expression.Parameter(typeof(object[]), "args");
            var argumentsExpression = new Expression[parameters.Length];

            for (var i = 0; i < parameters.Length; i++)
            {
                var index = Expression.Constant(i);
                var parameterType = parameters[i].ParameterType;

                var accessorExpression = Expression.ArrayIndex(parameterExpression, index);
                var castExpression = Expression.Convert(accessorExpression, parameterType);

                argumentsExpression[i] = castExpression;
            }

            var instantiator = Expression.New(constructorInfo, argumentsExpression);
            var lambdaExpression = Expression.Lambda(typeof(ActivatorInstance), instantiator, parameterExpression);
           
            return (ActivatorInstance) lambdaExpression.Compile();
        }
    }
}
