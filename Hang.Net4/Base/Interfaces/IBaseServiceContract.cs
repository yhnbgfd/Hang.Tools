using System.ServiceModel;

namespace Hang.Net4.Base.Interfaces
{
    [ServiceContract]
    public interface IBaseServiceContract
    {
        [OperationContract]
        string Action(string parameter);
    }
}
