namespace Easy.Every;

public class Program
{
    #region 常量
    /// <summary>
    /// 程序名称
    /// </summary>
    private const string ApplicationName = "Easy.Every";
    #endregion

    [STAThread]
    public static void Main()
    {
        if (!SingleApplication<App>.CheckIsFirst(ApplicationName)) return;
        using App app = new();
        app.InitializeComponent();
        app.Run();
    }
}
