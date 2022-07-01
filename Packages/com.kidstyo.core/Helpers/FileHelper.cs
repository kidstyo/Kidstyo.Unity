using System.IO;
using UnityEngine;

namespace KidStyo.Helper
{
    public static class FileHelper
    {
        public static bool IsCreateFolder(string path)
        {
            if (Directory.Exists(path)) return false;
            
            Debug.Log("CreateDirectory:" + path);
            Directory.CreateDirectory(path);
            return true;
        }
        
        public static bool IsCreateFile(string path)
        {
            if (File.Exists(path)) return false;
            
            var dir = Path.GetDirectoryName(path);
            IsCreateFolder(dir);
            
            Debug.Log("CreateFile:" + path);

            var fs = new FileStream(path, FileMode.Create);
            fs.Dispose();
            return true;
        }

        public static string ImportFile(string filePath)
        {
            return File.ReadAllText(filePath);
        }
    }
}