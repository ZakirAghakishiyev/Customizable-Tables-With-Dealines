using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using Windows.UI.Notifications;

namespace CustomizableTablesWithDeadlines.Infrastructure.Notifications;

public static class WindowsToastRegistration
{
    public const string AppUserModelId = "CustomizableTablesWithDeadlines.DeadlineManager";

    private static bool _registered;

    [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
    private static extern int SetCurrentProcessExplicitAppUserModelID(string appId);

    public static void Register()
    {
        if (_registered)
            return;

        _ = SetCurrentProcessExplicitAppUserModelID(AppUserModelId);
        EnsureStartMenuShortcut();
        _registered = true;
    }

    public static ToastNotifier CreateNotifier()
    {
        Register();
        return ToastNotificationManager.CreateToastNotifier(AppUserModelId);
    }

    private static void EnsureStartMenuShortcut()
    {
        try
        {
            var exePath = Environment.ProcessPath ?? Process.GetCurrentProcess().MainModule?.FileName;
            if (string.IsNullOrWhiteSpace(exePath))
                return;

            var programs = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.StartMenu),
                "Programs");
            Directory.CreateDirectory(programs);

            var shortcutPath = Path.Combine(programs, "Deadline Manager.lnk");
            if (File.Exists(shortcutPath))
                return;

            var shellType = Type.GetTypeFromProgID("WScript.Shell");
            if (shellType is null)
                return;

            dynamic shell = Activator.CreateInstance(shellType)!;
            dynamic shortcut = shell.CreateShortcut(shortcutPath);
            shortcut.TargetPath = exePath;
            shortcut.WorkingDirectory = Path.GetDirectoryName(exePath);
            shortcut.Description = "Deadline Manager";
            shortcut.Save();
        }
        catch
        {
            // Best-effort; toast may still work when explicit AUMID is set.
        }
    }
}
