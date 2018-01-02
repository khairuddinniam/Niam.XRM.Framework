using Microsoft.Xrm.Sdk;
using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace Niam.XRM.Framework.Plugin.ServiceProviders
{
    internal class FilePluginTracingService : ITracingService, IDisposable
    {
        private readonly string _logDirPath;
        private readonly IPluginExecutionContext _context;
        private readonly StringBuilder _logs = new StringBuilder();

        public FilePluginTracingService(string logDirPath, IPluginExecutionContext context)
        {
            _logDirPath = logDirPath ?? throw new ArgumentNullException(nameof(logDirPath));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public void Trace(string format, params object[] args) => 
            _logs.AppendLine(String.Format(CultureInfo.InvariantCulture, format, args));

        public void Dispose() => File.WriteAllText(GetFilePath(), _logs.ToString());

        private string GetFilePath()
        {
            var correlationId = _context.CorrelationId;
            var entityName = _context.PrimaryEntityName;
            var depth = _context.Depth;
            var index = GetIndex();
            var fileName = $"{correlationId:D}-{entityName}-{depth}-{index}.txt";
            var filePath = Path.Combine(_logDirPath, fileName);

            return filePath;
        }

        private int GetIndex()
        {
            var sharedVariables = _context.SharedVariables;
            const string key = "file-log-index";
            var index = sharedVariables.TryGetValue(key, out var previousValue)
                ? (int) previousValue + 1
                : 1;
            sharedVariables[key] = index;

            return index;
        }
    }
}
