using System;

namespace PhotoOrganizer.Utils
{
    /// <summary>
    /// Implement of <see cref="IConsoleWrapper" /> for providing System.Console functionality.
    /// </summary>
    public class ConsoleWrapper : IConsoleWrapper
    {
        /// <summary>
        /// Reads next character or function key pressed by user. Optionally displays key to output.
        /// </summary>
        /// <param name="intercept">If the pressed key should be shown in output.</param>
        /// <returns>An object that describes the <see cref="ConsoleKey"/> pressed.</returns>
        public ConsoleKeyInfo ReadKey(bool intercept)
        {
            return Console.ReadKey(intercept);
        }

        /// <summary>
        /// Reads next line of standard input stream from user.
        /// </summary>
        /// <returns>The line from standard input stream.</returns>
        public string ReadLine()
        {
            return Console.ReadLine();
        }

        /// <summary>
        /// Write the value to standard output stream without line terminator.
        /// </summary>
        /// <param name="value">Value to be written.</param>
        public void Write(string value)
        {
            Console.Write(value);
        }

        /// <summary>
        /// Write the value to standard output with line terminator.
        /// </summary>
        /// <param name="value">Value to be written.</param>
        public void WriteLine(string value)
        {
            Console.WriteLine(value);
        }
    }
}
