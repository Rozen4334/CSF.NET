﻿using System;
using System.Threading.Tasks;

namespace CSF
{
    public abstract class PreconditionAttribute : Attribute
    {
        public abstract Task<PreconditionResult> CheckAsync(ICommandContext context, Command info, IServiceProvider provider);
    }
}
