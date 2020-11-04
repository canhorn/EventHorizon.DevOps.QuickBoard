using EventHorizon.DevOps.QuickBoard.Application.Api;
using EventHorizon.DevOps.QuickBoard.Application.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EventHorizon.DevOps.QuickBoard.Application.Services
{
    public interface ApplicationStateService
        : ApplicationState
    {
        Task Init();
        void OnChanged(
            Func<Task> onChanged
        );
    }

    public class StandardApplicationStateService
        : ApplicationStateService
    {
        private readonly IList<Func<Task>> OnChangedList = new List<Func<Task>>();

        private readonly IJSRuntime _runtime;
        private readonly IConfiguration _configuration;

        public bool IsValid { get; private set; }
        public string Organization { get; private set; } = string.Empty;
        public string Project { get; private set; } = string.Empty;
        public string Team { get; private set; } = string.Empty;
        public string BacklogId { get; private set; } = string.Empty;
        public string WorkItemType { get; private set; } = string.Empty;

        public StandardApplicationStateService(
            IJSRuntime runtime,
            IConfiguration configuration
        )
        {
            _runtime = runtime;
            _configuration = configuration;

            Organization = configuration["Default:Organization"] ?? string.Empty;
            Project = configuration["Default:Project"] ?? string.Empty;
            Team = configuration["Default:Team"] ?? string.Empty;
            BacklogId = configuration["Default:BacklogId"] ?? string.Empty;
            WorkItemType = configuration["Default:WorkItemType"] ?? string.Empty;
        }

        public async Task Init()
        {
            var state = await _runtime.InvokeAsync<ApplicationStateModel>(
                "Interop.getApplicationState"
            );
            if (state != null)
            {
                // Found the State
                await UpdateState(
                    state
                );
            }
        }

        public async Task UpdateState(
            ApplicationStateModel model
        )
        {
            Organization = GetValue(
                model.Organization,
                "Default:Organization"
            );
            Project = GetValue(
                model.Project,
                "Default:Project"
            );
            Team = GetValue(
                model.Team,
                "Default:Team"
            );
            BacklogId = GetValue(
                model.BacklogId,
                "Default:BacklogId"
            );
            WorkItemType = GetValue(
                model.WorkItemType,
                "Default:WorkItemType"
            );

            IsValid = CheckIsValid();

            await _runtime.InvokeVoidAsync(
                "Interop.setApplicationState",
                new ApplicationStateModel
                {
                    Organization = Organization,
                    Project = Project,
                    Team = Team,
                    BacklogId = BacklogId,
                    WorkItemType = WorkItemType,
                }
            );

            foreach (var onChanged in OnChangedList)
            {
                await onChanged();
            }
        }

        private string GetValue(
            string value,
            string key
        )
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                return value;
            }
            return _configuration[key] ?? string.Empty;
        }

        private bool CheckIsValid()
        {
            return !string.IsNullOrWhiteSpace(
                Organization
            ) && !string.IsNullOrWhiteSpace(
                Project
            ) && !string.IsNullOrWhiteSpace(
                Team
            ) && !string.IsNullOrWhiteSpace(
                BacklogId
            ) && !string.IsNullOrWhiteSpace(
                WorkItemType
            );
        }

        public void OnChanged(
            Func<Task> onChanged
        )
        {
            OnChangedList.Add(
                onChanged
            );
        }
    }
}
