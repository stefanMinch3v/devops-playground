namespace TaskTronic.Drive.Messages
{
    using MassTransit;
    using System.Threading.Tasks;
    using TaskTronic.Drive.Services.Employees;
    using TaskTronic.Messages.Drive.Employees;

    public class UserRegisteredConsumer : IConsumer<UserRegisteredMessage>
    {
        private readonly IEmployeeService employeeService;

        public UserRegisteredConsumer(IEmployeeService employeeService) 
            => this.employeeService = employeeService;

        public async Task Consume(ConsumeContext<UserRegisteredMessage> context)
            => await this.employeeService.SaveAsync(
                context.Message.UserId, 
                context.Message.Email, 
                context.Message.Name);
    }
}
