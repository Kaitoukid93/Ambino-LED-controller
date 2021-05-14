
using System.Threading;

namespace adrilight
    {
        public interface IMusic
        {
        bool IsRunning { get; }

        void Run(CancellationToken token);
    }


    }


