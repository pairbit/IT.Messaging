using System;

namespace IT.Messaging;

public class MessagingException : Exception
{
    public MessagingException(string? message, Exception? innerException) : base(message, innerException) { }
}