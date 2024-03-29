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
            // ����ļ��Ƿ����
            if (File.Exists(filePath))
            {
                try
                {
                    // ��ȡռ�ø��ļ������н���
                    var processes = GetProcessesUsingFile(filePath);

                    if (processes.Count > 0)
                    {
                        Console.WriteLine("���½�������ʹ���ļ�:");
                        foreach (var process in processes)
                        {
                            Console.WriteLine($"��������: {process.ProcessName}, PID: {process.Id}");
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
                        Console.WriteLine("û�н�������ʹ���ļ�.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"�����쳣: {ex.Message}");
                    return false;
                }
                return true;
            }
            else
            {
                Console.WriteLine("�ļ�������.");
                return false;
            }
        }

        static List<Process> GetProcessesUsingFile(string filePath)
        {
            var processes = new List<Process>();
            IntPtr handle = IntPtr.Zero;

            try
            {
                // ���ļ�
                handle = OpenFile(filePath);

                // ����ļ��ɹ���
                if (handle != IntPtr.Zero)
                {
                    // ��ȡ�ļ������Ľ���ID
                    uint processId = GetProcessIdFromHandle(handle);

                    // ��ȡ���̶���
                    Process process = Process.GetProcessById((int)processId);

                    // ������̶���Ϊ�գ���ӵ��б���
                    if (process != null)
                    {
                        processes.Add(process);
                    }
                }
            }
            finally
            {
                // �ر��ļ����
                if (handle != IntPtr.Zero)
                {
                    CloseHandle(handle);
                }
            }

            return processes;
        }

        static IntPtr OpenFile(string filePath)
        {
            // ���ļ����
            IntPtr handle = IntPtr.Zero;

            try
            {
                // ���ļ�
                handle = CreateFile(filePath, FileAccessFlags.FILE_READ_ATTRIBUTES, FileShare.ReadWrite | FileShare.Delete, IntPtr.Zero, FileMode.Open, 0, IntPtr.Zero);
            }
            catch
            {
                // ���Դ��ļ�ʧ�ܵ��쳣
            }

            return handle;
        }

        static uint GetProcessIdFromHandle(IntPtr handle)
        {
            // ��ȡ����ID
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