namespace TaskTronic.Identity.Services.Identity
{
    using Data.Models;
    using MassTransit;
    using Microsoft.AspNetCore.Identity;
    using Models;
    using Services.Jwt;
    using Services.Messages;
    using System.Linq;
    using System.Threading.Tasks;
    using TaskTronic.Data.Models;
    using TaskTronic.Messages.Drive.Employees;
    using TaskTronic.Services;

    public class IdentityService : IIdentityService
    {
        private const string INVALID_CREDENTIALS = "Invalid credentials.";
        private const string EMPTY_FORM = "Empty form is not accepted.";

        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly IJwtGeneratorService jwtGeneratorService;
        private readonly IBus publisher;
        private readonly IMessageService messageService;

        public IdentityService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IJwtGeneratorService jwtGeneratorService,
            IBus publisher,
            IMessageService messageService)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.jwtGeneratorService = jwtGeneratorService;
            this.publisher = publisher;
            this.messageService = messageService;
        }

        public async Task EditAsync(InputEditModel model)
        {
            if (model is null)
            {
                return;
            }

            var user = await this.userManager.FindByIdAsync(model.UserId);

            if (user is null)
            {
                return;
            }

            user.UserName = model.UserName;
            user.Email = model.Email;

            await this.userManager.UpdateAsync(user);
        }

        public async Task<Result<OutputJwtModel>> LoginAsync(InputLoginModel model)
        {
            if (model is null)
            {
                return EMPTY_FORM;
            }

            var result = await this.signInManager.PasswordSignInAsync(model.UserName, model.Password, false, false);
            if (!result.Succeeded)
            {
                return INVALID_CREDENTIALS;
            }

            var user = await this.userManager.FindByNameAsync(model.UserName);
            if (user is null)
            {
                return INVALID_CREDENTIALS;
            }

            return await this.jwtGeneratorService.GenerateTokenAsync(user);
        }

        public async Task<Result<bool>> RegisterAsync(InputRegisterModel model)
        {
            if (model is null)
            {
                return EMPTY_FORM;
            }

            var user = new ApplicationUser { Email = model.Email, UserName = model.Username };
            var result = await this.userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                return Result<bool>.Failure(result.Errors.Select(e => e.Description));
            }

            var messageData = new UserRegisteredMessage
            {
                Email = user.Email,
                Name = user.UserName,
                UserId = user.Id
            };

            var message = new Message(messageData);

            await this.messageService.SaveAsync(message);

            await this.publisher.Publish(messageData);

            await this.messageService.MarkMessageAsPublishedAsync(message.Id);

            return Result<bool>.SuccessWith(true);
        }
    }
}
