using Spectre.Console;
using System;
using System.Threading.Tasks;

[assembly: CLSCompliant(true)]
namespace CSF.Spectre
{
    /// <inheritdoc/>
    public class SpectreModuleBase<T> : ModuleBase<T>
        where T : IContext
    {
        /// <inheritdoc/>
        public override ExecuteResult Error(string message, params object[] values)
        {
            AnsiConsole.MarkupLineInterpolated($"[red]{string.Format(message, values)}[/]");
            return ExecuteResult.FromSuccess();
        }

        /// <inheritdoc/>
        public override Task<ExecuteResult> ErrorAsync(string message, params object[] values)
        {
            return Task.FromResult(Error(message, values));
        }

        /// <inheritdoc/>
        public override ExecuteResult Info(string message, params object[] values)
        {
            AnsiConsole.MarkupLineInterpolated($"[yellow]{string.Format(message, values)}[/]");
            return ExecuteResult.FromSuccess();
        }

        /// <inheritdoc/>
        public override Task<ExecuteResult> InfoAsync(string message, params object[] values)
        {
            return Task.FromResult(Info(message, values));
        }

        /// <inheritdoc/>
        public override ExecuteResult Success(string message, params object[] values)
        {
            AnsiConsole.MarkupLineInterpolated($"[green]{string.Format(message, values)}[/]");
            return ExecuteResult.FromSuccess();
        }

        /// <inheritdoc/>
        public override Task<ExecuteResult> SuccessAsync(string message, params object[] values)
        {
            return Task.FromResult(Success(message, values));
        }

        /// <inheritdoc/>
        public override ExecuteResult Respond(string message, params object[] values)
        {
            AnsiConsole.MarkupLine($"{string.Format(message, values)}");
            return ExecuteResult.FromSuccess();
        }

        /// <inheritdoc/>
        public override Task<ExecuteResult> RespondAsync(string message, params object[] values)
        {
            return Task.FromResult(Respond(message, values));
        }

        /// <summary>
        ///     Returns the input value of the requested question.
        /// </summary>
        /// <param name="question">The question to ask.</param>
        /// <returns>The <see cref="Task"/> containing the string with selected value within.</returns>
        public Task<string> AskAsync(string question)
        {
            return Task.FromResult(Ask(question));
        }

        /// <summary>
        ///     Returns the input value of the requested question.
        /// </summary>
        /// <param name="question">The question to ask.</param>
        /// <returns>The string with selected value within.</returns>
        public string Ask(string question)
        {
            return AnsiConsole.Ask<string>(question);
        }

        /// <summary>
        ///     Returns the input value of the requested prompt.
        /// </summary>
        /// <param name="prompt">The prompt to request a value for.</param>
        /// <returns>The <see cref="Task"/> containing the string with selected value within.</returns>
        [CLSCompliant(false)]
        public Task<string> PromptAsync(TextPrompt<string> prompt)
        {
            return Task.FromResult(Prompt(prompt));
        }

        /// <summary>
        ///     Returns the input value of the requested prompt.
        /// </summary>
        /// <param name="prompt">The prompt to request a value for.</param>
        /// <returns>The string with selected value within.</returns>
        [CLSCompliant(false)]
        public string Prompt(TextPrompt<string> prompt)
        {
            return AnsiConsole.Prompt(prompt);
        }

        /// <summary>
        ///     Returns the value of a selected <see cref="SelectionPrompt{T}"/> item.
        /// </summary>
        /// <param name="selection">The selection to pick from.</param>
        /// <returns>The <see cref="Task"/> containing the string with selected value within.</returns>
        [CLSCompliant(false)]
        public Task<string> SelectAsync(SelectionPrompt<string> selection)
        {
            return Task.FromResult(Select(selection));
        }

        /// <summary>
        ///     Returns the value of a selected <see cref="SelectionPrompt{T}"/> item.
        /// </summary>
        /// <param name="selection">The selection to pick from.</param>
        /// <returns>The string with selected value within.</returns>
        [CLSCompliant(false)]
        public string Select(SelectionPrompt<string> selection)
        {
            return AnsiConsole.Prompt(selection);
        }
    }
}
