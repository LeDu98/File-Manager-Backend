using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Constants
{
    public class FileTypeRules
    {
        public static readonly ISet<string> AllowedExtensions =
            new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            { ".txt", ".png", ".jpg", ".jpeg" };

        private static readonly IReadOnlyDictionary<string, string> ExtensionToMime =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                [".txt"] = "text/plain",
                [".png"] = "image/png",
                [".jpg"] = "image/jpeg",
                [".jpeg"] = "image/jpeg",
            };

        public static string NormalizeExtension(string? ext)
        {
            ext ??= string.Empty;
            if (string.IsNullOrWhiteSpace(ext)) return string.Empty;
            return ext.StartsWith('.') ? ext.ToLowerInvariant() : "." + ext.ToLowerInvariant();
        }

        public static bool IsAllowed(string? ext) =>
            AllowedExtensions.Contains(NormalizeExtension(ext));

        public static bool TryGetContentType(string? ext, out string contentType)
        {
            var n = NormalizeExtension(ext);
            if (ExtensionToMime.TryGetValue(n, out var mime))
            {
                contentType = mime;
                return true;
            }
            contentType = "application/octet-stream";
            return false;
        }

        public static string GetContentTypeOrDefault(string? ext) =>
            TryGetContentType(ext, out var ct) ? ct : "application/octet-stream";
    }
}
