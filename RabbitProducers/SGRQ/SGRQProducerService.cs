using FluentScheduler;
using Microsoft.Extensions.PlatformAbstractions;
using PeterKottas.DotNetCore.WindowsService;
using PeterKottas.DotNetCore.WindowsService.Base;
using PeterKottas.DotNetCore.WindowsService.Interfaces;
using RabbitConsumers;
using System;
using System.IO;
using System.Timers;


namespace RabbitProducers.SGRQ
{
    class SGRQProducerService :  MicroService, IMicroService
    {
        private IMicroServiceController controller;
        private static int INTERVAL_IN_MILISECONDS = 900000; // 15 min in miliseconds

        public SGRQProducerService(IMicroServiceController controller)
        {
            this.controller = controller;
        }

        public void Start()
        {
            this.StartBase();
            Timers.Start("Poller", INTERVAL_IN_MILISECONDS, () =>
            {
                Console.WriteLine("Reading Saint Georges Respiratory Questionnaires into exchange queue at {0}\n", DateTime.Now.ToString("o"));
                var producer = new SGRQRabbitMQProducer();
                producer.Produce();
            },
            (e) =>
            {
                Console.WriteLine("Exception while polling: {0}\n", e.ToString());
            });
            Console.WriteLine("I started");
        }

        public void Stop()
        {
            this.StopBase();
            Console.WriteLine("I stopped");
        }

        public static void Main(string[] args)
        {
            var fileName = Path.Combine(PlatformServices.Default.Application.ApplicationBasePath, "log.txt");
            ServiceRunner<SGRQProducerService>.Run(config =>
            {
                var name = config.GetDefaultName();
                config.Service(serviceConfig =>
                {
                    serviceConfig.ServiceFactory((extraArguments, controller) =>
                    {
                        return new SGRQProducerService(controller);
                    });

                    serviceConfig.OnStart((service, extraParams) =>
                    {
                        Console.WriteLine("Service {0} started", name);
                        service.Start();
                    });

                    serviceConfig.OnStop(service =>
                    {
                        Console.WriteLine("Service {0} stopped", name);
                        service.Stop();
                    });

                    serviceConfig.OnError(e =>
                    {
                        File.AppendAllText(fileName, $"Exception: {e.ToString()}\n");
                        Console.WriteLine("Service {0} errored with exception : {1}", name, e.Message);
                    });
                });
            });
        }
    }
}
