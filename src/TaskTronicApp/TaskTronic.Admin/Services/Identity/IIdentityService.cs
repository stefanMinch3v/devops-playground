namespace TaskTronic.Admin.Services.Identity
{
    using Models.Identity;
    using Refit;
    using System.Threading.Tasks;

    public interface IIdentityService
    {
        [Post("/Identity/Login")]
        Task<OutputJwtModel> Login([Body] InputUserServiceModel loginInput);
    }
}
