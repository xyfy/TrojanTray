using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Trojan.Tray
{


    public class TrojanHelper : IDisposable
    {
        Process process = null;
        private static char DirectorySeparatorChar => Path.DirectorySeparatorChar;
        string exePath = null;
        string workDir = null;


        public string ExePath
        {
            get
            {
                return exePath;
            }
        }

        public delegate void DataReceived(string text, DataReceivedType dataReceivedType);

        public event DataReceived DataReceivedEvent;

        public TrojanHelper()
        {
            workDir = $"{Environment.CurrentDirectory}{ DirectorySeparatorChar }trojan";
            exePath = System.IO.Path.Combine(workDir, $"trojan.exe");
        }
        public void Start()
        {
            process = new Process();
            var processStartInfo = process.StartInfo;
            processStartInfo.WorkingDirectory = workDir;
            processStartInfo.RedirectStandardOutput = true;
            processStartInfo.RedirectStandardError = true;
            processStartInfo.CreateNoWindow = true;
            processStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            processStartInfo.UseShellExecute = false;
            processStartInfo.FileName = exePath;
            process.OutputDataReceived += Process_OutputDataReceived;
            process.ErrorDataReceived += Process_ErrorDataReceived;
            process.Start();
            process.BeginOutputReadLine();//开始读取输出数据
            process.BeginErrorReadLine();//开始读取错误数据，重要！
        }

        private void Process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                this.DataReceivedEvent(e.Data, DataReceivedType.Error);
            }
        }

        private void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                this.DataReceivedEvent(e.Data, DataReceivedType.Output);
            }
        }

        public void Close()
        {
            if (process != null && !process.HasExited)
            {
                process.Kill(true);
            }
        }
        public void Dispose()
        {
            if (process != null && !process.HasExited)
            {
                process.Dispose();
            }
        }
    }
}