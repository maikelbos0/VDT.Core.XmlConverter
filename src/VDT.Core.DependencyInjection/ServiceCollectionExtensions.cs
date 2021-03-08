﻿using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;
using VDT.Core.DependencyInjection.Decorators;

namespace VDT.Core.DependencyInjection {
    public static class ServiceCollectionExtensions {
        public static IServiceCollection AddAttributeServices(this IServiceCollection services, Assembly assembly) {
            return services;
        }

        public static IServiceCollection AddAttributeServices(this IServiceCollection services, Assembly assembly, Action<DecoratorOptions> setupAction) {
            return services;
        }
    }
}