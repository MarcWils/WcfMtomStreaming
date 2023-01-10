using System.IO;
using System.ServiceModel;
using System.Threading.Tasks;

namespace Shared
{
    [ServiceContract]
    public interface IStreamingService
    {
        [OperationContract(Name = "Upload")]
        Task UploadAsync(Stream stream);
    }
}
