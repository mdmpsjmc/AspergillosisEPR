using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.BackgroundTasks.Logging
{
    public static class FileLoggerFactoryExtensions
    {
        public static ILoggingBuilder AddFile(this ILoggingBuilder builder)
        {
            builder.Services.AddSingleton<ILoggerProvider, FileLoggerProvider>();
            return builder;
        }

        public static ILoggingBuilder AddFile(this ILoggingBuilder builder, Action<FileLoggerOptions> configure)
        {
            builder.AddFile();
            builder.Services.Configure(configure);

            return builder;
        }
    }
}
