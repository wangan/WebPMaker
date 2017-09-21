using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebPMaker {
    public class WebPConverter {
        private Dictionary<int, TaskCompletionSource<bool>> TSC = new Dictionary<int, TaskCompletionSource<bool>>();
        public static string Exe_cwebp { get; set; } = $@"{AppDomain.CurrentDomain.BaseDirectory}\libwebp\libwebp-0.6.0-windows-x64-no-wic\bin\cwebp.exe";
        public static string Exe_webpmux { get; set; } = $@"{AppDomain.CurrentDomain.BaseDirectory}\libwebp\libwebp-0.6.0-windows-x64-no-wic\bin\webpmux.exe";
        private string _imgDir { get; set; }
        private string _tempDir { get; set; }
        private string _tempFileName { get; set; }
        private string _outDir { get; set; }
        private string _outFileName { get; set; }

        public WebPConverter(string inputDir, string outName = "animate") {
            _imgDir = inputDir;
            _tempDir = inputDir + "/temp/";
            _tempFileName = inputDir + "/temp/" + "{0}.webp";
            _outDir = inputDir + "/output/";
            _outFileName = inputDir + $"\\output\\{outName}.WebP";

            if (!Directory.Exists(_tempDir))
                Directory.CreateDirectory(_tempDir);

            if (!Directory.Exists(_outDir))
                Directory.CreateDirectory(_outDir);
        }

        public async Task<string> Animate(Dictionary<int, string> imgMap, bool trim = false, int skip = 2, double fps = 11, int loop = 0) {
            var sb = new StringBuilder();
            sb.AppendFormat("-bgcolor 255,255,255,255 -loop {0} ", loop);
            var images = imgMap.OrderBy(I => I.Key).Select(I => I.Value).ToList();
            for (int i = 0; i < images.Count; i++) {
                if (trim && i % skip != 0)
                    continue;

                var cur = images[i];
                sb.AppendFormat(" -frame \"{0}\" +{1}+0+0+1+b", cur, (int)(1000.0 / fps));
            }

            sb.Append($" -o \"{_outFileName}\"");
            var cmdStr = sb.ToString();

            await Execute(cmdStr, Exe_webpmux);

            return _outFileName;
        }

        public async Task<Dictionary<int, string>> Convert(List<string> images, int q = 75) {
            Dictionary<int, string> imageDic = new Dictionary<int, string>();
            List<Task> convTasks = new List<Task>();
            List<string> cmd = new List<string>();

            for (int i = 0; i < images.Count; i++) {
                var curImg = images[i];
                var outFile = string.Format(_tempFileName, i + 1);

                var cmdStr = string.Format("-q {0} \"{1}\" -o \"{2}\"", q, curImg, outFile);
                var convTask = Task.Run(async () => {
                    await Convert(cmdStr);
                });

                convTasks.Add(convTask);
                imageDic[i] = outFile;
            }

            await Task.WhenAll(convTasks);

            return imageDic;
        }

        async Task Convert(string cmd) {
            await Execute(cmd, Exe_cwebp);
        }

        public static async Task Execute(string cmd, string exe) {
            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
            var webp = new Process() {
                EnableRaisingEvents = true,
                StartInfo = new ProcessStartInfo() {
                    FileName = exe,
                    Arguments = cmd,
                    CreateNoWindow = true,
                    UseShellExecute = false
                }
            };

            webp.Exited += new EventHandler((sender, e) => {
                tcs.SetResult(true);
            });

            webp.Start();
            webp.WaitForExit();

            await tcs.Task;
        }
    }
}
