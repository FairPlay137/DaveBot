using System;

namespace DaveBot.Common
{
    public class CommandUnsuccessfulException : Exception
    {
        public CommandUnsuccessfulException() : base(StringResourceHandler.GetTextStatic("err", "generic")) { }
        public CommandUnsuccessfulException(string message) : base(message) { }
    }
}
