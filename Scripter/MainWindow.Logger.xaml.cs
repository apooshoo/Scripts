using System.Collections.Concurrent;

namespace Scripter
{
    partial class MainWindow
    {
        private int _whileLoopCountTest = 0;
        private void WriteLogsToUiPeriodically(ConcurrentQueue<string> log, CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                _whileLoopCountTest++;
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
        }

        private void WriteLogToUi(string message)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                Output.AppendText(message);
                Output.AppendText(Environment.NewLine);
            }));
        }
    }
}
