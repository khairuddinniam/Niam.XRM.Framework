using System;

namespace Niam.XRM.Framework.Plugin
{
    [AttributeUsage(AttributeTargets.Assembly)]
    public sealed class FilePluginLoggingAttribute : Attribute
    {
        public string DirPath { get; }

        public FilePluginLoggingAttribute(string dirPath)
        {
            DirPath = dirPath ?? throw new ArgumentNullException(nameof(dirPath));
        }
    }
}
