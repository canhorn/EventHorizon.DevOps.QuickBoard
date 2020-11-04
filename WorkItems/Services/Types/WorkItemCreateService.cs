using EventHorizon.DevOps.QuickBoard.WorkItems.Services.Model;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace EventHorizon.DevOps.QuickBoard.WorkItems.Services.Types
{
    public interface WorkItemCreateService
    {
        Task<WorkItem> Create(
            string accessToken,
            string organization,
            string project,
            string type,
            WorkItemModel model
        );
    }

    public class AzureWorkItemCreateService
        : AzureDevOpsBase,
        WorkItemCreateService
    {
        private static string URL_TEMPLATE => "https://dev.azure.com/{organization}/{project}/_apis/wit/workitems/${type}?api-version=6.0";
        public AzureWorkItemCreateService(
            IHttpClientFactory clientFactory
        ) : base(clientFactory)
        {
        }

        public async Task<WorkItem> Create(
            string accessToken,
            string organization,
            string project,
            string type,
            WorkItemModel model
        )
        {
            var url = URL_TEMPLATE.Replace(
                "{organization}",
                organization
            ).Replace(
                "{project}",
                project
            ).Replace(
                "{type}",
                type
            );

            var result = await Post<WorkItem>(
                url,
                accessToken,
                model
                //new List<object>
                //{
                //    new
                //    {
                //        op = "add",
                //        path = "/fields/System.Title",
                //        value = title,
                //    },
                //    new
                //    {
                //        op = "add",
                //        path = "/fields/System.Description",
                //        value = $"Description for {title}",
                //    },
                //}
            );
            if (!result.Successful)
            {
                return new WorkItem();
            }

            return result.Result;
        }
    }
}
