namespace Easy.Every.Infrastructure.UserSettings;

/// <summary>
/// 数据位置
/// </summary>
public static class DataLocation
{
    /// <summary>
    /// 便携数据文件夹名
    /// </summary>
    public const string PortableDataFolderName = "UserData";
    /// <summary>
    /// 便携目录
    /// </summary>
    public static readonly string PortableDataDirectory = Path.Combine(Constant.ProgramDirectory, PortableDataFolderName);
    /// <summary>
    /// 本地数据目录
    /// </summary>
    public static readonly string LocalDataDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Constant.EasyEvery);

    /// <summary>
    /// 数据目录
    /// </summary>
    public static string DataDirectory => IsUsePortableData() ? PortableDataDirectory : LocalDataDirectory;

    /// <summary>
    /// 是否使用便携数据
    /// </summary>
    /// <returns></returns>
    public static bool IsUsePortableData() => Directory.Exists(PortableDataDirectory);
}
