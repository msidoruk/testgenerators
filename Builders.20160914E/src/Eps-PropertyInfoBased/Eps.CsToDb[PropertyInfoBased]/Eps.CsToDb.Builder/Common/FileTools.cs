using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eps.CsToDb.Builder.Common
{
    public class FileTools
    {
        public static bool IsFileAccessible(string filePath)
        {
            FileStream stream = null;
            try
            {
                FileInfo file = new FileInfo(filePath);
                stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None);
            }
            catch (IOException)
            {
                // the file is unavailable because it is:
                // still being written to
                // or being processed by another thread
                // or does not exist (has already been processed)
                return false;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }
            //file is not locked
            return true;
        }

        public static void WriteTextFile(string path, string content)
        {
            string directoryPath = Path.GetDirectoryName(path);
            DirectoryInfo directoryInfo = new DirectoryInfo(directoryPath);
            if (!directoryInfo.Exists)
                directoryInfo.Create();
            //
            using (var resultCsFileWriter = new StreamWriter(path))
            {
                resultCsFileWriter.WriteLine(content);
            }
        }

        public static void WriteTextFileIfChanged(string path, string content, out bool overriden)
        {
            overriden = false;
            string currentContent = string.Empty;
            if (File.Exists(path))
            {
                using (var fileReader = new StreamReader(path))
                {
                    currentContent = fileReader.ReadToEnd();
                }
            }
            if (string.IsNullOrEmpty(currentContent) || !currentContent.Trim().Equals(content.Trim()) /*We need trim cause WriteTextFile() or ReadToEnd() appends \r\n to end of line*/)
            {
                WriteTextFile(path, content);
                overriden = true;
            }
        }
    }
}
