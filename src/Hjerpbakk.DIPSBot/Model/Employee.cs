using System;

namespace Hjerpbakk.DIPSBot.Model
{
    public struct Employee : IEquatable<Employee>
    {
        public Employee(string slackUserId, string name)
        {
            SlackUserId = slackUserId;
            Name = name;
        }

        public string SlackUserId { get; }
        public string Name { get; }

        bool IEquatable<Employee>.Equals(Employee other)
            => SlackUserId == other.SlackUserId && Name == other.Name;
    }
}