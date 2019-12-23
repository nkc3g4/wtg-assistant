using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace wintogo.Utility
{
    public class BenchmarkResult
    {
        public double Write4K { get; set; }
        //public double WriteAccTime { get; set; }
        public double WriteSeq { get; set; }

    }
    public class Benchmark
    {
        private long dataLength = 1073737728L;

        private long test4kCount = 2048L;

        private long testAccCount = 128L;

        private static uint FILE_FLAG_NO_BUFFERING = 536870912u;

        private static uint FILE_FLAG_WRITE_THROUGH = 2147483648u;

        private static uint file_flags = FILE_FLAG_NO_BUFFERING | FILE_FLAG_WRITE_THROUGH;

        //private Thread tBench;




        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern SafeFileHandle CreateFile(string lpFileName, FileAccess dwDesiredAccess, FileShare dwShareMode, IntPtr lpSecurityAttributes, FileMode dwCreationDisposition, uint dwFlagsAndAttributes, IntPtr hTemplateFile);

        private void generate_random_array(byte[] rnd_array)
        {
            Random random = new Random();
            for (int i = 0; i < rnd_array.Length; i++)
            {
                rnd_array[i] = (byte)random.Next(255);
            }
        }


        private double write_seq(string path)
        {
            Random random = new Random();
            byte[] array = new byte[4096];
            generate_random_array(array);
           
            SafeFileHandle safeFileHandle = CreateFile(path, FileAccess.ReadWrite, FileShare.None, IntPtr.Zero, FileMode.OpenOrCreate, file_flags, IntPtr.Zero);
            if (safeFileHandle.IsInvalid)
            {
                throw new IOException("Could not open file stream.", new Win32Exception());
            }
            FileStream fileStream = new FileStream(safeFileHandle, FileAccess.ReadWrite, 4096, false);
            Stopwatch stopwatch = new Stopwatch();

            stopwatch.Start();
            fileStream.Position = dataLength;
            fileStream.Write(array, 0, 4096);
            fileStream.Flush();
            stopwatch.Stop();
            fileStream.Close();
            return (dataLength / 1048576.0) / (stopwatch.ElapsedMilliseconds / 1000);
        }
        private double write_file_4k(string path)
        {

            Random random = new Random();
            byte[] array = new byte[4096];
            generate_random_array(array);
            //new FileInfo(path).Delete();
            SafeFileHandle safeFileHandle = CreateFile(path, FileAccess.ReadWrite, FileShare.None, IntPtr.Zero, FileMode.OpenOrCreate, file_flags, IntPtr.Zero);
            if (safeFileHandle.IsInvalid)
            {
                throw new IOException("Could not open file stream.", new Win32Exception());
            }
            FileStream fileStream = new FileStream(safeFileHandle, FileAccess.ReadWrite, 4096, false);
            fileStream.Position = dataLength;
            fileStream.Write(array, 0, 4096);
            long num;
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            for (num = 0L; num < test4kCount; num++)
            {
                int num2 = random.Next((int)(dataLength / 4096 + 1));
                fileStream.Position = num2 * 4096;
                fileStream.Write(array, 0, 4096);
                fileStream.Flush();
            }
            fileStream.Close();
            stopwatch.Stop();
            File.Delete(path);
            return test4kCount * 4096 / 1024 / 1024 / (stopwatch.ElapsedMilliseconds / 1000.0);
        }

        private double write_access(string path)
        {

            Random random = new Random();
            byte[] array = new byte[4096];
            this.generate_random_array(array);
            SafeFileHandle safeFileHandle = CreateFile(path, FileAccess.ReadWrite, FileShare.None, IntPtr.Zero, FileMode.OpenOrCreate, file_flags, IntPtr.Zero);
            if (safeFileHandle.IsInvalid)
            {
                throw new IOException("Could not open file stream.", new Win32Exception());
            }
            FileStream fileStream = new FileStream(safeFileHandle, FileAccess.ReadWrite, 4096, false);
            fileStream.Position = this.dataLength;
            fileStream.Write(array, 0, 4096);
            for (long num2 = 0L; num2 < 64; num2++)
            {
                int num3 = random.Next(262144);
                fileStream.Position = num3 * 4096;
                fileStream.Write(array, 0, 512);
                fileStream.Flush();
            }
            //this.progressBar1.Invoke((Action)delegate
            //{
            //    this.progressBar1.Style = ProgressBarStyle.Blocks;
            //});
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            new Stopwatch();
            long num;
            for (num = 0L; num < this.testAccCount; num++)
            {
                if (num % 4 == 0L)
                {
                    //this.progressBar1.Invoke((Action)delegate
                    //{
                    //    this.progressBar1.Value = (int)((double)num / (double)this.testAccCount * 100.0);
                    //});
                }
                int num4 = random.Next(262144);
                fileStream.Position = num4 * 4096;
                fileStream.Write(array, 0, 512);
                fileStream.Flush();
            }
            fileStream.Close();
            fileStream.Dispose();
            stopwatch.Stop();

            File.Delete(path);
            return (double)stopwatch.ElapsedMilliseconds / (double)this.testAccCount;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="udisk">Like D:</param>
        /// <returns></returns>
        public BenchmarkResult DoBenchmark(string udisk)
        {
            
            string testbin = udisk + "\\test.bin";
            new FileInfo(testbin).Delete();
            double timeseq = write_seq(testbin);
            double time4k = write_file_4k(testbin);
            new FileInfo(testbin).Delete();
            BenchmarkResult result = new BenchmarkResult();
            result.Write4K = time4k;
            result.WriteSeq = timeseq;
            return result;
        }


        //private void button1_Click(object sender, EventArgs e)
        //{
        //    if (this.txtUDisk.Text.Trim() == string.Empty)
        //    {
        //        MessageBox.Show("请选择移动磁盘");
        //    }
        //    else if (this.btnStart.Text == "开始")
        //    {
        //        Form1 form;
        //        double time4k;
        //        double timeAcc;
        //        this.tBench = new Thread((ThreadStart)delegate
        //        {
        //            try
        //            {
        //                form = this;
        //                time4k = this.write_file_4k(this.txtUDisk.Text + "test.bin");
        //                this.txt4kResult.Invoke((Action)delegate
        //                {
        //                    form.txt4kResult.Text = time4k.ToString() + " MB/S";
        //                });
        //                timeAcc = this.write_access(this.txtUDisk.Text + "test.bin");
        //                this.txtAccResult.Invoke((Action)delegate
        //                {
        //                    form.txtAccResult.Text = timeAcc.ToString() + " ms";
        //                });
        //                this.btnStart.Invoke((Action)delegate
        //                {
        //                    this.btnStart.Text = "开始";
        //                });
        //            }
        //            catch (Exception ex)
        //            {
        //                MessageBox.Show(ex.ToString());
        //            }
        //        });
        //        this.tBench.Start();
        //        this.btnStart.Text = "停止";
        //    }
        //    else
        //    {
        //        if (this.tBench != null)
        //        {
        //            this.tBench.Abort();
        //        }
        //        this.btnStart.Text = "开始";
        //    }
        //}



    }
}
