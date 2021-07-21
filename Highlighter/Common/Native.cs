using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace Highlighter.Common {
    public sealed class Native {

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        public static string GetMainWindowClassName(Process target) {
            var buffer = new StringBuilder(256);
            GetClassName(target.MainWindowHandle, buffer, buffer.Capacity);
            return buffer.ToString();
        }
    }
}
