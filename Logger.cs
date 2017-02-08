using System;
using System.Collections.Generic;
using System.IO;

namespace CommonCode
{
    /// <summary>
    /// Takes messages from throughout the program and outputs them for the user to see.
    /// </summary>
    public class Logger
    {
        ///Planned Outputs:
        /// Text file
        /// Console Window
        /// In-Program Console (maybe)
        public static Logger GlobalLogger;

        static readonly string defaultOutputFile = ".//Content//log.txt";
        string outputFile;

        /// <summary>
        /// Logger will send messages of this level and higher to the corresponding output.
        /// </summary>
        public Dictionary<OutputTypes, MessageType> outputLevels = new Dictionary<OutputTypes,MessageType>();
        /// <summary>
        /// Bitfield of outputs that the logger is currently sending data to.
        /// </summary>
        OutputTypes currentOutputs = OutputTypes.TextFile;
        StreamWriter textWriter;

        public Logger() : this(defaultOutputFile, OutputTypes.TextFile, MessageType.Message) { }

        Logger(Logger original)
        {
            throw new NotImplementedException();
        }

        public Logger(string outputFile, OutputTypes types, MessageType outputLevel)
        {
            if (outputFile != null)
                this.outputFile = outputFile;
            if ((int)types < 1 || (int)types > 3)
                throw new ArgumentException();
            currentOutputs = types;
            if ((int)(currentOutputs & OutputTypes.TextFile) > 0)
            {
                textWriter = new StreamWriter(this.outputFile, true);
                outputLevels.Add(OutputTypes.TextFile, outputLevel);
                textWriter.BaseStream.Position = textWriter.BaseStream.Length;
            }
        }

        public void WriteMessage(string message, MessageType kind)
        {
            if ((int)(currentOutputs & OutputTypes.TextFile) > 0 && (byte)outputLevels[OutputTypes.TextFile] <= (byte)kind)
            {
                textWriter.WriteLine("[" + Enum.GetName(typeof(MessageType), kind) + "] " + message);
                textWriter.Flush();
            }
        }
        public void WriteException(Exception e)
        {
            if ((int)(currentOutputs & OutputTypes.TextFile) > 0)
            {
                textWriter.WriteLine("[Error] " + e.ToString());
                textWriter.Flush();
            }
        }

        /// <summary>
        /// Creates a new logger with the shallowly copied outputs and deeply copied output message level settings.
        /// </summary>
        /// <param name="original"></param>
        /// <returns></returns>
        public Logger Clone(Logger original)
        { return new Logger(original); }
    }

    /// <summary>
    /// Bitfield of outputs available to the logger.
    /// </summary>
    public enum OutputTypes
    { 
        None = 0,
        TextFile = 1,
        Console = 2
    }

    public enum MessageType : byte
    { 
        /// <summary>
        /// Status meant for messages that signify things working as intended.
        /// </summary>
        Message = 0,
        /// <summary>
        /// Status meant for messages that may cause problems, or errors that the program can recover from.
        /// </summary>
        Warning,
        /// <summary>
        /// Status meant for situations where the program has encountered a major problem that it can't or shouldn't fix on its own.
        /// </summary>
        Error
    }
}
