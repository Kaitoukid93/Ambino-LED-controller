
using System.Threading;

namespace adrilight
    {
        public interface IGifxelation
        {
        bool IsRunning { get; }

        void Run(CancellationToken token);
    }


    }


