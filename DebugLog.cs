using ColossalFramework.Plugins;
using System.IO;

namespace InfoViews.Util
{
    public static class DebugLog
    {
        public static void LogToFileOnly(string msg)
        {
            using (FileStream fileStream = new FileStream("InfoViews.txt", FileMode.Append))
            {
                StreamWriter streamWriter = new StreamWriter(fileStream);
                streamWriter.WriteLine(msg);
                streamWriter.Flush();
            }
        }
    }
}