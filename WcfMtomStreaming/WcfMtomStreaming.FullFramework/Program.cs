using Shared;
using System;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;

namespace WcfMtomStreaming.FullFramework
{
    internal class Program
    {
        static async Task Main()
        {
            Console.WriteLine("Starting...");
            using (var host = new ServiceHost(typeof(StreamingService)))
            {
                host.AddServiceEndpoint(typeof(IStreamingService),
                    CreateServerBinding(),
                    "http://localhost:9183");
                host.Description.Behaviors.OfType<ServiceDebugBehavior>().Single().IncludeExceptionDetailInFaults = true;
                host.Open();

                var cf = new ChannelFactory<IStreamingService>(CreateClientBinding());
                cf.Endpoint.Address = host.Description.Endpoints[0].Address;
                var client = cf.CreateChannel();

                Console.WriteLine("Calling service...");

                await client.UploadAsync(new BloatedStream(4_294_967_296));

                Console.WriteLine("Done, shutting down...");
                host.Close();
            }
        }

        private static Binding CreateServerBinding()
        {
            var binding = new CustomBinding();
            var mtomElement = new MtomMessageEncodingBindingElement(MessageVersion.Soap11, Encoding.Unicode);
            mtomElement.ReaderQuotas.MaxStringContentLength = 10971520;
            mtomElement.ReaderQuotas.MaxArrayLength = 10_485_760;

            binding.Elements.Add(mtomElement);
            binding.Elements.Add(new HttpTransportBindingElement
            {
                TransferMode = TransferMode.Streamed,
                MaxBufferSize = 1024 * 64,
                MaxBufferPoolSize = 1,
                MaxReceivedMessageSize = 5_368_709_120
            });
            binding.SendTimeout = TimeSpan.FromMinutes(5);
            binding.ReceiveTimeout = TimeSpan.FromMinutes(5);
            return binding;
        }

        private static Binding CreateClientBinding()
        {
            var binding = new CustomBinding();
            var mtomElement = new MtomMessageEncodingBindingElement(MessageVersion.Soap11, Encoding.UTF8);
            mtomElement.ReaderQuotas.MaxStringContentLength = 10971520;
            mtomElement.ReaderQuotas.MaxArrayLength = 10_485_760;

            binding.Elements.Add(mtomElement);
            binding.Elements.Add(new HttpTransportBindingElement
            {
                TransferMode = TransferMode.Streamed,
                MaxBufferSize = 1024 * 64,
                MaxBufferPoolSize = 1
            });
            binding.SendTimeout = TimeSpan.FromMinutes(5);
            binding.ReceiveTimeout = TimeSpan.FromMinutes(5);
            return binding;
        }
    }
}
