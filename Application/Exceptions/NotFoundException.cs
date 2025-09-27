using System;
using System.Runtime.Serialization;

namespace Application.Exceptions;

[Serializable]
public class NotFoundException : Exception
{
    public NotFoundException() : base() { }

    public NotFoundException(string? message) : base(message) { }

    public NotFoundException(string? message, Exception? innerException) : base(message, innerException) { }

#pragma warning disable SYSLIB0051 // Type or member is obsolete
    protected NotFoundException(SerializationInfo info, StreamingContext context) : base(info, context) { }
#pragma warning restore SYSLIB0051 // Type or member is obsolete

}
