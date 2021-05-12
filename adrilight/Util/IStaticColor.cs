

using System.Threading;

namespace adrilight
    {
        public interface IStaticColor
        {
            bool IsRunning { get; }

        void StaticCreator(CancellationToken token);
        }


    }


