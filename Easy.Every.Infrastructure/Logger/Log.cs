using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using Easy.Every.Infrastructure.UserSettings;
using NLog;
using NLog.Config;

namespace Easy.Every.Infrastructure.Logger;

/// <summary>
/// 日志
/// </summary>
public static class Log
{
    #region 常量
    /// <summary>
    /// 日志文件夹名
    /// </summary>
    private const string FolderName = "Logs";

    /// <summary>
    /// 日志布局(消息格式)<br />
    /// [] 中的内容只有在有异常时才会记录
    /// 14:27:35.7164 - DEBUG - logger - message
    /// [EXCEPTION OCCURS: exceptionType: exceptionMessage]
    /// </summary>
    private const string LogLayout = @"${date:format=HH\:mm\:ss.ffff} - ${level:uppercase=true:padding=-5} - ${logger} - ${message}${onexception:${newline}EXCEPTION OCCURS\: ${exception:format=toString}}";

    /// <summary>
    /// 错误记录器
    /// </summary>
    private const string WrongLogger = "WrongLogger";
    #endregion

    #region 字段
    /// <summary>
    /// 日志目录
    /// </summary>
    private static readonly string LogDirectory = Path.Combine(DataLocation.DataDirectory, FolderName, Constant.Version);
    #endregion

    #region 方法
    #region 构造
    static Log()
    {
        if (!Directory.Exists(LogDirectory)) Directory.CreateDirectory(LogDirectory);

        // 配置 Nlog
        LogManager.Setup().LoadConfiguration(configBuilder =>
        {
#if true
            // 写入文件 每个文件最大 5Mb 只保留最近30天的日志 
            configBuilder.ForLogger().FilterMinLevel(LogLevel.Debug)
            .WriteToFile(Path.Combine(LogDirectory, "${shortdate}.log"), LogLayout, archiveAboveSize: 1024 * 1024 * 5, maxArchiveDays: 30)
            .WithAsync();
            // debug
            configBuilder.ForLogger().FilterMinLevel(LogLevel.Debug).WriteToDebug(LogLayout).WithAsync();
#else
            configBuilder.ForLogger().FilterMinLevel(LogLevel.Info).WriteToFile(Path.Combine(LogDirectory, "${shortdate}.log"), LogLayout).WithAsync();
#endif
        });
    }
    #endregion

    #region 公开
    /// <summary>
    /// 记录异常
    /// </summary>
    /// <param name="logger">记录者</param>
    /// <param name="messgae">信息</param>
    /// <param name="exception">异常</param>
    public static void Exception(string logger, string messgae, Exception exception)
    {
        // 使异常更具有可观测性
        exception = exception.Demystify();
#if DEBUG
        ExceptionDispatchInfo.Capture(exception).Throw();
#else 
        ExceptionInternal(logger, messgae, exception);
#endif
    }
    /// <summary>
    /// 记录异常
    /// </summary>
    /// <param name="className">类名</param>
    /// <param name="messgae">信息</param>
    /// <param name="exception">异常</param>
    /// <param name="methodName">方法名（不用手动传参）</param>
    public static void RecordException(string className, string messgae, Exception exception, [CallerMemberName] string methodName = "") => Exception(CheckLogInfoAndReturnMethodFullName(className, methodName, messgae), messgae, exception);

    /// <summary>
    /// 记录错误
    /// </summary>
    /// <param name="logger">记录者</param>
    /// <param name="message">信息</param>
    public static void Error(string logger, string message) => LogInternal(logger, message, LogLevel.Error);
    /// <summary>
    /// 记录错误
    /// </summary>
    /// <param name="className">类名</param>
    /// <param name="messgae">信息</param>
    /// <param name="methodName">方法名（不用手动传参）</param>
    public static void RecordError(string className, string messgae, [CallerMemberName] string methodName = "") => Error(CheckLogInfoAndReturnMethodFullName(className, methodName, messgae), messgae);

    /// <summary>
    /// 记录警告
    /// </summary>
    /// <param name="logger">记录者</param>
    /// <param name="message">信息</param>
    public static void Warn(string logger, string message) => LogInternal(logger, message, LogLevel.Warn);
    /// <summary>
    /// 记录警告
    /// </summary>
    /// <param name="className">类名</param>
    /// <param name="messgae">信息</param>
    /// <param name="methodName">方法名（不用手动传参）</param>
    public static void RecordWarn(string className, string messgae, [CallerMemberName] string methodName = "") => Warn(CheckLogInfoAndReturnMethodFullName(className, methodName, messgae), messgae);

    /// <summary>
    /// 记录信息
    /// </summary>
    /// <param name="logger">记录者</param>
    /// <param name="message">信息</param>
    public static void Info(string logger, string message) => LogInternal(logger, message, LogLevel.Info);
    /// <summary>
    /// 记录信息
    /// </summary>
    /// <param name="className">类名</param>
    /// <param name="messgae">信息</param>
    /// <param name="methodName">方法名（不用手动传参）</param>
    public static void RecordInfo(string className, string messgae, [CallerMemberName] string methodName = "") => Info(CheckLogInfoAndReturnMethodFullName(className, methodName, messgae), messgae);

    /// <summary>
    /// 记录调试
    /// </summary>
    /// <param name="logger">记录者</param>
    /// <param name="message">信息</param>
    public static void Debug(string logger, string message) => LogInternal(logger, message, LogLevel.Debug);
    /// <summary>
    /// 记录调试
    /// </summary>
    /// <param name="className">类名</param>
    /// <param name="messgae">信息</param>
    /// <param name="methodName">方法名（不用手动传参）</param>
    public static void RecordDebug(string className, string messgae, [CallerMemberName] string methodName = "") => Debug(CheckLogInfoAndReturnMethodFullName(className, methodName, messgae), messgae);

    /// <summary>
    /// 记录痕迹
    /// </summary>
    /// <param name="logger">记录者</param>
    /// <param name="message">信息</param>
    public static void Trace(string logger, string message) => LogInternal(logger, message, LogLevel.Trace);
    /// <summary>
    /// 记录痕迹
    /// </summary>
    /// <param name="className">类名</param>
    /// <param name="messgae">信息</param>
    /// <param name="methodName">方法名（不用手动传参）</param>
    public static void RecordTrace(string className, string messgae, [CallerMemberName] string methodName = "") => Trace(CheckLogInfoAndReturnMethodFullName(className, methodName, messgae), messgae);
    #endregion

    #region 私有
    /// <summary>
    /// 检查日志信息，并且返回完整方法名
    /// </summary>
    /// <param name="className">类名</param>
    /// <param name="methodName">方法名</param>
    /// <param name="message">信息</param>
    /// <returns></returns>
    private static string CheckLogInfoAndReturnMethodFullName(string className, string methodName, string message)
    {
        // 记录的日志信息中没有类名
        if (string.IsNullOrWhiteSpace(className)) LogWrong($"No class name in logged log, message: {message ?? "no message"}");
        // 记录的日志信息中没有方法名
        if (string.IsNullOrWhiteSpace(methodName)) LogWrong($"No method name in logged log, message: {message ?? "no message"}");
        // 记录的日志信息中没有信息
        if (string.IsNullOrWhiteSpace(message)) LogWrong("No message in logged log");

        return $"{className}.{methodName}";
    }

    /// <summary>
    /// 日志有错误时使用的记录者
    /// </summary>
    /// <param name="message">信息</param>
    private static void LogWrong(string message) => LogManager.GetLogger(WrongLogger).Fatal(message);
    /// <summary>
    /// 记录异常
    /// </summary>
    /// <param name="logger">记录者</param>
    /// <param name="message">消息</param>
    /// <param name="exception">异常</param>
    private static void ExceptionInternal(string logger, string message, Exception exception) => LogManager.GetLogger(logger).Error(exception, message);
    /// <summary>
    /// 记录日志
    /// </summary>
    /// <param name="logger">记录者</param>
    /// <param name="message">消息</param>
    /// <param name="logLevel">日志等级</param>
    private static void LogInternal(string logger, string message, LogLevel logLevel) => LogManager.GetLogger(logger).Log(logLevel, message);
    #endregion
    #endregion
}
