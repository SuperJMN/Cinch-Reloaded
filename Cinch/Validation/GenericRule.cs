using System;
using System.Linq.Expressions;
using CinchExtended.Utilities;

namespace CinchExtended.Validation
{
    public class GenericRule<T> : Rule
    {
        #region Data
        private Func<T, bool> _ruleDelegate;
        #endregion

        #region Ctor
        public GenericRule(Expression<Func<T, object>> propertyExpression, string brokenDescription, Func<T, bool> ruleDelegate)
            : base(ObservableHelper.GetPropertyName<T>(propertyExpression), brokenDescription)
        {

            RuleDelegate = ruleDelegate;
        }
        #endregion

        #region Public Methods/Properties
        /// <summary>
        /// Gets or sets the delegate used to validate this rule.
        /// </summary>
        protected virtual Func<T, Boolean> RuleDelegate
        {
            get { return _ruleDelegate; }
            set { _ruleDelegate = value; }
        }
        #endregion

        #region Overrides
        /// <summary>
        /// Validates that the rule has not been broken.
        /// </summary>
        /// <param name="domainObject">The domain object being validated.</param>
        /// <returns>True if the rule has not been broken, or false if it has.</returns>
        public override bool ValidateRule(object domainObject)
        {
            return RuleDelegate((T)domainObject);
        }
        #endregion
    }
}