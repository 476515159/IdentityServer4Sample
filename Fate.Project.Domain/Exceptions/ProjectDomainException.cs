using System;
using System.Collections.Generic;
using System.Text;

namespace Fate.Project.Domain.Exceptions
{
    public class ProjectDomainException : Exception
    {
        public ProjectDomainException()
        {
        }

        public ProjectDomainException(string message) : base(message)
        {
        }

        public ProjectDomainException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
