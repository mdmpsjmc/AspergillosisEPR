using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitProducersStandard
{
    class NLogConfigurationFactory
    {
        public static void Configure()
        {
            var config = new NLog.Config.LoggingConfiguration();

            var logfile = new NLog.Targets.FileTarget() { FileName = "sgrq_connection.log", Name = "logfile" };
            var logconsole = new NLog.Targets.ConsoleTarget() { Name = "logconsole" };

            config.LoggingRules.Add(new NLog.Config.LoggingRule("*", LogLevel.Info, logconsole));
            config.LoggingRules.Add(new NLog.Config.LoggingRule("*", LogLevel.Debug, logfile));

            NLog.LogManager.Configuration = config;
        }
    }
}
