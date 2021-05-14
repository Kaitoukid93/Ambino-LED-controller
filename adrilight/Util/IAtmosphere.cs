
using System.Threading;

namespace adrilight
    {
        public interface IAtmosphere
        {
        bool IsRunning { get; }

        void Run(CancellationToken token);
    }


    }


