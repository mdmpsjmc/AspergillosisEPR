using System;
using System.ServiceProcess;

namespace RabbitProducersStandard.SGRQ
{

    public static class SGRQProducerService
    {
        public const string ServiceName = "SGRQProducerService";

        public class Service : ServiceBase
        {
            public Service()
            {
                ServiceName = SGRQProducerService.ServiceName;
            }

            protected override void OnStart(string[] args)
            {
                SGRQProducerService.Start(args);
            }

            protected override void OnStop()
            {
                SGRQProducerService.Stop();
            }
        }

        static void Main(string[] args)
        {
            if (!Environment.UserInteractive)
                // running as service
            using (var service = new Service())
                    ServiceBase.Run(service);
            else
            {
                // running as console app
                Start(args);
                var _logger = NLog.LogManager.GetCurrentClassLogger();
                _logger.Info("Done");                
                Stop();
            }
        }

        private static void Start(string[] args)
        {
            NLogConfigurationFactory.Configure();
            var producer = new SGRQRabbitMQProducer();
            producer.Produce();
        }

        private static void Stop()
        {
            // onstop code here
        }
    }
}
