﻿using BlockchainAuthIoT.DataProvider.Models.Policies.Rules;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Threading.Tasks;

namespace BlockchainAuthIoT.DataProvider.Attributes
{
    public class ValidateIntParamAttribute : PolicyParamFilterAttribute
    {
        private readonly string queryParamName;
        private readonly IntCondition condition;
        private readonly string policyParamName;

        /// <summary>
        /// Adds blockchain policy-based validation for an integer parameter.
        /// </summary>
        /// <param name="queryParamName">The name of the parameter in the query string</param>
        /// <param name="condition">The condition when comparing it to the policy-enforced rule</param>
        /// <param name="policyParamName">The name of the parameter in the policy</param>
        public ValidateIntParamAttribute(string queryParamName, IntCondition condition, string policyParamName = null)
        {
            this.queryParamName = queryParamName;
            this.condition = condition;
            this.policyParamName = policyParamName ?? queryParamName;
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var providedValue = int.Parse(context.HttpContext.Request.Query[queryParamName]);
            var rule = new IntPolicyRule(policyParamName, value =>
            {
                return condition switch
                {
                    IntCondition.EqualTo => providedValue == value,
                    IntCondition.GreaterThan => providedValue > value,
                    IntCondition.GreaterOrEqualTo => providedValue >= value,
                    IntCondition.LessThan => providedValue < value,
                    IntCondition.LessOrEqualTo => providedValue <= value,
                    _ => throw new NotImplementedException()
                };
            });

            await VerifyPolicyRule(context, next, rule);
        }
    }

    public enum IntCondition
    {
        GreaterThan,
        GreaterOrEqualTo,
        LessThan,
        LessOrEqualTo,
        EqualTo
    }
}
