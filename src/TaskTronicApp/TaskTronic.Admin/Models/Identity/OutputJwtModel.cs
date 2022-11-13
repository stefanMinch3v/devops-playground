namespace TaskTronic.Admin.Models.Identity
{
    public class OutputJwtModel
    {
        public OutputJwtModel(string token)
        {
            this.Token = token;
        }

        public string Token { get; }
    }
}
