﻿using System;

namespace IT.Messaging;

public class MessagingException : Exception
{
    public MessagingException(string? message, Exception? innerException = null) : base(message, innerException) { }
}

public class HandlerRegisteredException : MessagingException
{
    public HandlerRegisteredException(string? queue) : base($"Handler registered for queue '{queue}'") { }
}

public class HandlerNotRegisteredException : MessagingException
{
    public HandlerNotRegisteredException(string? queue) : base($"Handler not registered for queue '{queue}'") { }
}