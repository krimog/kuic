using System;

namespace Kuic.Csv
{
    /// <summary>
    /// The exception that is thrown when the format of the CSV is invalid.
    /// </summary>
    public class CsvFormatException : FormatException
    {
        /// <summary>
        /// Gets the line where the formatting error has been detected.
        /// </summary>
        /// <value>
        /// The line of the error.
        /// </value>
        public int? Line { get; }
        /// <summary>
        /// Gets the character index where the formatting error has been detected.
        /// </summary>
        /// <value>
        /// The character index of the error.
        /// </value>
        public int? Character { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvFormatException"/> class.
        /// </summary>
        /// <param name="message">The message of the exception</param>
        /// <param name="line">The line of the error.</param>
        public CsvFormatException(string message, int line) : this(message)
        {
            Line = line;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvFormatException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="line">The line of the error.</param>
        /// <param name="character">The character index of the error.</param>
        public CsvFormatException(string message, int line, int character) : this(message, line)
        {
            Character = character;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvFormatException"/> class.
        /// </summary>
        /// <param name="message">The message of the exception</param>
        /// <param name="line">The line of the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public CsvFormatException(string message, int line, Exception innerException) : this(message, innerException)
        {
            Line = line;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvFormatException"/> class.
        /// </summary>
        /// <param name="message">The message of the exception</param>
        /// <param name="line">The line of the error.</param>
        /// <param name="character">The character index of the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public CsvFormatException(string message, int line, int character, Exception innerException) : this(message, line, innerException)
        {
            Character = character;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvFormatException"/> class.
        /// </summary>
        public CsvFormatException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvFormatException"/> class.
        /// </summary>
        /// <param name="message">The message of the exception</param>
        public CsvFormatException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvFormatException"/> class.
        /// </summary>
        /// <param name="message">The message of the exception</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public CsvFormatException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
