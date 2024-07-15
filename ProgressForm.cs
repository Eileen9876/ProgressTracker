using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System;

namespace mylib
{
    public partial class ProgressForm : Form
    {
        private ProgressTrack Track;

        public ProgressForm(ProgressTrack track)
        {
            InitializeComponent();

            this.StartPosition = FormStartPosition.CenterParent;

            label.Text = "";

            Track = track;
        }

        public void Record_Progress(string msg)
        {
            label.Text = msg;
        }

        public void Set_CancelButtonClick(CancellationTokenSource cts, Task task)
        {
            cancel_button.Click += (sender, e) => 
            {
                cts.Cancel();
                task.Wait();
                this.Close();
            };
        }

        private void cancel_button_Click(object sender, EventArgs e)
        {
            Track.Cancel();
        }
    }

    public class ProgressTrack
    {
        private delegate void FormClose();

        private delegate void FormSetMsg(string msg);

        public delegate void Action();

        private Control Control;

        private ProgressForm Form;

        private CancellationTokenSource CTS;

        private Task Task;

        /// <summary>
        /// 假如發生例外狀況則回傳例外訊息。
        /// </summary>
        public Exception GetException { get; private set; }

        public ProgressTrack(Control control)
        {
            this.Control = control;
            this.Form = new ProgressForm(this);
            this.GetException = null;
        }

        ~ProgressTrack() 
        {
            this.Control = null;
            this.Form.Dispose();
            this.Form = null;
            this.CTS.Dispose(); 
            this.CTS = null;
            this.GetException = null;
        }

        /// <summary>
        /// 初始並執行。
        /// </summary>
        /// <param name="control">父控制項</param>
        /// <param name="action">任務函式</param>
        /// <returns></returns>
        public static ProgressTrack Run(Control control, Action action)
        {
            ProgressTrack track = new ProgressTrack(control);

            track.Run(action);

            return track;
        }

        /// <summary>
        /// 開始執行。
        /// </summary>
        /// <param name="action">任務函式</param>
        public void Run(Action action)
        {
            if (this.CTS != null) this.CTS.Dispose();

            this.CTS = new CancellationTokenSource();

            this.Task = Task.Run(() =>
            {
                try
                {
                    action();
                }
                catch (Exception ex) 
                {
                    GetException = ex;
                }
                finally
                {
                    Close(); //form close
                }
            },
            this.CTS.Token);

            this.Form.Record_Progress("");

            this.Form.ShowDialog(this.Control);

            if (!this.CTS.IsCancellationRequested) this.Task.Wait();
        }

        /// <summary>
        /// 發出「取消訊號」。
        /// </summary>
        public void Cancel()
        {
            if (this.CTS != null && !this.CTS.IsCancellationRequested) this.CTS.Cancel();
        }

        /// <summary>
        /// 確認是否有「取消訊號」，假如有則中斷任務並退出，可透過 GetException 接收「取消訊號」產生的例外訊息。
        /// </summary>
        public void CancelCheck()
        {
            this.CTS.Token.ThrowIfCancellationRequested();
        }

        /// <summary>
        /// 關閉視窗
        /// </summary>
        public void Close()
        {
            if (this.Control.InvokeRequired)
            {
                FormClose close = new FormClose(this.Form.Close);

                this.Control.Invoke(close);

                return;
            }

            this.Form.Close();
        }

        /// <summary>
        /// 設置視窗訊息
        /// </summary>
        public void SetMsg(string msg)
        {
            if (this.Control.InvokeRequired)
            {
                FormSetMsg set_msg = new FormSetMsg(this.Form.Record_Progress);

                this.Control.Invoke(set_msg, msg);

                return;
            }

            this.Form.Record_Progress(msg);
        }
    }
}

