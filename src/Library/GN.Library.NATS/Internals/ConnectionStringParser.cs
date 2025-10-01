using System;
using System.Collections.Generic;
using System.Text;

namespace GN.Library.Nats.Internals
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class ConnectionStringParser
    {
        private readonly Dictionary<string, string> _parameters;

        public ConnectionStringParser(string connectionString)
        {
            _parameters = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            Parse(connectionString);
        }

        private void Parse(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                return;

            var parts = SplitConnectionString(connectionString);

            foreach (var part in parts)
            {
                var equalIndex = part.IndexOf('=');
                if (equalIndex > 0 && equalIndex < part.Length - 1)
                {
                    var key = part.Substring(0, equalIndex).Trim();
                    var value = part.Substring(equalIndex + 1).Trim();

                    // Remove quotes if present
                    if (value.StartsWith("'") && value.EndsWith("'") && value.Length > 1)
                        value = value.Substring(1, value.Length - 2);
                    else if (value.StartsWith("\"") && value.EndsWith("\"") && value.Length > 1)
                        value = value.Substring(1, value.Length - 2);

                    _parameters[key] = value;
                }
            }
        }

        private List<string> SplitConnectionString(string connectionString)
        {
            var parts = new List<string>();
            var current = new StringBuilder();
            var inQuotes = false;
            var quoteChar = '\0';

            for (int i = 0; i < connectionString.Length; i++)
            {
                var ch = connectionString[i];

                if (!inQuotes && (ch == '\'' || ch == '"'))
                {
                    inQuotes = true;
                    quoteChar = ch;
                    current.Append(ch);
                }
                else if (inQuotes && ch == quoteChar)
                {
                    inQuotes = false;
                    quoteChar = '\0';
                    current.Append(ch);
                }
                else if (!inQuotes && ch == ';')
                {
                    if (current.Length > 0)
                    {
                        parts.Add(current.ToString().Trim());
                        current.Clear();
                    }
                }
                else
                {
                    current.Append(ch);
                }
            }

            // Add the last part if there's content
            if (current.Length > 0)
            {
                parts.Add(current.ToString().Trim());
            }

            return parts;
        }

        public string GetValue(string key)
        {
            return _parameters.TryGetValue(key, out var value) ? value : null;
        }

        public string GetValue(string key, string defaultValue)
        {
            return _parameters.TryGetValue(key, out var value) ? value : defaultValue;
        }

        public bool HasKey(string key)
        {
            return _parameters.ContainsKey(key);
        }

        public Dictionary<string, string> GetAllParameters()
        {
            return new Dictionary<string, string>(_parameters, StringComparer.OrdinalIgnoreCase);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (var kvp in _parameters)
            {
                if (sb.Length > 0)
                    sb.Append("; ");

                // Add quotes if value contains spaces or semicolons
                var value = kvp.Value;
                if (value.Contains(" ") || value.Contains(";"))
                    value = $"'{value}'";

                sb.Append($"{kvp.Key}={value}");
            }
            return sb.ToString();
        }
    }

    // Example usage and test class
    
}
