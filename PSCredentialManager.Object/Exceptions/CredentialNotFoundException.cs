using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace PSCredentialManager.Common.Exceptions
{
    public class CredentialNotFoundException : Exception, ISerializable
    {
        public CredentialNotFoundException() { }
        public CredentialNotFoundException(string message) : base(message) { }
        public CredentialNotFoundException(string message, Exception inner) : base(message, inner) { }
        public CredentialNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
