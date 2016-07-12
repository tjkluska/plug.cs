﻿using System;
using System.Linq;
using Plug.Factories;
using Plug.Exceptions;
using System.Collections.Concurrent;

namespace Plug
{
    public class Container
    {
        /// <summary>
        /// Private field for storing registrations in the current container
        /// </summary>
        private readonly ConcurrentDictionary<Type, Registration> registrations;

        public Container()
        {
            var concurrencyLevel = Environment.ProcessorCount * 2;

            registrations = new ConcurrentDictionary<Type, Registration>(concurrencyLevel, 0);
        }

        /// <summary>
        /// Register a new dependency
        /// </summary>
        /// <param name="registrationType">The dependency type of this registration (the interface type)</param>
        /// <param name="instanceType">The instance type of this registration</param>
        /// <param name="factory">The factory responsible for resolving this registration</param>
        public void Register(Type registrationType, Type instanceType, IFactory factory)
        {
            var isRegistered = registrations.TryAdd(registrationType, new Registration(registrationType, instanceType, factory));

            if (!isRegistered)
            {
                throw new DuplicateRegistrationException(registrationType);
            }
        }

        /// <summary>
        /// Remove a registration from the container
        /// </summary>
        /// <param name="registrationType">The dependency type of this registration (the interface type)</param>
        /// <returns></returns>
        public Registration Remove(Type registrationType)
        {
            Registration registration;
            registrations.TryRemove(registrationType, out registration);
            return registration;
        }

        /// <summary>
        /// Remove a registration from the container
        /// </summary>
        /// <typeparam name="T">The dependency type of this registration (the interface type)</typeparam>
        /// <returns></returns>
        public Registration Remove<T>()
        {
            return Remove(typeof(T));
        }

        /// <summary>
        /// Register a new dependency
        /// </summary>
        /// <typeparam name="T">The dependency type of this registration (the interface type)</typeparam>
        /// <param name="instanceType">The instance type of this registration</param>
        /// <param name="factory">The factory responsible for resolving this registration</param>
        public void Register<T>(Type instanceType, IFactory factory)
        {
            Register(typeof(T), instanceType, factory);
        }

        /// <summary>
        /// Register a new dependency
        /// </summary>
        /// <typeparam name="TD">The dependency type of this registration (the interface type)</typeparam>
        /// <typeparam name="TI">The instance type of this registration</typeparam>
        /// <param name="factory">The factory responsible for resolving this registration</param>
        public void Register<TD, TI>(IFactory factory)
        {
            // Call parent method
            Register<TD>(typeof(TI), factory);
        }

        /// <summary>
        /// Get the registration for the specified type
        /// </summary>
        /// <param name="registrationType">The dependency type of the registration (the interface type)</param>
        /// <returns></returns>
        private Registration getRegistration(Type registrationType)
        {
            Registration registration;

            var isRegistered = registrations.TryGetValue(registrationType, out registration);

            if (!isRegistered || registration == null)
            {
                throw new NotRegisteredException(registrationType);
            }

            return registration;
        }

        /// <summary>
        /// Resolve an instance of the specified type
        /// </summary>
        /// <param name="registrationType">The dependency type of the registration (the interface type)</param>
        /// <returns>The instance of the registration generated by it's assigned factory</returns>
        public object Resolve(Type registrationType)
        {
            var registration = getRegistration(registrationType);
            return registration.Resolve();
        }

        /// <summary>
        /// Resolve an instance of the specified type
        /// </summary>
        /// <typeparam name="T">The dependency type of the registration (the interface type)</typeparam>
        /// <returns>The instance of the registration generated by it's assigned factory</returns>
        public T Resolve<T>()
        {
            return (T) Resolve(typeof(T));
        }
    }
}
