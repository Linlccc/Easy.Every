using System.IO.Pipes;
using System.Threading;

namespace Easy.Every.Helper;

/// <summary>
/// 单应用程序接口
/// </summary>
public interface ISingleApplication
{
    /// <summary>
    /// 再次启动应用程序
    /// </summary>
    public void StartApplicationAgain();
}

/// <summary>
/// 单应用程序
/// </summary>
/// <typeparam name="TApplication">应用程序类型</typeparam>
public static class SingleApplication<TApplication> where TApplication : Application, ISingleApplication
{
    #region 常量
    /// <summary>
    /// 名称分隔符
    /// </summary>
    private const string Delimiter = ":";
    /// <summary>
    /// IPC(进程间通信)频道名称后缀
    /// </summary>
    private const string IPCChannelNameSuffix = "SingleApplication";
    #endregion

    #region 字段
    /// <summary>
    /// 单应用程序互斥体
    /// </summary>
    private static Mutex? _singleApplicationMutex;
    #endregion

    #region 方法
    #region 公开
    /// <summary>
    /// 检查是否是第一个
    /// </summary>
    /// <param name="uniqueAppName">唯一应用名称</param>
    /// <returns>如果是第一个返回true，否者返回false</returns>
    public static bool CheckIsFirst(string uniqueAppName)
    {
        // 用户_程序唯一名称
        string userAppName = $"{Environment.UserName}{Delimiter}{uniqueAppName}";
        // IPC管道名称
        string ipcChannelName = $"{userAppName}{Delimiter}{IPCChannelNameSuffix}";

        // 创建互斥体
        _singleApplicationMutex = new(true, userAppName, out bool isFirst);
        // 更具是否是第一个决定是创建服务还是客户端
        ((Action<string>)(isFirst ? CreateIPCService : CreateIPCClient))(ipcChannelName);
        return isFirst;
    }

    /// <summary>
    /// 清理单应用
    /// </summary>
    public static void Cleanup()
    {
        _singleApplicationMutex?.ReleaseMutex();
    }
    #endregion

    #region 私有
    #region 静态
    /// <summary>
    /// 创建IPC服务<br/>
    /// 当该应用多次启动时，通知第一个程序，该程序被再次启动
    /// </summary>
    /// <param name="ipcName">IPC名称</param>
    private static async void CreateIPCService(string ipcName)
    {
        using NamedPipeServerStream pipeServerStream = new(ipcName, PipeDirection.In);
        while (true)
        {
            // 等待连接
            await pipeServerStream.WaitForConnectionAsync();
            // 使用UI线程执行，程序再次启动，当前线程同步等待
            Application.Current?.Dispatcher.Invoke(((TApplication)Application.Current).StartApplicationAgain);
            // 断开本次连接
            pipeServerStream.Disconnect();
        }
    }
    /// <summary>
    /// 创建IPC客户端<br/>
    /// 像一个应用程序发送被再次启动的通知
    /// </summary>
    /// <param name="ipcName"></param>
    private static async void CreateIPCClient(string ipcName)
    {
        using NamedPipeClientStream pipeClientStream = new(".", ipcName, PipeDirection.Out);
        // 发起连接
        await pipeClientStream.ConnectAsync(0);
    }
    #endregion
    #endregion
    #endregion
}
