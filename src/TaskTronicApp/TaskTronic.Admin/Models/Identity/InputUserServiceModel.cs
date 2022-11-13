namespace TaskTronic.Admin.Models.Identity
{
    using TaskTronic.Models;

    public class InputUserServiceModel : IMapFrom<LoginFormModel>
    {
        public string UserName { get; set; }

        public string Password { get; set; }
    }
}
