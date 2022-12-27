using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

[assembly: CLSCompliant(true)]
namespace CSF
{
    /// <summary>
    ///     Represents the handler for registered commands.
    /// </summary>
    public static class CommandFramework
    {
        /// <summary>
        ///     Creates a new <see cref="IFrameworkBuilder"/> from the provided pipeline setup.
        /// </summary>
        /// <remarks>
        ///     This method sets up a number of default values for the returned <see cref="IFrameworkBuilder"/>.
        ///     <list type="bullet">
        ///         <item><description>Defines <paramref name="conveyor"/> as the specified <see cref="ICommandConveyor"/> for this builder.</description></item>
        ///         <item><description>Creates a new <see cref="CommandConfiguration"/> with default values.</description></item>
        ///         <item><description>Sets up a new <see cref="IServiceProvider"/> from <see cref="EmptyServiceProvider.Instance"/> with no defined services.</description></item>
        ///     </list>
        /// </remarks>
        /// <param name="conveyor">The pipeline service.</param>
        /// <returns>A new <see cref="IFrameworkBuilder"/> with default arguments and the defined <see cref="ICommandConveyor"/>.</returns>
        public static IFrameworkBuilder CreateDefaultBuilder<T>(T conveyor)
            where T : CommandConveyor
        {
            return new FrameworkBuilder<T>(conveyor);
        }

        /// <summary>
        ///     Creates a new <see cref="IFrameworkBuilder"/> with default setup.
        /// </summary>
        /// <remarks>
        ///     This method sets up a number of default values for the returned <see cref="IFrameworkBuilder"/>.
        ///     <list type="bullet">
        ///         <item><description>Creates a new <see cref="ICommandConveyor"/> with unmodified arguments.</description></item>
        ///         <item><description>Creates a new <see cref="CommandConfiguration"/> with default values.</description></item>
        ///         <item><description>Creates a new <see cref="IServiceProvider"/> from <see cref="EmptyServiceProvider.Instance"/> with no defined services.</description></item>
        ///     </list>
        /// </remarks>
        /// <returns>A new <see cref="IFrameworkBuilder"/> with default arguments.</returns>
        public static IFrameworkBuilder CreateDefaultBuilder()
        {
            return new FrameworkBuilder<CommandConveyor>(new CommandConveyor());
        }

        /// <summary>
        ///     Creates a new <see cref="ICommandFramework"/> with default setup directly. It skips all configuration.
        /// </summary>
        /// <remarks>
        ///     This method sets up the command framework with configuration that matches <see cref="CreateDefaultBuilder"/>. Read this method for more information.
        /// </remarks>
        /// <returns>A new <see cref="ICommandFramework"/>.</returns>
        public static ICommandFramework BuildWithMinimalSetup()
        {
            return CreateDefaultBuilder()
                .Build();
        }
    }

    /// <summary>
    ///     Represents the handler for registering and handling incoming commands.
    /// </summary>
    /// <remarks>
    ///     This is the root type of CSF. This type is responsible for:
    ///     <list type="bullet">
    ///         <item><description>Resolving all defined <see cref="ModuleBase{T}"/>'s and registering their commands.</description></item>
    ///         <item><description>Handling inbound commands and resolving their pipeline steps.</description></item>
    ///         <item><description>Holding references and making calls to the defined <see cref="ICommandConveyor"/>.</description></item>
    ///         <item><description>Logging the workflow to the provided <see cref="ILogger"/>.</description></item>
    ///     </list>
    /// </remarks>
    /// <typeparam name="T">The conveyor used to provide support in handling the command pipeline.</typeparam>
    public class CommandFramework<T> : ICommandFramework
        where T : CommandConveyor
    {
        private CancellationTokenSource _globalSource;

        /// <summary>
        ///     The conveyor that handles command creation, handle results and result invocation.
        /// </summary>
        public T Conveyor { get; }

        /// <inheritdoc/>
        public IList<IConditionalComponent> Commands { get; }

        /// <inheritdoc/>
        public CommandConfiguration Configuration { get; }

        /// <inheritdoc/>
        public IServiceProvider Services { get; }

        /// <inheritdoc/>
        public ILogger Logger { get; }

        /// <summary>
        ///     Creates a new instance of <see cref="CommandFramework{T}"/>.
        /// </summary>
        public CommandFramework(T conveyor)
            : this(new CommandConfiguration(), conveyor)
        {
            
        }

        /// <summary>
        ///     Creates a new instance of <see cref="CommandFramework{T}"/>.
        /// </summary>
        public CommandFramework(CommandConfiguration configuration, T conveyor)
            : this(EmptyServiceProvider.Instance, configuration, conveyor)
        {
            
        }

        /// <summary>
        ///     Creates a new instance of <see cref="CommandFramework{T}"/>.
        /// </summary>
        public CommandFramework(IServiceProvider serviceProvider, CommandConfiguration configuration, T conveyor)
        {
            configuration.Prefixes ??= new PrefixProvider();
            configuration.TypeReaders ??= new TypeReaderProvider();
            
            Configuration = configuration;

            Conveyor = conveyor;
            Services = serviceProvider;
        }

        /// <inheritdoc/>
        public virtual async Task StartAsync(bool autoConfigureAssemblies = true, CancellationToken cancellationToken = default)
        {
            await Task.CompletedTask;

            _ = RunAsync(autoConfigureAssemblies, cancellationToken);
        }

        /// <inheritdoc/>
        public virtual async Task RunAsync(bool autoConfigureAssemblies = true, CancellationToken cancellationToken = default)
        {
            _globalSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            if (autoConfigureAssemblies)
            {
                if (Configuration.RegistrationAssemblies is null || !Configuration.RegistrationAssemblies.Any())
                    throw new MissingValueException("Array was found to be null or empty.", nameof(Configuration.RegistrationAssemblies));

                await ConfigureTypeReadersAsync(_globalSource.Token);

                await ConfigureModulesAsync(_globalSource.Token);
            }

            await RunAsync();
        }

        protected virtual async Task RunAsync()
        {
            try
            {
                while (!_globalSource.IsCancellationRequested)
                {
                    var input = await Conveyor.GetInputAsync(_globalSource.Token);

                    var context = await Conveyor.BuildContextAsync(input, _globalSource.Token);

                    await ExecuteCommandsAsync(context, cancellationToken: _globalSource.Token);
                }
            }
            catch
            {
                _globalSource.Cancel();
                throw;
            }
        }

        /// <inheritdoc/>
        public virtual async Task StopAsync(CancellationToken cancellationToken = default)
        {
            await Task.CompletedTask;

            if (_globalSource is null)
                throw new MemberUnpreparedException();

            _globalSource.Cancel();
        }

        /// <inheritdoc/>
        public virtual async ValueTask ConfigureModulesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var assembly in Configuration.RegistrationAssemblies)
                await BuildModulesAsync(assembly, cancellationToken);
        }

        /// <inheritdoc/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual async ValueTask BuildModulesAsync(Assembly assembly, CancellationToken cancellationToken)
        {
            var mt = typeof(IModuleBase);

            foreach (var type in assembly.ExportedTypes)
                if (mt.IsAssignableFrom(type) && !type.IsAbstract && type.IsPublic)
                    await BuildModuleAsync(type, cancellationToken);
        }

        /// <inheritdoc/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual async ValueTask BuildModuleAsync(Type type, CancellationToken cancellationToken)
        {
            var module = ModuleInfo.Build(Configuration.TypeReaders, type);

            foreach (var component in module.Components)
            {
                Commands.Add(component);
                await component.RequestToHandleAsync(Conveyor, cancellationToken).ConfigureAwait(false);
            }
        }

        /// <inheritdoc/>
        public virtual async ValueTask ConfigureTypeReadersAsync(CancellationToken cancellationToken = default)
        {
            foreach (var assembly in Configuration.RegistrationAssemblies)
                await BuildTypeReadersAsync(assembly, cancellationToken);
        }

        /// <inheritdoc/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual async ValueTask BuildTypeReadersAsync(Assembly assembly, CancellationToken cancellationToken)
        {
            var tt = typeof(ITypeReader);

            foreach (var type in assembly.ExportedTypes)
                if (tt.IsAssignableFrom(type) && !type.IsAbstract && type.IsPublic)
                    await BuildTypeReaderAsync(type, cancellationToken);
        }

        /// <inheritdoc/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual async ValueTask BuildTypeReaderAsync(Type type, CancellationToken cancellationToken)
        {
            var reader = TypeReaderInfo.Build(type);

            var output = reader.Construct(Services);

            if (output is null)
                throw new ArgumentNullException(nameof(type), "The provided type caused an invalid return type.");

            if (output is ITypeReader typeReader)
            {
                Configuration.TypeReaders.Include(typeReader);
                await typeReader.RequestToHandleAsync(Conveyor, cancellationToken).ConfigureAwait(false);
            }

            throw new InvalidOperationException($"Could not box {type.Name} as {nameof(ITypeReader)}.");
        }

        /// <inheritdoc/>
        public virtual bool TryParsePrefix(ref string rawInput, out IPrefix prefix)
        {
            if (Configuration.Prefixes.TryGetPrefix(rawInput, out prefix))
            {
                rawInput = rawInput[prefix.Value.Length..].TrimStart();
                return true;
            }
            return false;
        }

        /// <inheritdoc/>
        public virtual async Task<IResult> ExecuteCommandsAsync<TContext>(TContext context, IServiceProvider services = null, CancellationToken cancellationToken = default)
            where TContext : IContext
        {
            services ??= Services;

            if (Configuration.DoAsynchronousExecution)
            {
                _ = Task.Run(async () =>
                {
                    var result = await RunPipelineAsync(context, services, cancellationToken).ConfigureAwait(false);

                    await result.RequestToHandleAsync(context, Conveyor, cancellationToken).ConfigureAwait(false);
                });
                return ExecuteResult.FromSuccess();
            }

            else
            {
                var result = await RunPipelineAsync(context, services, cancellationToken).ConfigureAwait(false);

                await result.RequestToHandleAsync(context, Conveyor, cancellationToken).ConfigureAwait(false);

                return result;
            }
        }

        /// <summary>
        ///     Invokes the pipeline for provided context.
        /// </summary>
        /// <remarks>
        ///     This method can be overridden to modify the execution flow.
        /// </remarks>
        /// <typeparam name="TContext">The context to execute the pipeline for.</typeparam>
        /// <param name="context">The context to execute the pipeline for.</param>
        /// <param name="provider">The services for this transient execution.</param>
        /// <param name="cancellationToken">The cancellation token that can be used to cancel this handle.</param>
        /// <returns>An asynchronous <see cref="ValueTask"/> with the <see cref="IResult"/> returned by this handle.</returns>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual async ValueTask<IResult> RunPipelineAsync<TContext>(TContext context, IServiceProvider provider, CancellationToken cancellationToken)
            where TContext : IContext
        {
            Logger.Debug($"Starting command pipeline for name: '{context.Name}'");

            // find commands
            var searchResult = await SearchAsync(context, cancellationToken).ConfigureAwait(false);

            if (!searchResult.IsSuccess)
                return searchResult;

            // check commands for availability.
            var checkResult = await CheckAsync(context, searchResult.Result, provider, cancellationToken).ConfigureAwait(false);

            if (!checkResult.IsSuccess)
                return checkResult;

            // select commands.
            var commands = checkResult.Result;
            if (!Configuration.ExecuteAllValidMatches)
                commands = new[] { checkResult.Result[0] };

            foreach (var command in commands)
            {
                // build module.
                var constructResult = await ConstructAsync(context, command, provider, cancellationToken).ConfigureAwait(false);

                if (!constructResult.IsSuccess)
                    return constructResult;

                // parse types.
                var readResult = await ParseAsync(context, command, cancellationToken).ConfigureAwait(false);

                if (!readResult.IsSuccess)
                    return readResult;

                // execute command.
                var result = await ExecuteAsync(context, command, (ModuleBase<TContext>)constructResult.Result, ((ParseResult)readResult).Result, cancellationToken).ConfigureAwait(false);

                if (!result.IsSuccess)
                    return result;
            }
            return ExecuteResult.FromSuccess();
        }

        /// <summary>
        ///     Searches through the command list to find the best matches.
        /// </summary>
        /// <remarks>
        ///     This method can be overridden to modify the execution flow.
        /// </remarks>
        /// <typeparam name="TContext">The context to execute the pipeline for.</typeparam>
        /// <param name="context">The context to execute the pipeline for.</param>
        /// <param name="cancellationToken">The cancellation token that can be used to cancel this handle.</param>
        /// <returns>An asynchronous <see cref="ValueTask"/> with the <see cref="IResult"/> returned by this handle.</returns>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual async ValueTask<SearchResult> SearchAsync<TContext>(TContext context, CancellationToken cancellationToken)
            where TContext : IContext
        {
            var matches = Commands
                .Where(command => command.Aliases.Any(alias => string.Equals(alias, context.Name, StringComparison.OrdinalIgnoreCase)));

            var groups = matches.CastWhere<ModuleInfo>()
                .OrderBy(x => x.Components.Count)
                .ToList();

            var commands = matches.CastWhere<CommandInfo>()
                .OrderBy(x => x.Parameters.Count)
                .ToList();

            if (groups.Any())
            {
                if (context.Parameters.Count > 0)
                {
                    var oldName = context.Name;
                    var oldParam = context.Parameters;

                    context.Name = context.Parameters[0].ToString();
                    context.Parameters = context.Parameters.GetRange(1);

                    var result = await SearchModuleAsync(context, groups[0], cancellationToken).ConfigureAwait(false);

                    if (result.IsSuccess)
                        return result;

                    context.Name = oldName;
                    context.Parameters = oldParam;
                }
            }

            if (commands.Any())
                return await SearchCommandsAsync(context, commands, cancellationToken).ConfigureAwait(false);

            return Conveyor.OnCommandNotFound(context);
        }

        /// <summary>
        ///     Searches through a single module to find the best matches.
        /// </summary>
        /// <remarks>
        ///     This method can be overridden to modify the execution flow.
        /// </remarks>
        /// <typeparam name="TContext">The context to execute the pipeline for.</typeparam>
        /// <param name="context">The context to execute the pipeline for.</param>
        /// <param name="module">The module to search commands in.</param>
        /// <param name="cancellationToken">The cancellation token that can be used to cancel this handle.</param>
        /// <returns>An asynchronous <see cref="ValueTask"/> with the <see cref="IResult"/> returned by this handle.</returns>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual async ValueTask<SearchResult> SearchModuleAsync<TContext>(TContext context, ModuleInfo module, CancellationToken cancellationToken)
            where TContext : IContext
        {
            var matches = module.Components
                .Where(command => command.Aliases.Any(alias => string.Equals(alias, context.Name, StringComparison.OrdinalIgnoreCase)));

            var commands = matches.CastWhere<CommandInfo>()
                .OrderBy(x => x.Parameters.Count)
                .ToList();

            if (commands.Count < 1)
            {
                var groups = matches.CastWhere<ModuleInfo>()
                    .OrderBy(x => x.Components.Count)
                    .ToList();

                if (groups.Any())
                {
                    context.Name = context.Parameters[0].ToString();
                    context.Parameters = context.Parameters.GetRange(1);

                    return await SearchModuleAsync(context, groups[0], cancellationToken).ConfigureAwait(false);
                }

                else
                    return Conveyor.OnCommandNotFound(context);
            }

            return await SearchCommandsAsync(context, commands, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        ///     Searches a single component for the its best matches.
        /// </summary>
        /// <remarks>
        ///     This method can be overridden to modify the execution flow.
        /// </remarks>
        /// <typeparam name="TContext">The context to execute the pipeline for.</typeparam>
        /// <param name="context">The context to execute the pipeline for.</param>
        /// <param name="commands">The commands to be searched through</param>
        /// <param name="cancellationToken">The cancellation token that can be used to cancel this handle.</param>
        /// <returns>An asynchronous <see cref="ValueTask"/> with the <see cref="IResult"/> returned by this handle.</returns>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual ValueTask<SearchResult> SearchCommandsAsync<TContext>(TContext context, IEnumerable<CommandInfo> commands, CancellationToken cancellationToken)
            where TContext : IContext
        {
            CommandInfo overload = null;
            IEnumerable<CommandInfo> SearchMatches()
            {
                foreach (var command in commands)
                {
                    if (overload is null && command.IsErrorOverload)
                        overload = command;

                    var commandLength = command.Parameters.Count;
                    var contextLength = context.Parameters.Count;

                    // If parameter & input length is equal, prefer it over all matches.
                    if (commandLength == contextLength)
                        yield return command;

                    // If command length is lower than context length, look for a remainder attribute.
                    // Due to sorting upwards, it will continue the loop and prefer the remainder attr with most parameters.
                    if (commandLength <= contextLength)
                        foreach (var parameter in command.Parameters)
                            if (parameter.Flags.HasFlag(ParameterFlags.IsRemainder))
                                yield return command;

                    // If context length is lower than command length, return the command with least optionals.
                    if (commandLength > contextLength)
                    {
                        int requiredLength = 0;
                        foreach (var parameter in command.Parameters)
                            if (!parameter.Flags.HasFlag(ParameterFlags.IsOptional))
                                requiredLength++;

                        if (contextLength >= requiredLength)
                            yield return command;
                    }
                }
            }

            var matches = SearchMatches();

            if (!matches.Any())
            {
                if (overload is null)
                    return Conveyor.OnBestOverloadUnavailable(context);
                else
                    return SearchResult.FromSuccess(new[] { overload });
            }

            Logger.Trace($"Found matches for name: '{context.Name}': {string.Join(", ", matches)}");

            return SearchResult.FromSuccess(matches.ToArray());
        }

        /// <summary>
        ///     Checks the preconditions of all resolved commands.
        /// </summary>
        /// <remarks>
        ///     This method can be overridden to modify the execution flow.
        /// </remarks>
        /// <typeparam name="TContext">The context to execute the pipeline for.</typeparam>
        /// <param name="context">The context to execute the pipeline for.</param>
        /// <param name="matches">The command matches to be used for executing commands.</param>
        /// <param name="provider">The services to be used for handling the preconditions.</param>
        /// <param name="cancellationToken">The cancellation token that can be used to cancel this handle.</param>
        /// <returns>An asynchronous <see cref="ValueTask"/> with the <see cref="IResult"/> returned by this handle.</returns>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual async ValueTask<CheckResult> CheckAsync<TContext>(TContext context, IEnumerable<CommandInfo> matches, IServiceProvider provider, CancellationToken cancellationToken)
            where TContext : IContext
        {
            var commands = new List<CommandInfo>();
            var failureResult = PreconditionResult.FromSuccess();

            foreach (var match in matches)
            {
                var result = await CheckPreconditionsAsync(context, match, provider, cancellationToken);

                if (result.IsSuccess)
                    commands.Add(match);
                else
                    failureResult = result;
            }

            if (!commands.Any())
                return CheckResult.FromError(failureResult.ErrorMessage, failureResult.Exception);

            return CheckResult.FromSuccess(commands.ToArray());
        }

        /// <summary>
        ///     Checks the preconditions of a single command.
        /// </summary>
        /// <remarks>
        ///     This method can be overridden to modify the execution flow.
        /// </remarks>
        /// <typeparam name="TContext">The context to execute the pipeline for.</typeparam>
        /// <param name="context">The context to execute the pipeline for.</param>
        /// <param name="command">The command to be used for execution.</param>
        /// <param name="provider">The services used to handle the precondition.</param>
        /// <param name="cancellationToken">The cancellation token that can be used to cancel this handle.</param>
        /// <returns>An asynchronous <see cref="ValueTask"/> with the <see cref="IResult"/> returned by this handle.</returns>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual async ValueTask<PreconditionResult> CheckPreconditionsAsync<TContext>(TContext context, CommandInfo command, IServiceProvider provider, CancellationToken cancellationToken)
            where TContext : IContext
        {
            foreach (var precon in command.Preconditions)
            {
                var result = await precon.CheckAsync(context, command, provider, cancellationToken).ConfigureAwait(false);

                if (!result.IsSuccess)
                    return result;
            }

            Logger.Trace($"Succesfully ran precondition checks for {command.Name}.");

            return PreconditionResult.FromSuccess();
        }

        /// <summary>
        ///     Constructs the module to be used for command execution.
        /// </summary>
        /// <remarks>
        ///     This method can be overridden to modify the execution flow.
        /// </remarks>
        /// <typeparam name="TContext">The context to execute the pipeline for.</typeparam>
        /// <param name="context">The context to execute the pipeline for.</param>
        /// <param name="command">The command to be used for execution.</param>
        /// <param name="provider">The services used to create the module.</param>
        /// <param name="cancellationToken">The cancellation token that can be used to cancel this handle.</param>
        /// <returns>An asynchronous <see cref="ValueTask"/> with the <see cref="IResult"/> returned by this handle.</returns>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual ValueTask<ConstructionResult> ConstructAsync<TContext>(TContext context, CommandInfo command, IServiceProvider provider, CancellationToken cancellationToken)
            where TContext : IContext
        {
            var services = new List<object>();
            foreach (var dependency in command.Module.Constructor.Dependencies)
            {
                if (dependency.Type == typeof(IServiceProvider))
                    services.Add(provider);
                else
                {
                    var t = provider.GetService(dependency.Type);

                    if (t is null && !dependency.Flags.HasFlag(ParameterFlags.IsNullable))
                        return Conveyor.OnServiceNotFound(context, dependency);

                    services.Add(t);
                }
            }

            var obj = command.Module.Constructor.EntryPoint.Invoke(services.ToArray());

            if (obj is ModuleBase<TContext> commandBase)
            {
                commandBase.SetContext(context);
                commandBase.SetInformation(command);

                commandBase.SetLogger(Logger);

                Logger.Trace($"Succesfully constructed module for {command.Name}");

                return ConstructionResult.FromSuccess(commandBase);
            }
            return Conveyor.OnInvalidModule(context, command.Module);
        }

        /// <summary>
        ///     Parses the types found in the command parameters from the provided context.
        /// </summary>
        /// <remarks>
        ///     This method can be overridden to modify the execution flow.
        /// </remarks>
        /// <typeparam name="TContext">The context to execute the pipeline for.</typeparam>
        /// <param name="context">The context to execute the pipeline for.</param>
        /// <param name="command">The command to be used for execution.</param>
        /// <param name="cancellationToken">The cancellation token that can be used to cancel this handle.</param>
        /// <returns>An asynchronous <see cref="ValueTask"/> with the <see cref="IResult"/> returned by this handle.</returns>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual async ValueTask<IResult> ParseAsync<TContext>(TContext context, CommandInfo command, CancellationToken cancellationToken)
            where TContext : IContext
        {
            int index = 0;
            var parameters = new List<object>();

            foreach (var parameter in command.Parameters)
            {
                if (parameter.Flags.HasFlag(ParameterFlags.IsRemainder))
                {
                    parameters.Add(string.Join(" ", context.Parameters.Skip(index)));
                    break;
                }

                if (parameter.Flags.HasFlag(ParameterFlags.IsOptional) && context.Parameters.Count <= index)
                {
                    var missingResult = Conveyor.OnMissingValue(context, parameter);

                    if (!missingResult.IsSuccess)
                        return Conveyor.OnOptionalNotPopulated(context);

                    var resultType = missingResult.Result.GetType();

                    if (resultType == parameter.Type || missingResult.Result == Type.Missing)
                    {
                        parameters.Add(missingResult.Result);
                        continue;
                    }
                    else
                        return Conveyor.OnMissingReturnedInvalid(context, parameter.Type, resultType);
                }

                if (parameter.Type == typeof(string) || parameter.Type == typeof(object))
                {
                    parameters.Add(context.Parameters[index]);
                    index++;
                    continue;
                }

                var result = await parameter.TypeReader.ReadAsync(context, parameter, context.Parameters[index], cancellationToken).ConfigureAwait(false);

                if (!result.IsSuccess)
                    return result;

                parameters.Add(result.Result);
                index++;
            }

            Logger.Trace($"Succesfully populated parameters for {command.Name}. Count: {parameters.Count}");

            return ParseResult.FromSuccess(parameters.ToArray());
        }

        /// <summary>
        ///     Executes the provided command.
        /// </summary>
        /// <remarks>
        ///     This method can be overridden to modify the execution flow.
        /// </remarks>
        /// <typeparam name="TContext">The context to execute the pipeline for.</typeparam>
        /// <param name="context">The context to execute the pipeline for.</param>
        /// <param name="command">The command to execute.</param>
        /// <param name="module">The module to use for command execution.</param>
        /// <param name="parameters">The parsed parameters to be used when executing the method.</param>
        /// <param name="cancellationToken">The cancellation token that can be used to cancel this handle.</param>
        /// <returns>An asynchronous <see cref="ValueTask"/> with the <see cref="IResult"/> returned by this handle.</returns>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual async ValueTask<IResult> ExecuteAsync<TContext>(TContext context, CommandInfo command, ModuleBase<TContext> module, object[] parameters, CancellationToken cancellationToken)
            where TContext : IContext
        {
            try
            {
                Logger.Debug($"Starting execution of {module.CommandInfo.Name}.");
                var sw = Stopwatch.StartNew();

                await module.BeforeExecuteAsync(command, context, cancellationToken).ConfigureAwait(false);

                var returnValue = command.Method.Invoke(module, parameters);

                switch (returnValue)
                {
                    case Task<IResult> execTask:
                        var asyncResult = await execTask;
                        if (!asyncResult.IsSuccess)
                            return asyncResult;
                        break;
                    case Task task:
                        await task;
                        break;
                    case IResult syncResult:
                        if (!syncResult.IsSuccess)
                            return syncResult;
                        break;
                    default:
                        if (returnValue is null)
                            break;
                        var returnResult = Conveyor.OnUnhandledReturnType(context, returnValue);
                        if (!returnResult.IsSuccess)
                            return returnResult;
                        break;
                }

                await module.AfterExecuteAsync(command, context, cancellationToken).ConfigureAwait(false);

                sw.Stop();
                Logger.Debug($"Finished execution of {module.CommandInfo.Name} in {sw.ElapsedMilliseconds} ms.");

                return ExecuteResult.FromSuccess();
            }
            catch (Exception ex)
            {
                return Conveyor.OnUnhandledException(context, command, ex);
            }
        }
    }
}
