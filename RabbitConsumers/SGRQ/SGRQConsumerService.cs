using Microsoft.Extensions.PlatformAbstractions;
using PeterKottas.DotNetCore.WindowsService;
using PeterKottas.DotNetCore.WindowsService.Base;
using PeterKottas.DotNetCore.WindowsService.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace RabbitConsumers.SGRQ
{
    class SGRQConsumerService : MicroService, IMicroService
    {
        private IMicroServiceController controller;
        private static int INTERVAL_IN_MILISECONDS = 60000;

        public SGRQConsumerService(IMicroServiceController controller)
        {
            this.controller = controller;
        }

        public void Start()
        {
            this.StartBase();
            Timers.Start("Poller", INTERVAL_IN_MILISECONDS, () =>
            {
                Console.WriteLine("Consuming Saint Georges Respiratory Questionnaires into database at {0}\n", DateTime.Now.ToString("o"));
                var consumer = new SGRQRabbitMQConsumer();
                consumer.Consume();

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
            ServiceRunner<SGRQConsumerService>.Run(config =>
            {
                var fileName = Path.Combine(PlatformServices.Default.Application.ApplicationBasePath, "log.txt");
                var name = config.GetDefaultName();
                config.Service(serviceConfig =>
                {
                    serviceConfig.ServiceFactory((extraArguments, controller) =>
                    {
                        return new SGRQConsumerService(controller);
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
