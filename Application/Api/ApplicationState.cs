using EventHorizon.DevOps.QuickBoard.Application.Model;
using System.Threading.Tasks;

namespace EventHorizon.DevOps.QuickBoard.Application.Api
{
    public interface ApplicationState
    {
        bool IsValid { get; }
        string Organization { get; }
        string Project { get; }
        string Team { get; }
        string BacklogId { get; }
        string WorkItemType { get; }

        Task UpdateState(
            ApplicationStateModel model
        );
    }
}
