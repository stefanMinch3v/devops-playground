namespace TaskTronic.Identity.Messages
{
    using MassTransit;
    using System.Threading.Tasks;
    using Services.Identity;
    using Models;
    using TaskTronic.Messages.Drive.Employees;

    public class UserEditedConsumer : IConsumer<UserEditedMessage>
    {
        private readonly IIdentityService identityService;

        public UserEditedConsumer(IIdentityService identityService)
            => this.identityService = identityService;

        public async Task Consume(ConsumeContext<UserEditedMessage> context)
            => await this.identityService.EditAsync(
                new InputEditModel 
                { 
                    Email = context.Message.Email,
                    UserId = context.Message.UserId,
                    UserName = context.Message.Name
                });
    }
}
