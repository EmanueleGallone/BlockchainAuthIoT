﻿using BlockchainAuthIoT.DataProvider.Models.Policies.Rules;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BlockchainAuthIoT.DataProvider.Attributes
{
    public class ValidateListParamAttribute : PolicyParamFilterAttribute
    {
        private readonly string queryParamName;
        private readonly ListCondition condition;
        private readonly string policyParamName;

        /// <summary>
        /// Adds blockchain policy-based validation for a list parameter, encoded as a string
        /// of comma-separated values.
        /// </summary>
        /// <param name="queryParamName">The name of the parameter in the query string</param>
        /// <param name="condition">The condition when comparing it to the policy-enforced rule</param>
        /// <param name="policyParamName">The name of the parameter in the policy</param>
        public ValidateListParamAttribute(string queryParamName, ListCondition condition, string policyParamName = null)
        {
            this.queryParamName = queryParamName;
            this.condition = condition;
            this.policyParamName = policyParamName ?? queryParamName;
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var providedValues = context.HttpContext.Request.Query[queryParamName].ToString().Split(',');
            var rule = new StringPolicyRule(policyParamName, value =>
            {
                if (condition == ListCondition.AllContainedIn)
                {
                    var allowedList = value.Split(',');
                    return providedValues.All(v => allowedList.Contains(v));
                }

                return false;
            });

            await VerifyPolicyRule(context, next, rule);
        }
    }

    public enum ListCondition
    {
        AllContainedIn
    }
}
