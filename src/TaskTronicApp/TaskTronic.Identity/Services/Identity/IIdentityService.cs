namespace TaskTronic.Identity.Services.Identity
{
    using Models;
    using System.Threading.Tasks;
    using TaskTronic.Services;

    public interface IIdentityService
    {
        Task<Result<bool>> RegisterAsync(InputRegisterModel model);

        Task<Result<OutputJwtModel>> LoginAsync(InputLoginModel model);

        Task EditAsync(InputEditModel model);
    }
}
