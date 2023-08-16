using System.Collections.Generic;
using System.Threading.Tasks;
using Nest;
using QueryEditor.Services.ElasticSearch;
using System;
using ElasticSearchSearchEnhancement.Models.Search;
using System.Linq.Expressions;
using System.Reflection;
using Kovai.Churn360.Customers.Core.Models;

namespace QueryEditor
{
    class Program
    {
        static async Task Main(string[] args)
        {
            ElasticClient elasticClient = ElasticSearchService.GetElasticClient();
            ElasticSearchService elasticSearch = new ElasticSearchService();
            ElasticSearchService.IndexMapping = ElasticSearchService.GetMapping(elasticClient);

            var customerCampaignResponsePath = GetPropertyName<CustomerSearch>(
                    customer => customer.Contacts) + "." + GetPropertyName<CustomerContact>(
                    contact => contact.Campaigns) + "." + GetPropertyName<CustomerContactCampaign>(
                    campaign => campaign.RespondedOn);

            //await ElasticSearchService.IndexChildDocumentAsync(elasticClient, "11");

            await ElasticSearchService.UpdateChildDocumentAsync(elasticClient, "11");
        }

        public static string GetPropertyName<T>(Expression<Func<T, object>> property)
        {
            List<string> stack = new List<string>();
            LambdaExpression lambda = (LambdaExpression)property;
            MemberExpression memberExpression;


            if (lambda.Body is UnaryExpression)
            {
                UnaryExpression unaryExpression = (UnaryExpression)(lambda.Body);
                memberExpression = (MemberExpression)(unaryExpression.Operand);
            }
            else
            {
                memberExpression = (MemberExpression)(lambda.Body);
            }

            var current = memberExpression;


            while (current != null)
            {
                stack.Add(((PropertyInfo)current.Member).Name);

                if (current.Expression is MemberExpression)
                {
                    current = (MemberExpression)current.Expression;
                }
                else
                {
                    current = null;
                }
            }
            stack.Reverse();
            var result = string.Join('.', stack);
            return char.ToLowerInvariant(result[0]) + result.Substring(1);
        }
    }
}