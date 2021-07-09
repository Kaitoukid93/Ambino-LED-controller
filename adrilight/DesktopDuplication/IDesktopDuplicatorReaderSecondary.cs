using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;

namespace adrilight
{
    public interface IDesktopDuplicatorReaderSecondary
    {
        bool IsRunning { get; }

        void Run(CancellationToken token);
    }
}