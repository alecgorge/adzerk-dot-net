﻿using System;

namespace StackExchange.Adzerk.Models
{
    class AdzerkApiException : ApplicationException
    {
        public dynamic Context;

        public AdzerkApiException(string message) : base(message)
        {
        }

        public AdzerkApiException(string message, dynamic context) : base(message)
        {
            this.Context = context;
        }

        public AdzerkApiException(string message, Exception ex, dynamic context) : base(message, ex)
        {
            this.Context = context;
        }
    }
}
