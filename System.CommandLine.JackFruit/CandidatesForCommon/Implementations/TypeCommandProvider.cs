﻿using System.Collections.Generic;
using System.CommandLine.Invocation;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace System.CommandLine.JackFruit
{
    public abstract class TypeCommandProvider : ICommandProvider<Type>
    {
        private readonly IHelpProvider<Type, PropertyInfo> helpProvider;
        private readonly IOptionProvider<Type, PropertyInfo> optionProvider;
        private readonly IArgumentProvider<Type, PropertyInfo> argumentProvider;
        private readonly IInvocationProvider invocationProvider;

        public TypeCommandProvider(
                    IDescriptionProvider<Type> descriptionProvider = null,
                    IHelpProvider<Type, PropertyInfo> helpProvider = null,
                    IOptionProvider<Type, PropertyInfo> optionProvider = null,
                    IArgumentProvider<Type, PropertyInfo> argumentProvider = null,
                    IInvocationProvider invocationProvider = null)
        {
            this.helpProvider = helpProvider
                                ?? new TypeHelpProvider(descriptionProvider);
            this.optionProvider = optionProvider
                                ?? new PropertyInfoOptionProvider();
            this.argumentProvider = argumentProvider
                                ?? new TypeArgumentProvider();
            this.invocationProvider = invocationProvider
                                ?? invocationProvider;

            this.optionProvider.HelpProvider = this.optionProvider.HelpProvider
                                ?? this.helpProvider;
            this.argumentProvider.HelpProvider = this.argumentProvider.HelpProvider
                                ?? this.helpProvider;
        }

        public IHelpProvider<Type> HelpProvider { get; set; }

        public Argument GetArgument(Type source)
        {
            return argumentProvider.GetArgument(source);
        }

        public Command GetCommand(Type currentType) 
            => FillCommand(currentType, new Command(name: GetName(currentType)));

        public RootCommand GetRootCommand(Type currentType) 
            => FillCommand(currentType, new RootCommand());

        private T FillCommand<T>(Type currentType, T command)
            where T : Command
        {
            command.Description = GetHelp(currentType);
            SetHandler(command, currentType);

            command.AddOptions(GetOptions(currentType));
            command.Argument = GetArgument(currentType);

            return command
                .AddCommands(GetSubCommands(currentType));
        }

        public string GetHelp(Type currentType)
        {
            var attribute = currentType.GetCustomAttribute<HelpAttribute>(); ;
            return Extensions.GetHelp(attribute, HelpProvider, currentType);
        }

        public string GetName(Type currentType)
            => currentType.Name;

        public IEnumerable<Option> GetOptions(Type currentType)
        {
            return currentType.GetProperties()
                .Where(p => argumentProvider.IsArgument(currentType, p))
                .Select(x => optionProvider.GetOption(currentType, x));
        }

        public IEnumerable<Command> GetSubCommands(Type currentType)
        {
            var subCommandTypes = GetSubCommandTypes(currentType);
            return subCommandTypes == null
                ? null
                : subCommandTypes
                    .Select(t => GetCommand(t));
        }

        protected abstract IEnumerable<Type> GetSubCommandTypes(Type currentType);

        private void SetHandler(Command command, Type currentType)
        {
            var bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic;
            var methodInfo = typeof(TypeCommandProvider).GetMethod(nameof(SetHandlerInternal), bindingFlags);
            var constructedMethod = methodInfo.MakeGenericMethod(currentType);
            constructedMethod.Invoke(this, new object[] { command });
        }

        private void SetHandlerInternal<TResult>(Command command)
        {
            Func<TResult, Task<int>> invocation = null;
            if (invocationProvider != null)
            {
                invocation = invocationProvider.InvokeAsyncFunc<TResult>();
            }
            else
            {
                var methodInfo = typeof(TResult).GetMethod("InvokeAsync");
                if (methodInfo != null)
                {
                    invocation = x => (Task<int>)methodInfo.Invoke(x, null);
                }
            }
            if (invocation != null)
            {
                Func<InvocationContext, Task<int>> invocationWrapper
                    = context => InvokeMethodWithResult(context, invocation);
                command.Handler = new SimpleCommandHandler(invocationWrapper);
            }
        }

        private Task<int> InvokeMethodWithResult<TResult>(InvocationContext context, Func<TResult, Task<int>> invocation)
        {
            var result = Activator.CreateInstance<TResult>();
            var binder = new TypeBinder(typeof(TResult));
            binder.SetProperties(context, result);
            return invocation(result);
        }

        private async Task<int> InvokeAsync(InvocationContext x,
            Func<Task<int>> invocation)
        {
            return await invocation();
        }
    }
}
