using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using Hakuu.Properties;
using Microsoft.Win32.SafeHandles;

namespace Hakuu.Utils
{
    public class FileOccupationCheck
    {
        public static int PID;
        public static string ProcessName;
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern uint GetFinalPathNameByHandle(IntPtr hFile, [MarshalAs(UnmanagedType.LPTStr)] System.Text.StringBuilder lpszFilePath, uint cchFilePath, uint dwFlags);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr OpenProcess(uint dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool CloseHandle(IntPtr hObject);

        [Flags]
        enum FileAccessFlags : uint
        {
            FILE_READ_DATA = 0x0001,
            FILE_WRITE_DATA = 0x0002,
            FILE_APPEND_DATA = 0x0004,
            FILE_READ_EA = 0x0008,
            FILE_WRITE_EA = 0x0010,
            FILE_EXECUTE = 0x0020,
            FILE_READ_ATTRIBUTES = 0x0080,
            FILE_WRITE_ATTRIBUTES = 0x0100,
            FILE_ALL_ACCESS = 0x1F01FF,
            FILE_GENERIC_READ = 0x120089,
            FILE_GENERIC_WRITE = 0x120116,
            FILE_GENERIC_EXECUTE = 0x1200A0
        }

        [Flags]
        enum ProcessAccessFlags : uint
        {
            PROCESS_QUERY_INFORMATION = 0x0400,
            PROCESS_VM_READ = 0x0010
        }

        public static bool Check(string filePath, out string ProcessName,out int? PID)
        {
            ProcessName = "";
            PID = 0;
            // 检查文件是否存在
            if (File.Exists(filePath))
            {
                try
                {
                    // 获取占用该文件的所有进程
                    var processes = GetProcessesUsingFile(filePath);

                    if (processes.Count > 0)
                    {
                        Console.WriteLine("以下进程正在使用文件:");
                        foreach (var process in processes)
                        {
                            Console.WriteLine($"进程名称: {process.ProcessName}, PID: {process.Id}");
                            ProcessName = process.ProcessName;
                            PID = process.Id;
                            if (PID == 0)
                            {
                                return true;
                            }
                            return false;
                        }
                    }
                    else
                    {
                        Console.WriteLine("没有进程正在使用文件.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"出现异常: {ex.Message}");
                    return false;
                }
                return true;
            }
            else
            {
                Console.WriteLine("文件不存在.");
                return false;
            }
        }

        static List<Process> GetProcessesUsingFile(string filePath)
        {
            var processes = new List<Process>();
            IntPtr handle = IntPtr.Zero;

            try
            {
                // 打开文件
                handle = OpenFile(filePath);

                // 如果文件成功打开
                if (handle != IntPtr.Zero)
                {
                    // 获取文件所属的进程ID
                    uint processId = GetProcessIdFromHandle(handle);

                    // 获取进程对象
                    Process process = Process.GetProcessById((int)processId);

                    // 如果进程对象不为空，添加到列表中
                    if (process != null)
                    {
                        processes.Add(process);
                    }
                }
            }
            finally
            {
                // 关闭文件句柄
                if (handle != IntPtr.Zero)
                {
                    CloseHandle(handle);
                }
            }

            return processes;
        }

        static IntPtr OpenFile(string filePath)
        {
            // 打开文件句柄
            IntPtr handle = IntPtr.Zero;

            try
            {
                // 打开文件
                handle = CreateFile(filePath, FileAccessFlags.FILE_READ_ATTRIBUTES, FileShare.ReadWrite | FileShare.Delete, IntPtr.Zero, FileMode.Open, 0, IntPtr.Zero);
            }
            catch
            {
                // 忽略打开文件失败的异常
            }

            return handle;
        }

        static uint GetProcessIdFromHandle(IntPtr handle)
        {
            // 获取进程ID
            uint processId = 0;
            GetWindowThreadProcessId(handle, out processId);
            return processId;
        }

        const uint FILE_SHARE_READ = 0x00000001;
        const uint FILE_SHARE_WRITE = 0x00000002;
        const uint OPEN_EXISTING = 3;

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern IntPtr CreateFile(
            string lpFileName,
            FileAccessFlags dwDesiredAccess,
            FileShare dwShareMode,
            IntPtr lpSecurityAttributes,
            FileMode dwCreationDisposition,
            uint dwFlagsAndAttributes,
            IntPtr hTemplateFile);

        [DllImport("user32.dll", SetLastError = true)]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);
    }
}