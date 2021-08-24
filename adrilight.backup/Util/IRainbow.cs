
using System.Threading;

namespace adrilight
    {
        public interface IRainbow
        {
        bool IsRunning { get; }

        void Run(CancellationToken token);
    }


    }


