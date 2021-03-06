﻿using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac;
using Module = Autofac.Module;

namespace Xunit.Ioc.Autofac
{
    /// <summary>
    /// This module allows you to easily register all your test classes within the container
    /// </summary>
    /// <remarks>
    /// It searches for all test classes with the <see cref="RunWithAttribute"/> set to use
    /// the <see cref="IocTestClassCommand"/> and automatically adds them to the container.
    /// </remarks>
    public class TestsModule : Module
    {
        private readonly IEnumerable<Assembly> _assemblies;

        /// <param name="assembly">The assembly to search for test classes</param>
        public TestsModule(Assembly assembly)
            : this(new[] { assembly })
        {
        }

        /// <param name="assemblies">The assemblies to search for test classes</param>
        public TestsModule(IEnumerable<Assembly> assemblies)
        {
            _assemblies = assemblies;
        }

        /// <inheritdoc/>
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            var testClasses = from assembly in _assemblies
                              from type in assembly.GetTypes()
                              where type.IsClass && type.IsAbstract == false && type.IsGenericTypeDefinition == false
                              let runWithAttr = type.GetCustomAttributes(typeof(RunWithAttribute), false)
                                  .Cast<RunWithAttribute>()
                                  .FirstOrDefault()
                              where runWithAttr != null && runWithAttr.TestClassCommand == typeof(IocTestClassCommand)
                              select type;

            foreach (var testClass in testClasses)
                builder.RegisterType(testClass);
        }
    }
}
