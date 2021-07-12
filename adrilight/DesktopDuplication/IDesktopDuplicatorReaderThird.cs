using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;

namespace adrilight
{
    public interface IDesktopDuplicatorReaderThird
    {
        bool IsRunning { get; }

        void Run(CancellationToken token);
        void Stop();
        void RefreshCapturingState();
    }
}