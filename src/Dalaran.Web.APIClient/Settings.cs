// Copyright (c) Vidol Chalamov.
// See the LICENSE file in the project root for more information.

namespace Dalaran.Web.APIClient
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;

    public class Settings
    {
        public AuthSettings Auth { get; set; }

        public MongoSettings Mongo { get; set; }

        public TemplateSettings Template { get; set; }

        public void Validate()
        {
            var sections = new Dictionary<string, IEnumerable<string>>();

            if (this.Auth == null)
            {
                sections.Add(nameof(this.Auth), new[] { "Missing section." });
            }
            else if (this.Auth.GetValidationErrors().Any() == true)
            {
                sections.Add(nameof(this.Auth), this.Auth.GetValidationErrors());
            }

            if (this.Mongo == null)
            {
                sections.Add(nameof(this.Mongo), new[] { "Missing section." });
            }
            else if (this.Mongo.GetValidationErrors().Any() == true)
            {
                sections.Add(nameof(this.Mongo), this.Mongo.GetValidationErrors());
            }

            if (this.Template == null)
            {
                sections.Add(nameof(this.Template), new[] { "Missing section." });
            }
            else if (this.Template.GetValidationErrors().Any() == true)
            {
                sections.Add(nameof(this.Template), this.Template.GetValidationErrors());
            }

            if (sections.Any())
            {
                var builder = new StringBuilder();
                foreach (var section in sections)
                {
                    var errors = section.Value.Select(value => string.Format(CultureInfo.InvariantCulture, value, section.Key.ToLowerInvariant()));
                    builder.Append($"\r\nErrors in '{section.Key.ToLowerInvariant()}' section:\r\n - {string.Join("\r\n - ", errors)}");
                }

                throw new InvalidOperationException(
                    $@"Validation of configuration settings failed.{builder.ToString()}");
            }
        }

        public sealed class AuthSettings
        {
            public string Authority { get; set; }

            public string ClientId { get; set; }

            public string Secret { get; set; }

            public bool IsValid() => !this.GetValidationErrors().Any();

            public IEnumerable<string> GetValidationErrors()
            {
                if (string.IsNullOrEmpty(this.Authority))
                {
                    yield return $"'{{0}}:{nameof(this.Authority).ToLowerInvariant()}' is null or empty.";
                }

                if (string.IsNullOrEmpty(this.ClientId))
                {
                    yield return $"'{{0}}:{nameof(this.ClientId).ToLowerInvariant()}' is null or empty.";
                }

                if (string.IsNullOrEmpty(this.Secret))
                {
                    yield return $"'{{0}}:{nameof(this.Secret).ToLowerInvariant()}' is null or empty.";
                }
            }
        }

        public sealed class MongoSettings
        {
            public string ConnectionString { get; set; }

            public bool IsValid() => !this.GetValidationErrors().Any();

            public IEnumerable<string> GetValidationErrors()
            {
                if (string.IsNullOrEmpty(this.ConnectionString))
                {
                    yield return $"'{{0}}:{nameof(this.ConnectionString).ToLowerInvariant()}' is null or empty.";
                }
            }
        }

        public sealed class TemplateSettings
        {
            public string Path { get; set; }

            public bool IsValid() => !this.GetValidationErrors().Any();

            public IEnumerable<string> GetValidationErrors()
            {
                if (string.IsNullOrEmpty(this.Path))
                {
                    yield return $"'{{0}}:{nameof(this.Path).ToLowerInvariant()}' is null or empty.";
                }
            }
        }
    }
}