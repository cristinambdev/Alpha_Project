
namespace Business.Models;

public class MiniProjectResult<T> : ServiceResult
{
    public T? Result { get; set; }

}

public class MiniProjectResult : ServiceResult
{
}