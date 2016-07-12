﻿using System;
using Plug.Factories;
using Plug.Exceptions;

namespace Plug
{
    /// <summary>
    /// Base registration type to store in generic lists
    /// </summary>
    public class Registration
    {
        private DateTime lastResolutionDate;

        /// <summary>
        /// The factory responsible for resolving this registration
        /// </summary>
        public IFactory Factory { get; }

        /// <summary>
        /// A boxed reference to the current instance of the dependency
        /// </summary>
        public object Instance { get; set; }

        /// <summary>
        /// The type of this registration (the type of the interface)
        /// </summary>
        public Type RegistrationType { get; }

        /// <summary>
        /// The instance type of this registration
        /// </summary>
        public Type InstanceType { get; }

        /// <summary>
        /// The time (UTC) the last instance was resolved
        /// </summary>
        public DateTime LastResolutionDate
        {
            get { return lastResolutionDate; }
        }

        /// <summary>
        /// Validate the instance type to ensure it implements the dependency type.
        /// If not validated we can get an invalid cast exception when trying to unbox the instance
        /// </summary>
        /// <param name="registrationType">The dependency type of this registration (the interface type)</param>
        /// <param name="instanceType">The instance type of this registration</param>
        private void ValidateRegistration(Type registrationType, Type instanceType)
        {
            if (!registrationType.IsInterface)
            {
                throw new InvalidTypeException("Registration type must be an interface");
            }
            
            if (!instanceType.IsClass)
            {
                throw new InvalidTypeException("Instance type must be a class");
            }

            // Check to ensure the instance implements the interface
            if (!registrationType.IsAssignableFrom(instanceType))
            {
                throw new NotAssignableFromException(registrationType, instanceType);
            }
        }

        /// <summary>
        /// A class to store information about a dependency
        /// </summary>
        /// <param name="instanceType">The instance type of this registration</param>
        /// <param name="factory">The factory responsible for resolving this registration</param>
        public Registration(Type registrationType, Type instanceType, IFactory factory)
        {
            ValidateRegistration(registrationType, instanceType);

            Factory = factory;
            RegistrationType = registrationType;
            InstanceType = instanceType;
        }

        /// <summary>
        /// Resolve an instance of this registration
        /// </summary>
        /// <returns>The instance of the registration generated by the assigned factory</returns>
        public object Resolve()
        {
            Factory.Resolve(this);
            lastResolutionDate = DateTime.UtcNow;

            return Instance;
        }
    }
}
