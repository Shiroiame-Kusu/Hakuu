using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Hakuu.Utils
{
    public class PropertyOperation : System.Collections.Hashtable
    {
        private string? fileName;
        public static int? FileOccupiedProcessPID;
        public static string? FileOccupiedProcessName;
        private ArrayList list = new ArrayList();
        public ArrayList List
        {
            get { return list; }
            set { list = value; }
        }
        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="fileName">Ҫ��д��properties�ļ���</param>
        public PropertyOperation(string fileName)
        {
            PropertyOperator(fileName);
        }
        
        public void PropertyOperator(string fileName)
        {
            this.fileName = fileName;
            this.Load(fileName);
        }
        /// <summary>
        /// ��д����ķ���
        /// </summary>
        /// <param name="key">��</param>
        /// <param name="value">ֵ</param>
        public override void Add(object key, object? value)
        {
            base.Add(key, value);
            list.Add(key);

        }

        public void Update(object key, object value)
        {
            base.Remove(key);
            list.Remove(key);
            this.Add(key, value);

        }
        public override ICollection Keys
        {
            get
            {
                return list;
            }
        }
        /// <summary>
        /// �����ļ�
        /// </summary>
        /// <param name="filePath">�ļ�·��</param>
        private void Load(string filePath)
        {
            char[] convertBuf = new char[1024];
            int limit;
            int keyLen;
            int valueStart;
            char c;
            string bufLine = string.Empty;
            bool hasSep;
            bool precedingBackslash;
            using (StreamReader? sr = new StreamReader(filePath))
            {
                while (sr.Peek() >= 0)
                {
                    bufLine = sr.ReadLine();
                    limit = bufLine.Length;
                    keyLen = 0;
                    valueStart = limit;
                    hasSep = false;
                    precedingBackslash = false;
                    if (bufLine.StartsWith("#"))
                        keyLen = bufLine.Length;
                    while (keyLen < limit)
                    {
                        c = bufLine[keyLen];
                        if ((c == '=' || c == ':') & !precedingBackslash)
                        {
                            valueStart = keyLen + 1;
                            hasSep = true;
                            break;
                        }
                        else if ((c == ' ' || c == '\t' || c == '\f') & !precedingBackslash)
                        {
                            valueStart = keyLen + 1;
                            break;
                        }
                        if (c == '\\')
                        {
                            precedingBackslash = !precedingBackslash;
                        }
                        else
                        {
                            precedingBackslash = false;
                        }
                        keyLen++;
                    }
                    while (valueStart < limit)
                    {
                        c = bufLine[valueStart];
                        if (c != ' ' && c != '\t' && c != '\f')
                        {
                            if (!hasSep && (c == '=' || c == ':'))
                            {
                                hasSep = true;
                            }
                            else
                            {
                                break;
                            }
                        }
                        valueStart++;
                    }
                    string key = bufLine.Substring(0, keyLen);
                    string values = bufLine.Substring(valueStart, limit - valueStart);
                    if (key == "")
                        key += "#";
                    while (key.StartsWith("#") & this.Contains(key))
                    {
                        key += "#";
                    }
                    this.Add(key, values);
                }
            }
        }
        public bool FileCheck(string filePath)
        {
            Process[] processes = Process.GetProcesses();
            foreach (Process process in processes)
            {
                try
                {
                    // 获取进程打开的文件句柄
                    foreach (ProcessModule module in process.Modules)
                    {
                        if (module.FileName.Equals(filePath, StringComparison.OrdinalIgnoreCase))
                        {
                            Console.WriteLine($"Process Name: {process.ProcessName}, PID: {process.Id}");
                            return false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    // 忽略访问拒绝或其他异常
                    Console.WriteLine($"Error accessing process {process.ProcessName}: {ex.Message}");
                }
            }
            return true;
            
        }
        /// <summary>
        /// �����ļ�
        /// </summary>
        /// <param name="filePath">Ҫ������ļ���·��</param>
        public bool Save()
        {
            string? filePath = this.fileName;
            if (File.Exists(filePath))
            {
                int PID;
                string ProcessName;
                /*if (FileOccupationCheck.Check(filePath,out FileOccupiedProcessName,out FileOccupiedProcessPID))
                {
                    File.Delete(filePath);
                    
                }
                else
                {   
                    //FileOccupiedProcessPID = PID;
                    //FileOccupiedProcessName = ProcessName;
                    
                    return false;
                }*/
                File.Delete(filePath);
            }
            FileStream fileStream = File.Create(filePath);
            StreamWriter sw = new StreamWriter(fileStream);
            foreach (object item in list)
            {
                String key = (String)item;
                String val = (String)this[key];
                if (key.StartsWith("#"))
                {
                    if (val == "")
                    {
                        sw.WriteLine(key);
                    }
                    else
                    {
                        sw.WriteLine(val);
                    }
                }
                else
                {
                    sw.WriteLine(key + "=" + val);
                }
            }
            sw.Close();
            fileStream.Close();
            return true;
        }
    }
}