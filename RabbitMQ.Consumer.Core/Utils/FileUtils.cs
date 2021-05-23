using System;
using System.IO;

namespace RabbitMQ.Consumer.Core.Utils
{
    public static class FileUtils
    {
        public static string ReadFileFromPath(string filePath)
        {
            var absolutePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filePath);
            var fileContent = File.ReadAllText(absolutePath);

            return fileContent;
        }
    }
}