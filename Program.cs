using System;
using System.Runtime.InteropServices;
using System.Diagnostics;

public class WindowFocusListener
{
  private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

  [DllImport("user32.dll")]
  private static extern bool EnumWindows(EnumWindowsProc enumProc, IntPtr lParam);

  [DllImport("user32.dll")]
  private static extern bool IsWindowVisible(IntPtr hWnd);

  [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
  private static extern int GetWindowTextLength(IntPtr hWnd);

  [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
  private static extern int GetWindowText(IntPtr hWnd, System.Text.StringBuilder lpString, int nMaxCount);

  [DllImport("user32.dll")]
  private static extern IntPtr GetForegroundWindow();

  [DllImport("user32.dll")]
  private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

  [DllImport("user32.dll")]
  private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

  [DllImport("user32.dll")]
  private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

  private const uint MOD_CTRL = 0x0002;
  private const uint VK_M = 0x4D;
  private const int HOTKEY_ID = 1;

  static void Main(string[] args)
  {
    Console.WriteLine("Listening for changes to the focused window. The focused window will be minimized automatically, except for the 'overlay' app. Press Ctrl+M to stop the app.");

    IntPtr overlayWindow = IntPtr.Zero; // Replace this with the handle to your "overlay" window
    IntPtr hWnd = Process.GetCurrentProcess().MainWindowHandle;
    RegisterHotKey(hWnd, HOTKEY_ID, MOD_CTRL, VK_M);

    while (true)
    {

      IntPtr newWindow = GetForegroundWindow();

      if (IsWindowVisible(newWindow) && newWindow != overlayWindow)
      {
        int length = GetWindowTextLength(newWindow);
        var builder = new System.Text.StringBuilder(length + 1);
        GetWindowText(newWindow, builder, builder.Capacity);

        string windowTitle = builder.ToString();
        Console.WriteLine("Focused window changed to: " + windowTitle);
        if (windowTitle.Equals("MSI Center", StringComparison.OrdinalIgnoreCase))
        {

          // Minimize the focused window
          ShowWindow(newWindow, 6); // 6 corresponds to minimizing the window
          Console.WriteLine("Hiding: " + windowTitle);
        }
      }
    }
  }
}