using Microsoft.VisualStudio.Services.WebApi.Patch;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;

namespace EventHorizon.DevOps.QuickBoard.WorkItems.Services.Model
{
    public class WorkItemModel
        : JsonPatchDocument
    {
        public WorkItemModel(
            string title,
            string description
        )
        {
            Add(
                new JsonPatchOperation
                {
                    Operation = Operation.Add,
                    Path = "/fields/System.Title",
                    Value = title,
                }
            );
            Add(
                new JsonPatchOperation
                {
                    Operation = Operation.Add,
                    Path = "/fields/System.Description",
                    Value = description,
                }
            );
        }
    }
}
