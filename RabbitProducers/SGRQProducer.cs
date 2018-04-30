using Microsoft.Extensions.PlatformAbstractions;
using PeterKottas.DotNetCore.WindowsService;
using PeterKottas.DotNetCore.WindowsService.Base;
using PeterKottas.DotNetCore.WindowsService.Interfaces;
using System;
using System.IO;
using System.Timers;

namespace RabbitProducers
{
    class SGRQProducer :  IMicroService
    {
        public SGRQProducer()
        {

        }
        private IMicroServiceController controller;

        private System.Timers.Timer timer = new System.Timers.Timer(1000);

        public SGRQProducer(IMicroServiceController controller)
        {
            this.controller = controller;
        }

        private string fileName = Path.Combine(PlatformServices.Default.Application.ApplicationBasePath, "log.txt");
        public void Start()
        {
            Console.WriteLine("I started");
            Console.WriteLine(fileName);
            File.AppendAllText(fileName, "Started\n");

            /**
             * A timer is a simple example. But this could easily 
             * be a port or messaging queue client
             */
            timer.Elapsed += _timer_Elapsed;
            timer.Start();
        }

        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            File.AppendAllText(fileName, string.Format("Polling at {0}\n", DateTime.Now.ToString("o")));
        }

        public void Stop()
        {
            timer.Stop();
            File.AppendAllText(fileName, "Stopped\n");
            Console.WriteLine("I stopped");
        }

        public static void Main(string[] args)
        {
            var fileName = Path.Combine(PlatformServices.Default.Application.ApplicationBasePath, "log.txt");
            ServiceRunner<SGRQProducer>.Run(config =>
            {
                var name = config.GetDefaultName();
                config.Service(serviceConfig =>
                {
                    serviceConfig.ServiceFactory((extraArguments, controller) =>
                    {
                        return new SGRQProducer(controller);
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
