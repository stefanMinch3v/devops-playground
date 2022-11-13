namespace TaskTronic.Identity.Models
{
    using System;
    using System.Collections.Generic;

    public class OutputJwtModel
    {
        public OutputJwtModel(string token, IList<string> roles, DateTime expiration)
        {
            this.Token = token;
            this.Roles = roles;
            this.Expiration = expiration;
        }

        public string Token { get; }
        public IList<string> Roles { get; }
        public DateTime Expiration { get; }
    }
}
