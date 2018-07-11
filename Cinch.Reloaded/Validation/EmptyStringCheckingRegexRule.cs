using System;
using System.Text.RegularExpressions;

namespace Cinch.Reloaded.Validation
{
    public class EmptyStringCheckingRegexRule : RegexRule
    {
        private readonly string regex;

        public EmptyStringCheckingRegexRule(string datavalue, string invalidName, string regex)
            : base(datavalue, invalidName, regex)
        {
            this.regex = regex;
        }

        public override bool ValidateRule(object domainObject)
        {
            var pi = domainObject.GetType().GetProperty(this.PropertyName);
            var value = pi.GetValue(domainObject, null) as String;
            if (value != null)
            {
                var m = Regex.Match(value, regex);
                return !m.Success;
            }
            return false;
        }
    }
}