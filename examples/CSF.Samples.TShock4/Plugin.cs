﻿using CSF.TShock;
using System;
using Terraria;
using TerrariaApi.Server;

namespace CSF.Samples.TShock
{
    [ApiVersion(2, 1)]
    public sealed class Plugin : TerrariaPlugin
    {
        private readonly TShockCSF _csf;

        public Plugin(Main game)
            : base(game)
        {
            // Define the command standardization framework made for TShock.
            _csf = new TShockCSF(new TShockCommandConfiguration()
            {
                DoAsynchronousExecution = false,
                ReplaceAllExisting = true
            });
        }

        public override async void Initialize()
        {
            // Build the modules available in the current assembly.
            var result = await _csf.BuildModulesAsync(typeof(Plugin).Assembly);

            if (!result.IsSuccess)
                throw new InvalidOperationException(result.ErrorMessage, result.Exception);
        }
    }
}