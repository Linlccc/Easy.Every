namespace Easy.Every;

public partial class App : ISingleApplication, IDisposable
{
    public void Dispose()
    {
    }

    public void StartApplicationAgain()
    {
        if (Current.MainWindow.WindowState == WindowState.Minimized) Current.MainWindow.WindowState = WindowState.Normal;
        Current.MainWindow.Activate();
    }
}
