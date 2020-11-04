using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.TeamFoundation.Work.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace EventHorizon.DevOps.QuickBoard.WorkItems.Services.Types
{
    public interface WorkItemListService
    {
        Task<IEnumerable<WorkItem>> List(
            string accessToken,
            string organization,
            string project,
            string team,
            string backlogId
        );
    }

    public class AzureWorkItemListService
        : AzureDevOpsBase,
        WorkItemListService
    {
        private static string BACKLOG_LEVEL_URL_TEMPLATE => "https://dev.azure.com/{organization}/{project}/{team}/_apis/work/backlogs/{backlogId}/workItems?api-version=6.0-preview.1";
        private static string WORK_ITEMS_URL_TEMPLATE => "https://dev.azure.com/{organization}/{project}/_apis/wit/workitems?ids={ids}&api-version=6.0";

        public AzureWorkItemListService(
            IHttpClientFactory clientFactory
        ) : base(clientFactory)
        {
        }

        public async Task<IEnumerable<WorkItem>> List(
            string accessToken,
            string organization,
            string project,
            string team,
            string backlogId
        )
        {
            var url = BACKLOG_LEVEL_URL_TEMPLATE.Replace(
                "{organization}",
                organization
            ).Replace(
                "{project}",
                project
            ).Replace(
                "{team}",
                team
            ).Replace(
                "{backlogId}",
                backlogId
            );

            var backlogLevelWorkItemsResult = await Get<BacklogLevelWorkItems>(
                url,
                accessToken
            );
            if (!backlogLevelWorkItemsResult.Successful)
            {
                return new List<WorkItem>();
            }

            var workItemIdList = backlogLevelWorkItemsResult.Result.WorkItems.Select(
                a => a.Target.Id
            );
            if (!workItemIdList.Any())
            {
                return new List<WorkItem>();
            }

            url = WORK_ITEMS_URL_TEMPLATE.Replace(
                "{organization}",
                organization
            ).Replace(
                "{project}",
                project
            ).Replace(
                "{ids}",
                string.Join(
                    ',',
                    workItemIdList
                )
            );

            var workItemsResult = await Get<ValueResponse<WorkItem>>(
                url,
                accessToken
            );

            if (!workItemsResult.Successful)
            {
                return new List<WorkItem>();
            }

            return workItemsResult.Result.Value;
        }

        public class ValueResponse<T>
        {
            public int Count { get; set; }
            public IEnumerable<T> Value { get; set; }
        }
    }
}
