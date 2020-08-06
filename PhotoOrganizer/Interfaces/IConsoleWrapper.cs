using System;

namespace PhotoOrganizer.Utils
{
    /// <summary>
    /// Interface for providing Console functionality.
    /// </summary>
    public interface IConsoleWrapper
    {
        /// <summary>
        /// Reads next character or function key pressed by user. Optionally displays key to output.
        /// </summary>
        /// <param name="intercept">If the pressed key should be shown in output.</param>
        /// <returns>An object that describes the <see cref="ConsoleKey"/> constant.</returns>
        ConsoleKeyInfo ReadKey(bool intercept);

        /// <summary>
        /// Reads next line of input.
        /// </summary>
        /// <returns>The read line from input.</returns>
        string ReadLine();

        /// <summary>
        /// Writes value without line terminator.
        /// </summary>
        /// <param name="value">Value to write.</param>
        void Write(string value);

        /// <summary>
        /// Writes value with newline.
        /// </summary>
        /// <param name="value">Value to write.</param>
        void WriteLine(string value);
    }
}
