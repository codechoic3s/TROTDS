using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interop;
using System.Windows.Shapes;
using TROTDS.Logging;

namespace TROTDS
{
    public static class Utils
    {
        public static event Action<string> OnError;
        public static bool TryFileExists(string path, LogTask logTask = null)
        {
            logTask?.Log($"Verifing file {path}...");
            if (File.Exists(path))
            {
                logTask?.Log($"File is exists {path}!");
                return true;
            }
            var msg = $"File not exists on {path}.";
            OnError?.Invoke(msg);
            logTask?.Log(msg);
            return false;
        }
        public static bool TryDirectoryExists(string path, LogTask logTask = null)
        {
            logTask?.Log($"Verifing dir {path}...");
            if (Directory.Exists(path))
            {
                logTask?.Log($"Dir is exists {path}!");
                return true;
            }
            var msg = $"Dir not exists on {path}.";
            OnError?.Invoke(msg);
            logTask?.Log(msg);
            return false;
        }
        public static bool TryKillProcess(this Process process, LogTask logTask = null)
        {
            try
            {
                logTask?.Log($"Killing process...");
                process.Kill();
            }
            catch (Exception e)
            {
                var msg = $"Failed kill process. ({e.Message})";
                OnError?.Invoke(msg);
                logTask?.Log(msg);
                return false;
            }
            logTask?.Log($"Process killed!");
            return true;
        }
        public static bool TryProcessWaitForExit(this Process process, int ms = 0, LogTask logTask = null)
        {
            try
            {
                if (ms == 0)
                {
                    process.WaitForExit();
                    var msg = $"Process catched as stopped.";
                    logTask?.Log(msg);
                    return true;
                }
                else if (process.WaitForExit(ms))
                {
                    var msg = $"Process catched as stopped.";
                    logTask?.Log(msg);
                    return true;
                }
            }
            catch (Exception e)
            {
                var msg = $"Critical error on Process.WaitForExit. ({e.Message})";
                OnError?.Invoke(msg);
                logTask?.Log(msg);
                return false;
            }
            return false;
        }
        public static bool TryProcessStart(this Process process, LogTask logTask = null)
        {
            try
            {
                logTask?.Log($"Starting process ...");
                if (!process.Start())
                {
                    var msg = $"Failed execute .";
                    OnError?.Invoke(msg);
                    logTask?.Log(msg);
                    return false;
                }
            }
            catch (Exception e)
            {
                var msg = $"Critical error on execution process. ({e.Message})";
                OnError?.Invoke(msg);
                logTask?.Log(msg);
                return false;
            }
            logTask?.Log($"Process executed!");
            return true;
        }
        public static bool TryDeleteDirectory(string path, LogTask logTask = null)
        {
            try
            {
                logTask?.Log($"Deleting dir {path}...");
                Directory.Delete(path, true);
            }
            catch (Exception e)
            {
                var msg = $"Failed delete dir {path}. ({e.Message})";
                OnError?.Invoke(msg);
                logTask?.Log(msg);
                return false;
            }
            logTask?.Log($"Deleted dir {path}!");
            return true;
        }
        public static bool TryCreateDirectory(string path, LogTask logTask = null)
        {
            try
            {
                logTask?.Log($"Creating dir {path}...");
                Directory.CreateDirectory(path);
            }
            catch (Exception e)
            {
                var msg = $"Failed create dir {path}. ({e.Message})";
                OnError?.Invoke(msg);
                logTask?.Log(msg);
                return false;
            }
            logTask?.Log($"Created dir {path}!");
            return true;
        }
        public static bool TrySafeWork(string name, Action action, LogTask logTask = null)
        {
            try
            {
                logTask?.Log($"Work {name} started...");
                action?.Invoke();
            }
            catch (Exception e)
            {
                var msg = $"Work {name} catched exception. ({e.Message})";
                OnError?.Invoke(msg);
                logTask?.Log(msg);
                return false;
            }
            logTask?.Log($"Work {name} completed!");
            return true;
        }
        public static bool TryFileWriteAllBytes(string path, byte[] data, LogTask logTask = null)
        {
            try
            {
                logTask?.Log($"Writing bytes into file {path}...");
                File.WriteAllBytes(path, data);
            }
            catch (Exception e)
            {
                var msg = $"Failed write bytes into file {path}. ({e.Message})";
                OnError?.Invoke(msg);
                logTask?.Log(msg);
                return false;
            }
            logTask?.Log($"Writed bytes into file {path}!");
            return true;
        }
        public static bool TryFileWriteAllText(string path, string data, LogTask logTask = null)
        {
            try
            {
                logTask?.Log($"Writing text into file {path}...");
                File.WriteAllText(path, data);
            }
            catch (Exception e)
            {
                var msg = $"Failed write text into file {path}. ({e.Message})";
                OnError?.Invoke(msg);
                logTask?.Log(msg);
                return false;
            }
            logTask?.Log($"Writed text into file {path}!");
            return true;
        }
        public static bool TryZipFileExtractToDirectory(string pathZip, string pathRaw, LogTask logTask = null)
        {
            try
            {
                logTask?.Log($"Extracting zip from {pathZip} to {pathRaw}...");
                ZipFile.ExtractToDirectory(pathZip, pathRaw);
            }
            catch (Exception e)
            {
                var msg = $"Failed extract zip from {pathZip} to {pathRaw}. ({e.Message})";
                OnError?.Invoke(msg);
                logTask?.Log(msg);
                return false;
            }
            logTask?.Log($"Extracted zip from {pathZip} to {pathRaw}!");
            return true;
        }
        public static bool TryDeleteFile(string path, LogTask logTask = null)
        {
            try
            {
                logTask?.Log($"Deleting file {path}...");
                File.Delete(path);
            }
            catch (Exception e)
            {
                var msg = $"Failed delete file {path}. ({e.Message})";
                OnError?.Invoke(msg);
                logTask?.Log(msg);
                return false;
            }
            logTask?.Log($"Deleted file {path}!");
            return true;
        }
        public static bool TryCopyFile(string sourcePath, string destPath, LogTask logTask = null)
        {
            try
            {
                logTask?.Log($"Copying file from {sourcePath} to {destPath}...");
                File.Copy(sourcePath, destPath);
            }
            catch (Exception e)
            {
                var msg = $"Failed copy file from {sourcePath} to {destPath}. ({e.Message})";
                OnError?.Invoke(msg);
                logTask?.Log(msg);
                return false;
            }
            logTask?.Log($"Copied file from {sourcePath} to {destPath}.");
            return true;
        }
    }
}
