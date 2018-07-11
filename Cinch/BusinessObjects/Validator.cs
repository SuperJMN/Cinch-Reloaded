using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using CinchExtended.Validation;

namespace CinchExtended.BusinessObjects
{
    public class Validator
    {

        #region Data
        private List<Rule> rules = new List<Rule>();
        Object _domainObject;

        #endregion

        public Validator(Object domainObject)
        {
            _domainObject = domainObject;
        }

        #region Public Methods/Properties
        /// <summary>
        /// Gets a value indicating whether or not this domain object is valid. 
        /// </summary>
        public virtual bool IsValid
        {
            get
            {
                return String.IsNullOrEmpty(this.Error);
            }
        }

        /// <summary>
        /// Gets an error message indicating what is wrong with this domain object. 
        /// The default is an empty string ("").
        /// </summary>
        public virtual string Error
        {
            get
            {
                string result = this[string.Empty];
                if (result != null && result.Trim().Length == 0)
                {
                    result = null;
                }
                return result;
            }
        }

        /// <summary>
        /// Gets the error message for the property with the given name.
        /// </summary>
        /// <param name="propertyName">The name of the property whose error 
        /// message to get.</param>
        /// <returns>The error message for the property. 
        /// The default is an empty string ("").</returns>
        public virtual string this[string propertyName]
        {
            get
            {
                StringBuilder sb = new StringBuilder();

                propertyName = CleanString(propertyName);

                foreach (Rule r in GetBrokenRules(propertyName))
                {
                    if (propertyName == string.Empty || r.PropertyName == propertyName)
                    {
                        sb.AppendLine(r.Description);
                    }
                }

                string result = sb.ToString().Trim();
                if (result.Length == 0)
                {
                    result = null;
                }
                return result;
            }
        }

        /// <summary>
        /// Validates all rules on this domain object, returning a list of the broken rules.
        /// </summary>
        /// <returns>A read-only collection of rules that have been broken.</returns>
        public virtual ReadOnlyCollection<Rule> GetBrokenRules()
        {
            return GetBrokenRules(string.Empty);
        }

        /// <summary>
        /// Validates all rules on this domain object for a given property, 
        /// returning a list of the broken rules.
        /// </summary>
        /// <param name="property">The name of the property to check for. 
        /// If null or empty, all rules will be checked.</param>
        /// <returns>A read-only collection of rules that have been broken.</returns>
        public virtual ReadOnlyCollection<Rule> GetBrokenRules(string property)
        {
            property = CleanString(property);

            List<Rule> broken = new List<Rule>();

            foreach (Rule r in this.rules)
            {
                // Ensure we only validate a rule 
                if (r.PropertyName == property || property == string.Empty)
                {
                    bool isRuleBroken = r.ValidateRule(_domainObject);
                    Debug.WriteLine(DateTime.Now.ToLongTimeString() +
                        ": Validating the rule: '" + r.ToString() +
                        "' on object '" + this.ToString() + "'. Result = " +
                        ((isRuleBroken == false) ? "Valid" : "Broken"));

                    if (isRuleBroken)
                    {
                        broken.Add(r);
                    }
                }
            }

            return broken.AsReadOnly();
        }


        /// <summary>
        /// Adds a new rule to the list of rules
        /// </summary>
        /// <param name="newRule">The new rule</param>
        public void AddRule(Rule newRule)
        {
            this.rules.Add(newRule);
        }

        /// <summary>
        /// Cleans a string by ensuring it isn't null and trimming it.
        /// </summary>
        /// <param name="s">The string to clean.</param>
        protected string CleanString(string s)
        {
            return (s ?? string.Empty).Trim();
        }
        #endregion
    }
}