﻿namespace Domain.Entities
{
    public class ProjectUser
    {
        public int ProjectId { get; set; }

        public int UserId { get; set; }

        public Project Project { get; set; }

        public User User { get; set; }
    }
}
