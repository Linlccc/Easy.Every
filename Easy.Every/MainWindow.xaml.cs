namespace Easy.Every;

public partial class MainWindow : Window
{
    private static readonly string ClassName = typeof(MainWindow).FullName ?? typeof(MainWindow).Name;

    public MainWindow()
    {
        InitializeComponent();
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        Log.Exception($"{GetType().FullName}.Button_Click", "123", new Exception("222"));
        Log.RecordException(ClassName, "123", new Exception("222"));
    }
}
