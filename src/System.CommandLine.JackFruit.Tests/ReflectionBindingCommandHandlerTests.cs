﻿// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using FluentAssertions;
using Xunit;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.CommandLine.Invocation;
using System.Threading.Tasks;
using System.Reflection;
using System.CommandLine.JackFruit.Tests.MethodModel;
using System.CommandLine.Builder;

namespace System.CommandLine.JackFruit.Tests
{
    public class ReflectionCommandHandlerTests
    {
        private readonly TestConsole _console;
        private readonly TestProgram _testProgram;
        private readonly Command testParent;


        public ReflectionCommandHandlerTests()
        {
            _console = new TestConsole();
            _testProgram = new TestProgram();
            var helpFinder = PreBinderContext.Current.DescriptionProvider;
            helpFinder.AddStrategy<object>((c, s) => (false, DescriptionFinder.Description(s)));
            helpFinder.AddStrategy<object>((c, s) => (false, HybridModelDescriptionFinder.Description(s)));
            testParent = new Command("test");
        }

        [Fact]
        public void Can_add_and_use_parameterInfo_valueFunc_bindings()
        {
            var type = typeof(Fruit);
            var methodInfo = type.GetMethod(nameof(Fruit.Bowl));
            var handler = ReflectionCommandHandler.Create<Fruit>(methodInfo);
            handler.Should().NotBeNull();
            handler.AddBinding(FuncBindingAction<Fruit>.Create( methodInfo.GetParameters()[0], () => "Water"));
            handler.AddBinding(FuncBindingAction<Fruit>.Create( methodInfo.GetParameters()[1], () => true));
            handler.AddBinding(FuncBindingAction<Fruit>.Create( methodInfo.GetParameters()[2], () => 1_000_000));
            handler.AddBinding(FuncBindingAction<Fruit>.Create( methodInfo.GetParameters()[3], () => "Cavendish"));
            handler.BindActions.Should().HaveCount(4);
            CheckFuncBindAction<Fruit>(handler.BindActions[0], methodInfo.GetParameters()[0], typeof(string));
            CheckFuncBindAction<Fruit>(handler.BindActions[1], methodInfo.GetParameters()[1], typeof(bool));
            CheckFuncBindAction<Fruit>(handler.BindActions[2], methodInfo.GetParameters()[2], typeof(int));
            CheckFuncBindAction<Fruit>(handler.BindActions[3], methodInfo.GetParameters()[3], typeof(string));
            var task = handler.InvokeAsync(null);
            task.Should().NotBeNull();
            Fruit.Captured.Should().Be(@"Melon = Water
Berry = True
Mango = 1000000
Banana = Cavendish");
        }

        [Fact]
        public void Can_add_and_use_parameterInfo_option_bindings()
        {
            var type = typeof(Fruit);
            var methodInfo = type.GetMethod(nameof(Fruit.Bowl));
            var handler = ReflectionCommandHandler.Create<Fruit>(methodInfo);
            handler.Should().NotBeNull();

            var command = new RootCommand();
            command.Handler = handler;
            var melonOption = new Option("melon", "", new Argument<string>());
            var berryOption = new Option("berry", "", new Argument<bool>());
            var mangoOption = new Option("mango", "", new Argument<int>());
            var bananaOption = new Option("banana", "", new Argument<string>());
            handler.AddBinding(SymbolBindingAction.Create(methodInfo.GetParameters()[0], melonOption));
            handler.AddBinding(SymbolBindingAction.Create(methodInfo.GetParameters()[1], berryOption));
            handler.AddBinding(SymbolBindingAction.Create(methodInfo.GetParameters()[2], mangoOption));
            handler.AddBinding(SymbolBindingAction.Create(methodInfo.GetParameters()[3], bananaOption));
            command.AddOptions(new Option[] { melonOption, berryOption, mangoOption, bananaOption });

            handler.BindActions.Should().HaveCount(4);
            CheckOptionAction(handler.BindActions[0], methodInfo.GetParameters()[0].Name, methodInfo.GetParameters()[0], typeof(string));
            CheckOptionAction(handler.BindActions[1], methodInfo.GetParameters()[1].Name, methodInfo.GetParameters()[1], typeof(bool));
            CheckOptionAction(handler.BindActions[2], methodInfo.GetParameters()[2].Name, methodInfo.GetParameters()[2], typeof(int));
            CheckOptionAction(handler.BindActions[3], methodInfo.GetParameters()[3].Name, methodInfo.GetParameters()[3], typeof(string));

            var parser = new CommandLineBuilder(command).Build();
            var parseResult = parser.Parse("melon Cantalope berry true mango 62 banana ohYeah");
            var task = parser.InvokeAsync(parseResult);

            task.Should().NotBeNull();
            const string expected = @"Melon = Cantalope
Berry = True
Mango = 62
Banana = ohYeah";
            Fruit.Captured.Should().Be(expected);
        }

        [Fact]
        public void Can_add_and_use_propertyInfo_valueFunc_bindings()
        {
            var type = typeof(FruitType);
            var handler = ReflectionCommandHandler.Create<FruitType>();
            handler.Should().NotBeNull();
            handler.AddBinding(FuncBindingAction<FruitType>.Create(type.GetProperties()[0], () => "Water"));
            handler.AddBinding(FuncBindingAction<FruitType>.Create(type.GetProperties()[1], () => true));
            handler.AddBinding(FuncBindingAction<FruitType>.Create(type.GetProperties()[2], () => 1_000_000));
            handler.AddBinding(FuncBindingAction<FruitType>.Create(type.GetProperties()[3], () => "Cavendish"));
            handler.BindActions.Should().HaveCount(4);
            CheckFuncBindAction<FruitType>(handler.BindActions[0], type.GetProperties()[0], typeof(string));
            CheckFuncBindAction<FruitType>(handler.BindActions[1], type.GetProperties()[1], typeof(bool));
            CheckFuncBindAction<FruitType>(handler.BindActions[2], type.GetProperties()[2], typeof(int));
            CheckFuncBindAction<FruitType>(handler.BindActions[3], type.GetProperties()[3], typeof(string));
            var task = handler.InvokeAsync(null);
            task.Should().NotBeNull();
            FruitType.Captured.Should().Be(@"Melon = Water
Berry = True
Mango = 1000000
Banana = Cavendish");
        }

        [Fact]
        public void Can_add_and_use_propertyInfo_option_bindings()
        {
            var type = typeof(FruitType);
            var handler = ReflectionCommandHandler.Create<FruitType>();
            handler.Should().NotBeNull();

            var command = new RootCommand();
            command.Handler = handler;
            var melonOption = new Option("melon", "", new Argument<string>());
            var berryOption = new Option("berry", "", new Argument<bool>());
            var mangoOption = new Option("mango", "", new Argument<int>());
            var bananaOption = new Option("banana", "", new Argument<string>());
            handler.AddBinding(SymbolBindingAction.Create(type.GetProperties()[0], melonOption));
            handler.AddBinding(SymbolBindingAction.Create(type.GetProperties()[1], berryOption));
            handler.AddBinding(SymbolBindingAction.Create(type.GetProperties()[2], mangoOption));
            handler.AddBinding(SymbolBindingAction.Create(type.GetProperties()[3], bananaOption));
            command.AddOptions(new Option[] { melonOption, berryOption, mangoOption, bananaOption });

            handler.BindActions.Should().HaveCount(4);
            CheckOptionAction(handler.BindActions[0], type.GetProperties()[0].Name, type.GetProperties()[0], typeof(string));
            CheckOptionAction(handler.BindActions[1], type.GetProperties()[1].Name, type.GetProperties()[1], typeof(bool));
            CheckOptionAction(handler.BindActions[2], type.GetProperties()[2].Name, type.GetProperties()[2], typeof(int));
            CheckOptionAction(handler.BindActions[3], type.GetProperties()[3].Name, type.GetProperties()[3], typeof(string));

            var parser = new CommandLineBuilder(command).Build();
            var parseResult = parser.Parse("melon Cantalope berry true mango 62 banana ohYeah");
            var task = parser.InvokeAsync(parseResult);

            task.Should().NotBeNull();
            const string expected = @"Melon = Cantalope
Berry = True
Mango = 62
Banana = ohYeah";
            Fruit.Captured.Should().Be(expected);
        }

        private void CheckArgumentAction(SymbolBindingAction bindAction,
                 ParameterInfo parameterInfo, Type returnType)
        {
            bindAction.Should().NotBeNull();
            bindAction.ReflectionThing.Should().Be(parameterInfo);
            bindAction.Symbol.Should().NotBeNull();
            var argument = bindAction.Symbol as Argument;
            argument.Should().NotBeNull();
            argument.Name.Should().BeEquivalentTo(parameterInfo.Name);
            argument.ArgumentType.Should().Be(parameterInfo.ParameterType);
            bindAction.ReturnType.Should().Be(returnType);
        }

        private void CheckOptionAction(BindingActionBase  bindAction,
                string name, object reflectionThing, Type returnType)
        {
            bindAction.Should().NotBeNull();
            var optionBindAction = bindAction as SymbolBindingAction;
            optionBindAction.Should().NotBeNull();
            optionBindAction.ReflectionThing.Should().Be(reflectionThing);
            optionBindAction.Symbol.Should().NotBeNull();
            var option = optionBindAction.Symbol as Option;
            option.Should().NotBeNull();
            option.Name.Should().BeEquivalentTo(name);
            if (returnType == typeof(bool))
            {
                // backwards but effective
                new Type[] { null, typeof(bool) }.Should().Contain(option.Argument.ArgumentType);
            }
            else
            {
                option.Argument.ArgumentType.Should().Be(returnType);
            }
            bindAction.ReturnType.Should().Be(returnType);
        }

        private void CheckFuncBindAction<T>(BindingActionBase bindAction,
                object reflectionThing, Type returnType)
            where T : class
        {
            bindAction.Should().NotBeNull();
            var funcBindAction = bindAction as FuncBindingAction<T>;
            funcBindAction.Should().NotBeNull();
            funcBindAction.ReflectionThing.Should().Be(reflectionThing);
            funcBindAction.ValueFunc.Should().NotBeNull();
            funcBindAction.ReturnType.Should().Be(returnType);

        }
    }
}