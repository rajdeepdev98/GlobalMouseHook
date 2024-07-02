using static ConsoleApp1.src.Utils.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ConsoleApp1.src.Utils.Types;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Windows.Forms;
using System.Windows.Automation;

namespace ConsoleApp1.src.Model
{
    public class MouseHook

    {
       private IntPtr hookId=IntPtr.Zero;
       private HookProc hookProc;
       public POINT point;
       private string exeName;
       private string elementName;
       private int hookType;
       private int action;




        private delegate IntPtr HookProc(int nCode, IntPtr wParam, IntPtr lParam);
       public MouseHook(int hookType,int action)
        {
            this.hookType = hookType;
            this.action = action;
            hookProc = HookCallback;
            hookId = SetHook(hookProc);
            Application.Run();
            UnsetHook();


        }
       

        private IntPtr HookCallback(int  nCode, IntPtr wParam, IntPtr lParam)
        {
            if(nCode>=0 && wParam == (IntPtr)this.action)
            {
                MSLLHOOKSTRUCT hookStruct = (MSLLHOOKSTRUCT)(Marshal.PtrToStructure(lParam,typeof (MSLLHOOKSTRUCT)))  ;
                this.point.x = hookStruct.pt.x;
                this.point.y=hookStruct.pt.y;   

                Point temp=new Point(this.point.x,this.point.y);
                IntPtr hWnd = GetForegroundWindow();
                IntPtr windFoc=WindowFromPoint(temp);
                //Console.WriteLine(windFoc);
                GetWindowThreadProcessId(hWnd, out int processId);
                Process process = Process.GetProcessById(processId);
                this.exeName = process.MainModule.FileName.Split("\\").Last();
                AutomationElement focusedElement = AutomationElement.FocusedElement;

                if (focusedElement != null)
                {
                   // Console.WriteLine("Focused Element: " + focusedElement.Current.ControlType.ProgrammaticName.ToString().Split(".").Last());
                    this.elementName = focusedElement.Current.ControlType.ProgrammaticName.ToString().Split(".").Last();
                }
                else
                {
                    this.elementName = "";
                }
                printData();

            }
            return CallNextHookEx(hookId,nCode, wParam,lParam);
        }
        private IntPtr SetHook(HookProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookExW(this.hookType, proc, GetModuleHandle(curModule.ModuleName), 0);
            }


        }
        private void UnsetHook()
        {
            UnhookWindowsHookEx(this.hookId);
        }

        private void printData()
        {
            Console.WriteLine($"{this.exeName}:{{ X={this.point.x},Y={this.point.y}}} : {this.elementName}");


        }
        [DllImport("user32.dll")]
        private static extern IntPtr SetWindowsHookExW(int idHook, HookProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll")]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll")]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll")]
        private static extern IntPtr GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("user32.dll")]
        private static extern IntPtr WindowFromPoint(Point point);





    }
}
