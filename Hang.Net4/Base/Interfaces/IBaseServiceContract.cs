using System.ServiceModel;

namespace Hang.Net4.Base.Interfaces
{
    [ServiceContract(Name = "BaseService")]
    public interface IBaseServiceContract
    {
        [OperationContract]
        string Action(string parameter);
    }
}
