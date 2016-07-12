﻿using Plug.Factories;
using Plug.Helpers;
using System;

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

        public AppDomain Domain { get; }

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
        /// A class to store information about a dependency
        /// </summary>
        /// <param name="instanceType">The instance type of this registration</param>
        /// <param name="factory">The factory responsible for resolving this registration</param>
        public Registration(Type registrationType, Type instanceType, IFactory factory)
        {
            Validator.ValidateRegistration(registrationType, instanceType, factory);
                        
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

            Validator.ValidateInstance(Instance, InstanceType);

            return Instance;
        }

        ~Registration()
        {
            if (Instance != null && Instance is IDisposable)
            {
                ((IDisposable) Instance).Dispose();
            }
        } 
    }
}
