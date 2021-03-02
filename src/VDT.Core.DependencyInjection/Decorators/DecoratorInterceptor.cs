﻿using Castle.DynamicProxy;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace VDT.Core.DependencyInjection.Decorators {
    internal sealed class DecoratorInterceptor : IInterceptor {
        private readonly IDecorator decorator;
        private readonly Predicate<MethodInfo> predicate;

        internal DecoratorInterceptor(IDecorator decorator, Predicate<MethodInfo> predicate) {
            this.decorator = decorator;
            this.predicate = predicate;
        }

        public void Intercept(IInvocation invocation) {
            if (ShouldDecorate(invocation.Method)) {
                var context = new MethodExecutionContext(invocation.TargetType, invocation.InvocationTarget, invocation.Method, invocation.Arguments, invocation.GenericArguments);
                var method = invocation.Method;

                decorator.BeforeExecute(context);

                if (method.ReturnType == typeof(Task)) {
                    DecorateTask(invocation, context);
                }
                else if (method.ReturnType.IsGenericType && method.ReturnType.GetGenericTypeDefinition() == typeof(Task<>)) {
                    var decorator = typeof(DecoratorInterceptor).GetMethod(nameof(DecorateTaskWithResult), BindingFlags.NonPublic | BindingFlags.Instance)?
                        .MakeGenericMethod(context.Method.ReturnType.GetGenericArguments()) ?? throw new InvalidOperationException($"Method '{nameof(DecoratorInterceptor)}.{nameof(DecorateTaskWithResult)}' was not found.");

                    decorator.Invoke(this, new object[] { invocation, context });
                }
                else {
                    Decorate(invocation, context);
                }
            }
            else {
                invocation.Proceed();
            }
        }

        private void DecorateTask(IInvocation invocation, MethodExecutionContext context) {
            invocation.Proceed();

            invocation.ReturnValue = ((Func<Task>)(async () => {
                try {
                    await (Task)invocation.ReturnValue;
                }
                catch (Exception ex) {
                    decorator.OnError(context, ex);
                    throw;
                }

                decorator.AfterExecute(context);
            }))();
        }

        private void DecorateTaskWithResult<TResult>(IInvocation invocation, MethodExecutionContext context) {
            invocation.Proceed();

            invocation.ReturnValue = ((Func<Task<TResult>>)(async () => {
                var task = (Task<TResult>)invocation.ReturnValue;

                try {
                    await task;
                }
                catch (Exception ex) {
                    decorator.OnError(context, ex);
                    throw;
                }

                decorator.AfterExecute(context);

                return task.Result;
            }))();
        }

        private void Decorate(IInvocation invocation, MethodExecutionContext context) {
            try {
                invocation.Proceed();
            }
            catch (Exception ex) {
                decorator.OnError(context, ex);
                throw;
            }

            decorator.AfterExecute(context);
        }

        private bool ShouldDecorate(MethodInfo methodInfo) {
            return predicate(methodInfo);
        }
    }
}
