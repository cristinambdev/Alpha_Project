using Business.Models;
using Data.Repositories;
using Domain.Extentions;
using Domain.Models;

namespace Business.Services;

public interface IStatusService
{
    Task<StatusResult<Status>> GetStatusByIdAsync(int id);
    Task<StatusResult<Status>> GetStatusByNameAsync(string StatusName);
    Task<StatusResult<IEnumerable<Status>>> GetStatusesAsync();
}

public class StatusService(IStatusRepository statusRepository) : IStatusService
{
    private readonly IStatusRepository _statusRepository = statusRepository;


    public async Task<StatusResult<IEnumerable<Status>>> GetStatusesAsync()
    {
        var result = await _statusRepository.GetAllAsync();
        return result.Succeeded
            ? new StatusResult<IEnumerable<Status>> { Succeeded = true, StatusCode = 200, Result = result.Result }
            : new StatusResult<IEnumerable<Status>> { Succeeded = false, StatusCode = result.StatusCode, Error = result.Error };

        //return result.MapTo<StatusResult>(); // dynamic mapping

    }


    public async Task<StatusResult<Status>> GetStatusByNameAsync(string StatusName)
    {
        var result = await _statusRepository.GetAsync(x => x.StatusName == StatusName);
        return result.Succeeded
         ? new StatusResult<Status> { Succeeded = true, StatusCode = 200, Result = result.Result }
         : new StatusResult<Status> { Succeeded = false, StatusCode = result.StatusCode, Error = result.Error };
    }

    public async Task<StatusResult<Status>> GetStatusByIdAsync(int id)
    {
        var result = await _statusRepository.GetAsync(x => x.Id == id);
        return result.Succeeded
         ? new StatusResult<Status> { Succeeded = true, StatusCode = 200, Result = result.Result }
         : new StatusResult<Status> { Succeeded = false, StatusCode = result.StatusCode, Error = result.Error };
    }
}
