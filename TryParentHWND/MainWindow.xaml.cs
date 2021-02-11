using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO.Pipes;
using System.IO;

namespace TryParentHWND
{
    public partial class MainWindow : Window
    {

        [DllImport("User32.dll")]
        static extern bool MoveWindow(IntPtr handle, int x, int y, int width, int height, bool redraw);

        internal delegate int WindowEnumProc(IntPtr hwnd, IntPtr lparam);
        [DllImport("user32.dll")]
        internal static extern bool EnumChildWindows(IntPtr hwnd, WindowEnumProc func, IntPtr lParam);

        [DllImport("user32.dll")]
        static extern int SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        private Process process;
        private IntPtr unityHWND = IntPtr.Zero;

        private const int WM_ACTIVATE = 0x0006;
        private readonly IntPtr WA_ACTIVE = new IntPtr(1);
        private readonly IntPtr WA_INACTIVE = new IntPtr(0);

        public MainWindow()
        {
            InitializeComponent();

            try
            {
                process = new Process();
                process.StartInfo.FileName = "UnityGame.exe";
                process.StartInfo.Arguments = "-parentHWND " + panel1.Handle.ToInt32() + " " + Environment.CommandLine;

                Console.WriteLine(process.StartInfo.FileName);
                Console.WriteLine(process.StartInfo.Arguments);

                Console.WriteLine(panel1.Handle);

                process.StartInfo.UseShellExecute = true;
                process.StartInfo.CreateNoWindow = true;

                //process.WaitForInputIdle();

                //Methods.PipeServer.SartDuplexPipe("Pipe");

                process.Start();

                process.WaitForInputIdle();

                // Doesn't work for some reason ?!
                //unityHWND = process.MainWindowHandle;
                 
                EnumChildWindows(panel1.Handle, WindowEnum, IntPtr.Zero);

                //unityHWNDLabel.Text = "Unity HWND: 0x" + unityHWND.ToString("X8");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + ".\nCheck if Container.exe is placed next to UnityGame.exe.");
            }

        }

        private void ActivateUnityWindow()
        {
            Console.WriteLine("Got Focus!");
            SendMessage(unityHWND, WM_ACTIVATE, WA_ACTIVE, IntPtr.Zero);
        }

        private void DeactivateUnityWindow()
        {
            SendMessage(unityHWND, WM_ACTIVATE, WA_INACTIVE, IntPtr.Zero);
        }

        private int WindowEnum(IntPtr hwnd, IntPtr lparam)
        {
            unityHWND = hwnd;
            ActivateUnityWindow();
            return 0;
        }

        private void Window_Resize(object sender, EventArgs e)
        {
            MoveWindow(unityHWND, 0, 0, (int)panel1.Width, (int)panel1.Height, true);
            ActivateUnityWindow();
        }

        // Close Unity application
        private void MainWindow_FormClosed(object sender, EventArgs e)
        {
            Console.WriteLine("Close!");

            try
            {
                process.CloseMainWindow();

                Thread.Sleep(1000);
                while (!process.HasExited)
                    process.Kill();
            }
            catch (Exception)
            {

            }
        }

        private void Button_Click111(object sender, EventArgs e)
        {
            string str = "Hello Console!";
            Console.WriteLine(str);
            Methods.PipeServer.SendMessage(str);
        }

        private void Form1_Activated(object sender, EventArgs e)
        {
            ActivateUnityWindow();
        }

        private void Form1_Deactivate(object sender, EventArgs e)
        {
            DeactivateUnityWindow();
        }

        
    }
}
