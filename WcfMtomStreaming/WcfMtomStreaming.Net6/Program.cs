using Shared;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var app = builder.Build();

        app.MapGet("/", () => "Hello World!");
        app.MapPost("/", async (HttpContext context) =>
        {
            context.Response.Headers.ContentType = "multipart/related; type=\"application/xop+xml\";start=\"<http://tempuri.org/0>\";boundary=\"uuid:fca834ef-6b4a-43c0-a7d0-09064d2827e8+id=1\";start-info=\"text/xml\"";
            await context.Response.WriteAsync("--uuid:fca834ef-6b4a-43c0-a7d0-09064d2827e8+id=1\r\nContent-ID: <http://tempuri.org/0>\r\nContent-Transfer-Encoding: 8bit\r\nContent-Type: application/xop+xml;charset=utf-8;type=\"text/xml\"\r\n\r\n<s:Envelope xmlns:s=\"http://schemas.xmlsoap.org/soap/envelope/\"><s:Body><UploadResponse xmlns=\"http://tempuri.org/\"></UploadResponse></s:Body></s:Envelope>");
        });

        app.Start();

        var cf = new ChannelFactory<IStreamingService>(CreateClientBinding(),
            new EndpointAddress(app.Urls.First(u => u.StartsWith("http:"))));
        var client = cf.CreateChannel();

        Console.WriteLine("Calling service...");

        await client.UploadAsync(new BloatedStream(4_294_967_296));

        await app.DisposeAsync();
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