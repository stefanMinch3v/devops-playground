namespace TaskTronic.Common
{
    using System;

    public class BaseDomainException : Exception
    {
        private string message;

        public new string Message
        {
            get => this.message ?? base.Message;
            set => this.message = value;
        }
    }
}
