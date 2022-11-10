using System.Diagnostics;
using System.Reflection;

namespace Easy.Every.Infrastructure;

/// <summary>
/// 常量
/// </summary>
public static class Constant
{
    public const string EasyEvery = "Easy.Every";

    /// <summary>
    /// 应用程序文件名
    /// </summary>
    public const string ApplicationFileName = $"{EasyEvery}.exe";
    /// <summary>
    /// 程序目录
    /// </summary>
    public static readonly string ProgramDirectory = Directory.GetCurrentDirectory();
    /// <summary>
    /// 应用程序完整文件名
    /// </summary>
    public static readonly string ApplicationFileFullName = Path.Combine(ProgramDirectory, ApplicationFileName);
    /// <summary>
    /// 图片目录
    /// </summary>
    public static readonly string ImagesDirectory = Path.Combine(ProgramDirectory, "Images");

    /// <summary>
    /// 当前程序集
    /// </summary>
    public static readonly Assembly Assembly = typeof(Constant).Assembly;
    /// <summary>
    /// 版本
    /// </summary>
    public static readonly string Version = FileVersionInfo.GetVersionInfo(Assembly.Location).ProductVersion ?? "0.0.1";

}
