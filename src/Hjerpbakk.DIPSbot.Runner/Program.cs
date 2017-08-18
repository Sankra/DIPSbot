using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topshelf;

namespace Hjerpbakk.DIPSbot.Runner {
    class Program {
        static readonly Logger logger;

        static Program() {
            logger = LogManager.GetCurrentClassLogger();
        }

        static void Main(string[] args) {
            try {
                logger.Info("Starting DIPSbot.");
                HostFactory.Run(host => {
                    host.Service<DIPSbotHost>(service => {
                        service.ConstructUsing(name => new DIPSbotHost());
                        service.WhenStarted(n => { n.Start(); });
                        service.WhenStopped(n => n.Stop());
                    });

                    host.UseNLog();

                    host.OnException(exception => { logger.Fatal(exception, "Fatal error, DIPSbot going down."); });

                    host.RunAsNetworkService();

                    host.SetDisplayName("Slack DIPSbot");
                    host.SetServiceName("Slack DIPSbot");
                    host.SetDescription("The DIPS specific Slackbot.");
                });

                logger.Info("DIPSbot stopped.");
            }
            catch (Exception e) {
                logger.Fatal(e, "Could not start DIPSbot");
            }
        }
    }
}