using System.Collections.Concurrent;

namespace Scripter
{
    public partial class MainWindow
    {
        private Task StartLogging(ConcurrentQueue<string> log, CancellationTokenSource cancelTokenSource)
            => Task.Run(() => WriteLogsToUiPeriodically(log, cancelTokenSource.Token), cancelTokenSource.Token);

        private DateTime _startTime;

        private void WriteLogsToUiPeriodically(ConcurrentQueue<string> log, CancellationToken token)
        {
            _startTime = DateTime.Now;
            while (!token.IsCancellationRequested)
            {
                TryWriteLogToUI(log);
                Thread.Sleep(100);
            }
        }

        private void TryWriteLogToUI(ConcurrentQueue<string> log)
        {
            if (log.TryDequeue(out var message))
            {
                if (!string.IsNullOrEmpty(message))
                {
                    WriteLogToUi(message);
                }
            }
        }

        private void WriteRemainingLogs(ConcurrentQueue<string> log)
        {
            while (!log.IsEmpty)
            {
                TryWriteLogToUI(log);
            }
            WriteLogToUi("Operation complete. " +
                "Time elapsed: " + (DateTime.Now - _startTime).TotalSeconds + " seconds.");
        }

        private void WriteLogToUi(string message)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                Output.AppendText(message);
                Output.AppendText(Environment.NewLine);
            }));
        }

        private void ClearLog()
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                Output.Clear();
            }));
        }
    }
}
