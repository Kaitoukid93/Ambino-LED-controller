using adrilight.Resources;
using NLog;
using System;
using System.Threading.Tasks;

namespace adrilight.Util
{
    class AdrilightUpdater
    {
        private readonly ILogger _log = LogManager.GetCurrentClassLogger();
        private const string ADRILIGHT_RELEASES = "https://ambino.net/download-software/";

        public AdrilightUpdater(IUserSettings settings, IContext context)
        {
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
            Context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public void StartThread()
        {
#if !DEBUG
            var t = new Thread(async () => await StartSquirrel())
            {
                Name = "adrilight Update Checker",
                IsBackground = true,
                Priority = ThreadPriority.BelowNormal
            };
            t.Start();
#endif
        }

        public IUserSettings Settings { get; }
        public IContext Context { get; }

        private async Task StartSquirrel()
        {
            while (true)
            {
                try
                {
                    using (var mgr = new UpdateManager(ADRILIGHT_RELEASES))
                    {
                        

                        SquirrelAwareApp.HandleEvents(
          onInitialInstall: v => mgr.CreateShortcutForThisExe(),
          onAppUpdate: v => mgr.CreateShortcutForThisExe(),
          onAppUninstall: v => mgr.RemoveShortcutForThisExe());

                    }
                }
                catch (Exception ex)
                {
                    _log.Error(ex, $"error when update checking: {ex.GetType().FullName}: {ex.Message}");
                }

                //check once a day for updates
                await Task.Delay(TimeSpan.FromDays(1));
            }
        }
    }
}
