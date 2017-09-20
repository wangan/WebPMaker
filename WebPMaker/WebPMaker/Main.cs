using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using System.Threading;

namespace WebPMaker {
    public partial class Main : Form {
        private ImageList imgList = null;
        private readonly SynchronizationContext synchronizationContext;
        //private System.Windows.Forms.Timer infoTimer = null;
        private string _inputDir = null;

        public Main() {
            InitializeComponent();
            imgView.ListViewItemSorter = new ListViewIndexComparer();
            imgList = new ImageList() {
                ColorDepth = ColorDepth.Depth32Bit
            };
            imgView.LargeImageList = imgList;

            //infoTimer = new System.Windows.Forms.Timer();
            //infoTimer.Interval = 100;
            //infoTimer.Enabled = true;


            synchronizationContext = SynchronizationContext.Current;
        }

        private async void imgView_DragDrop(object sender, DragEventArgs e) {
            var filesOrDirs = e.Data.GetData("FileDrop") as string[];
            if (null != filesOrDirs && filesOrDirs.Length > 0) {

                if (filesOrDirs.Length > 1)
                    MessageBox.Show(this, "UI只支持处理一个动画，如需批处理请用命令行方式！", "提示", MessageBoxButtons.OK);

                List<string> images = new List<string>();
                FileAttributes attr = File.GetAttributes(filesOrDirs[0]);
                if ((attr & FileAttributes.Directory) == FileAttributes.Directory) {
                    _inputDir = filesOrDirs[0];
                    if (!string.IsNullOrEmpty(_inputDir)) {
                        images = Directory.GetFiles(_inputDir, "*.*").ToList();

                        if (null != images && images.Count > 0) {
                            List<Task> imgTasks = new List<Task>();
                            ConcurrentBag<ListViewItem> imgListItems = new ConcurrentBag<ListViewItem>();

                            Mask_Progrs mask = new Mask_Progrs(images.Count) {
                                Dock = DockStyle.Fill
                            };
                            panel2.Controls.Add(mask);
                            mask.BringToFront();

                            //var successToken = new CancellationTokenSource();
                            //var infoTask = Task.Run(async () => {
                            //    while (!successToken.IsCancellationRequested) {


                            //        await Task.Delay(100);
                            //    }
                            //}, successToken.Token);
                            //infoTimer.Tick += new EventHandler((from, ee) => {
                            //    mask.ShowInfo(imgListItems.Count);
                            //});
                            //infoTimer.Enabled = true;
                            //infoTimer.Start();
                            imgView.Clear();

                            foreach (var curImg in images) {
                                try {
                                    if (!curImg.EndsWith("png") && !curImg.EndsWith("PNG"))
                                        continue;

                                    var imgTask = Task.Run(async () => {
                                        var curKey = Path.GetFileNameWithoutExtension(curImg);

                                        using (MemoryStream mStream = new MemoryStream()) {
                                            using (FileStream fStream = new FileStream(curImg, FileMode.Open, FileAccess.Read)) {
                                                var count = 0;
                                                var buffer = new byte[1024];

                                                do {
                                                    count = await fStream.ReadAsync(buffer, 0, 1024);
                                                    await mStream.WriteAsync(buffer, 0, count);
                                                } while (count > 0);

                                                mStream.Seek(0, SeekOrigin.Begin);
                                                var image = Image.FromStream(mStream);

                                                imgView.BeginInvoke(new Action(() => {
                                                    if (imgList.ImageSize.Height == 16)
                                                        imgList.ImageSize = new Size((int)(image.Width * 1.0 / image.Height * 256), 256);

                                                    imgList.Images.Add(curKey, image);
                                                }));

                                                var imgListItem = new ListViewItem() {
                                                    Name = curKey,
                                                    ImageKey = curKey,
                                                    Text = curKey,
                                                    Tag = curImg
                                                };

                                                imgListItems.Add(imgListItem);

                                                //imgView.Invoke(new Action(() => {
                                                //imgView.Items.Add(listItem);
                                                //mask.ShowInfo(imgListItems.Count);
                                                //}));

                                                this.Invoke(new Action(() => {
                                                    mask.ShowInfo(imgListItems.Count);
                                                }));

                                                //synchronizationContext.Post(new SendOrPostCallback(o => {
                                                //    imgView.Items.Add(listItem);
                                                //    mask.ShowInfo(imgListItems.Count);
                                                //}), null);
                                            }
                                        }
                                    });

                                    imgTasks.Add(imgTask);
                                } catch (Exception ex) {
                                }
                            }

                            await Task.WhenAll(imgTasks);
                            //infoTimer.Tick -= new EventHandler((from, ee) => {
                            //    mask.ShowInfo(imgListItems.Count);
                            //});
                            //infoTimer.Stop();
                            var items = imgListItems.ToArray();
                            imgView.Items.AddRange(items);

                            panel2.Controls.Remove(mask);

                        }
                    }
                } else {
                    MessageBox.Show(this, "请选择包含序列图片的目录！", "提示", MessageBoxButtons.OK);
                }
            }

            if (e.Effect == DragDropEffects.Move) {
                var listItem = e.Data.GetData(typeof(ListViewItem));
                if (null != listItem) {
                    int targetIndex = imgView.InsertionMark.Index;
                    if (targetIndex == -1) {
                        return;
                    }
                    if (imgView.InsertionMark.AppearsAfterItem) {
                        targetIndex++;
                    }

                    ListViewItem draggedItem = (ListViewItem)e.Data.GetData(typeof(ListViewItem));
                    imgView.BeginUpdate();

                    imgView.Items.Insert(targetIndex, (ListViewItem)draggedItem.Clone());
                    imgView.Items.Remove(draggedItem);
                    imgView.EndUpdate();
                }
            }
        }

        private void imgView_DragEnter(object sender, DragEventArgs e) {
            var listItem = e.Data.GetData(typeof(ListViewItem));
            if (null != listItem) {
                e.Effect = DragDropEffects.Move;
            } else {
                e.Effect = DragDropEffects.All;
            }
        }

        private void imgView_ItemDrag(object sender, ItemDragEventArgs e) {
            imgView.DoDragDrop(e.Item, DragDropEffects.Move);
        }

        private void imgView_DragOver(object sender, DragEventArgs e) {
            if (e.Effect == DragDropEffects.Move) {
                Point ptScreen = new Point(e.X, e.Y);
                Point pt = imgView.PointToClient(ptScreen);
                ListViewItem item = imgView.GetItemAt(pt.X, pt.Y);

                int targetIndex = imgView.InsertionMark.NearestIndex(pt);
                if (targetIndex > -1) {
                    Rectangle itemBounds = imgView.GetItemRect(targetIndex);
                    if (pt.X > itemBounds.Left + (itemBounds.Width / 2)) {
                        imgView.InsertionMark.AppearsAfterItem = true;
                    } else {
                        imgView.InsertionMark.AppearsAfterItem = false;
                    }
                }
                imgView.InsertionMark.Index = targetIndex;
            }
        }

        private class ListViewIndexComparer : System.Collections.IComparer {
            public int Compare(object x, object y) {
                return ((ListViewItem)x).ImageKey.CompareTo(((ListViewItem)y).ImageKey);
            }
        }

        private async void run_Click(object sender, EventArgs e) {
            int qscale = 75, loop = 0, fps = 11;
            if (!Int32.TryParse(qscaleInput.Text, out qscale)) {
                MessageBox.Show(this, "压缩比为整数，取值范围 [0,100]！", "提示", MessageBoxButtons.OK);
                return;
            }

            if (loopInput.Text != "无限" && !Int32.TryParse(loopInput.Text, out loop)) {
                MessageBox.Show(this, "循环次数为整数（0 代表无限循环）！", "提示", MessageBoxButtons.OK);
                return;
            }

            if (!Int32.TryParse(fpsInput.Text, out fps)) {
                MessageBox.Show(this, "循环次数为整数（0 代表无限循环）！", "提示", MessageBoxButtons.OK);
                return;
            }

            if (!string.IsNullOrEmpty(_inputDir)) {
                List<string> images = new List<string>();
                foreach (ListViewItem cur in imgView.Items) {
                    images.Add(cur.Tag as string);
                }

                if (null != images && images.Count > 0) {

                    WebPConverter converter = new WebPConverter(_inputDir);
                    var imgMap = await converter.Convert(images, qscale);
                    var animateFile = await converter.Animate(imgMap, false, 2, fps, loop);

                    var res = MessageBox.Show(this, $"动画合成成功！\r\n文件保存目录\t{animateFile}\r\n\r\n是否打开目录？", "提示", MessageBoxButtons.YesNo);
                    if (res == DialogResult.Yes) {
                        var animateDir = Path.GetDirectoryName(animateFile);
                        if (Directory.Exists(animateDir)) {
                            await WebPConverter.Execute(animateDir, Environment.GetFolderPath(Environment.SpecialFolder.Windows) + "\\explorer.exe");
                        }
                    }

                    _inputDir = "";
                }
            } else {
                MessageBox.Show(this, "请选择包含序列图片的目录！", "提示", MessageBoxButtons.OK);
            }
        }
    }
}
