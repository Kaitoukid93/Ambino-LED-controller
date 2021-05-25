

using System.Threading;

namespace adrilight
    {
        public interface IStaticColor
        {
            bool IsRunning { get; }

        void Run(CancellationToken token);
        }


    }


