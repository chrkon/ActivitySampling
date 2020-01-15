using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;

namespace ActivitySampling.Module.View.CLI.Windows
{
    class CliPositioning
    {
        [DllImport("User32.dll")]
        static extern IntPtr FindWindow(String lpClassName, String lpWindowName);
        
        [DllImport("user32.dll", SetLastError = true)]
        static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, SetWindowPosFlags uFlags);

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;        // x position of upper-left corner
            public int Top;         // y position of upper-left corner
            public int Right;       // x position of lower-right corner
            public int Bottom;      // y position of lower-right corner
        }

        // The GetWindowRect function takes a handle to the window as the first parameter. The second parameter
        // must include a reference to a Rectangle object. This Rectangle object will then have it's values set
        // to the window rectangle properties.
        [DllImport("User32.dll")]
        public static extern long GetWindowRect(IntPtr hWnd, ref RECT lpRect);

        static String windowClass = "ConsoleWindowClass";

        enum WinPosition 
        {
            HWND_TOP = 0,
            HWND_BOTTOM = 1,
            HWND_TOPMOST = -1,
            HWND_NOTOPMOST = -2
        }

        [Flags()]
        private enum SetWindowPosFlags : uint
        {
            IgnoreResize = 0x0001,
            IgnoreMove = 0x0002,
            IgnoreZOrder = 0x0004,
            DoNotRedraw = 0x0008,
            DoNotActivate = 0x0010,
            DrawFrame = 0x0020, FrameChanged = 0x0020,
            ShowWindow = 0x0040,
            HideWindow = 0x0080,
            DoNotCopyBits = 0x0100,
            DoNotChangeOwnerZOrder = 0x0200, DoNotReposition = 0x0200,
            DoNotSendChangingEvent = 0x0400,
            DeferErase = 0x2000,
            AsynchronousWindowPosition = 0x4000,
        }

        public static Rectangle GetRectangle()
        {
            String title = Console.Title;  // backup title
            Console.Title += Guid.NewGuid().ToString();  // make window title unique
            IntPtr thisWindow = FindWindow(windowClass, Console.Title);
            Console.Title = title;  // restore original title
            var r = new RECT();            
            GetWindowRect(thisWindow, ref r);
            var rectangle = new Rectangle(r.Left, r.Top, r.Right - r.Left, r.Bottom - r.Top);
            return rectangle;
        }

        public static void SetRectangle(Rectangle r)
        {
            String title = Console.Title;  // backup title
            Console.Title += Guid.NewGuid().ToString();  // make window title unique
            IntPtr thisWindow = FindWindow(windowClass, Console.Title);
            Console.Title = title;  // restore original title
            SetWindowPos(thisWindow, (IntPtr)WinPosition.HWND_TOP, r.X, r.Y, r.Width, r.Height, SetWindowPosFlags.ShowWindow);
        }
    }
}
