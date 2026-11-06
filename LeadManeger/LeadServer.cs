using System;
using System.Collections.Generic;
using System.Linq;

namespace LeadManager
{
    public class LeadService
    {
        private readonly List<Lead> _leads = new();
        private int _nextId = 1;

        // GET /leads (с фильтрацией по source и name)
        public List<Lead> GetLeads(string? filterSource, string? filterName)
        {
            var query = _leads.AsQueryable();

            if (!string.IsNullOrEmpty(filterSource) && Enum.TryParse<LeadSource>(filterSource, true, out var src))
                query = query.Where(l => l.Source == src);

            if (!string.IsNullOrWhiteSpace(filterName))
                query = query.Where(l => l.Name.Contains(filterName, StringComparison.OrdinalIgnoreCase));

            return query.ToList();
        }

        public Lead? GetLeadById(int id) => _leads.FirstOrDefault(l => l.Id == id);

        public Lead CreateLead(string name, LeadSource source)
        {
            ValidateName(name);

            var lead = new Lead
            {
                Id = _nextId++,
                Name = name,
                Source = source,
                Status = LeadStatus.@new // При создании всегда new
            };
            _leads.Add(lead);
            return lead;
        }

        public bool UpdateLead(int id, string name, LeadSource source, LeadStatus status)
        {
            var lead = GetLeadById(id);
            if (lead == null) return false;

            ValidateName(name);

            lead.Name = name;
            lead.Source = source;
            lead.Status = status;
            return true;
        }

        private void ValidateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name) || name.Length < 2 || name.Length > 60)
                throw new ArgumentException("Имя должно быть от 2 до 60 символов.");
        }
    }
}