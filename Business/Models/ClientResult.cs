using Domain.Models;

namespace Business.Models;

public class ClientResult : ServiceResult
{
    public IEnumerable<Client>? Result { get; set; }
    //public Client? Result { get; set; }  // by chat gpt: Single Client instead of IEnumerable<Client>
}
