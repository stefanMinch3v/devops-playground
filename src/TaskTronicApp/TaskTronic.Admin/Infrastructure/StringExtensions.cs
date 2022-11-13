namespace TaskTronic.Admin.Infrastructure
{
    using System;

    public static class StringExtensions
    {
        public static string ToControllerName(this string controllerName)
        {
            if (string.IsNullOrWhiteSpace(controllerName))
            {
                throw new InvalidOperationException($"{nameof(controllerName)} cannot be empty.");
            }

            var controllerIndex = controllerName.IndexOf("Controller");

            if (controllerIndex == -1)
            {
                throw new InvalidOperationException($"{controllerName} is not a controller name.");
            }

            return controllerName.Substring(0, controllerIndex);
        }
    }
}
