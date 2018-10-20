using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaveBot.Common
{
    public class CommandUnsuccessfulException : Exception
    {
        public CommandUnsuccessfulException() : base(StringResourceHandler.GetTextStatic("err", "generic")) { }
        public CommandUnsuccessfulException(string message) : base(message) { }
    }
}
