using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Serilog;

namespace Deployer.Execution
{
    public class ScriptRunner : IScriptRunner
    {
        private readonly IEnumerable<Type> typeUniverse;
        private readonly IInstanceBuilder instanceBuilder;
        private readonly IPathBuilder pathBuilder;

        public ScriptRunner(IEnumerable<Type> typeUniverse, IInstanceBuilder instanceBuilder, IPathBuilder pathBuilder)
        {
            this.typeUniverse = typeUniverse;
            this.instanceBuilder = instanceBuilder;
            this.pathBuilder = pathBuilder;
        }

        public async Task Run(Script script)
        {
            foreach (var sentence in script.Sentences)
            {
                await Run(sentence);
            }
        }

        private async Task Run(Sentence sentence)
        {
            var transformedSentence = await TransformSentence(sentence);

            var instance = BuildInstance(instanceBuilder, transformedSentence);
            var operationStr = GetOperationStr(transformedSentence.Command.Name, instance.GetType());
            var commandArguments = transformedSentence.Command.Arguments;
            Log.Information(string.Format(operationStr, commandArguments.Cast<object>().ToArray()));

            await instance.Execute();
        }

        private async Task<Sentence> TransformSentence(Sentence sentence)
        {
            var transformed = sentence.Command.Arguments.ToObservable().Select(x => Observable.FromAsync(async () =>
                    {
                        var val = x.Value;
                        if (val is string str)
                        {
                            return new Argument(await pathBuilder.Replace(str));
                        }

                        return new Argument(val);
                    }))
                    .Merge(1);
            
            var positionalArguments = await transformed.ToList();
            return new Sentence(new Command(sentence.Command.Name, positionalArguments));
        }

        private static string GetOperationStr(string commandName, Type type)
        {
            var description = type.GetTypeInfo().GetCustomAttribute<TaskDescriptionAttribute>()?.Text;

            if (description != null)
            {
                return description;
            }

            return "Executing " + commandName;
        }

        private IDeploymentTask BuildInstance(IInstanceBuilder builder, Sentence sentence)
        {
            try
            {
                var type = typeUniverse.Single(x => x.Name == sentence.Command.Name);
                var parameters = sentence.Command.Arguments.Select(x => x.Value);
                return (IDeploymentTask) builder.Create(type, parameters.ToArray());
            }
            catch (InvalidOperationException)
            {
                throw new ScriptException($"Task '{sentence.Command.Name}' not found");
            }
        }
    }
}