using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.MappingExtension
{
    public static class ProjectStatusMapping
    {
        public static Domain.Enums.ProjectStatus ToDomainStatus(this Application.Enums.ProjectStatus source)
        {
            return source switch
            {
                Application.Enums.ProjectStatus.Active => Domain.Enums.ProjectStatus.Active,
                Application.Enums.ProjectStatus.Inactive => Domain.Enums.ProjectStatus.Inactive,
                Application.Enums.ProjectStatus.Archive => Domain.Enums.ProjectStatus.Archive,
                _ => throw new ArgumentException($"Unknown {nameof(Application.Enums.ProjectStatus)} value: {source}")
            };
        }
    }
}
