using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    public class OperationException:Exception
    {
        public OperationException() { }

        public OperationException(string message) :base(message) { }

        public OperationException(string message, Exception innerException):base(message,innerException) { }
    }
}
