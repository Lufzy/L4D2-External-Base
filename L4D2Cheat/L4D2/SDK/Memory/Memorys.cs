using System;
using System.Text;
using System.Drawing;
using System.Numerics;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Memorys
{
    class Memorys // https://github.com/Lufzy/Advanced-Memory
    {
        private static IntPtr m_pProcessHandle;

        private static int m_iNumberOfBytesRead;
        private static int m_iNumberOfBytesWritten;

        public static void Initialize(int ProcessID) => m_pProcessHandle = Imports.OpenProcess(PROCESS_VM_OPERATION | PROCESS_VM_READ | PROCESS_VM_WRITE, false, ProcessID);

        public static T Read<T>(int address) where T : struct
        {
            int ByteSize = Marshal.SizeOf(typeof(T));

            byte[] buffer = new byte[ByteSize];

            Imports.ReadProcessMemory((int)m_pProcessHandle, address, buffer, buffer.Length, ref m_iNumberOfBytesRead);

            return ByteArrayToStructure<T>(buffer);
        }

        public static byte[] ReadMemory(int offset, int size)
        {
            byte[] buffer = new byte[size];

            Imports.ReadProcessMemory((int)m_pProcessHandle, offset, buffer, size, ref m_iNumberOfBytesRead);

            return buffer;

        }

        public static bool ReadBytes(int StartingAdress, ref byte[] output)
        {
            if (Imports.ReadProcessMemory((int)m_pProcessHandle, StartingAdress, output, output.Length, ref m_iNumberOfBytesRead)) return true;
            else return false;
        }

        public static byte[] ReadBytes(int StartingAdress, int length)
        {
            byte[] output = new byte[length];
            if (Imports.ReadProcessMemory((int)m_pProcessHandle, StartingAdress, output, output.Length, ref m_iNumberOfBytesRead)) return output;
            else return null;
        }

        public static string ReadStringASCII(int offset, int size)
        {
            return Imports.CutString(Encoding.ASCII.GetString(ReadMemory(offset, size)));
        }
        public static string ReadStringUnicode(int offset, int size)
        {
            return Imports.CutString(Encoding.Unicode.GetString(ReadMemory(offset, size)));
        }
        public static string ReadStringUTF(int offset, int size)
        {
            return Imports.CutString(Encoding.UTF8.GetString(ReadMemory(offset, size)));
        }

        public static float[] ReadMatrix<T>(int address, int MatrixSize) where T : struct
        {
            int ByteSize = Marshal.SizeOf(typeof(T));

            byte[] buffer = new byte[ByteSize * MatrixSize];

            Imports.ReadProcessMemory((int)m_pProcessHandle, address, buffer, buffer.Length, ref m_iNumberOfBytesRead);

            return ConvertToFloatArray(buffer);
        }

        public static void Write<T>(int address, object Value) where T : struct
        {
            byte[] buffer = StructureToByteArray(Value);

            Imports.WriteProcessMemory((int)m_pProcessHandle, address, buffer, buffer.Length, out m_iNumberOfBytesWritten);
        }

        #region Extensions | WorldToScreen & FindDMAAddy
        public static Vector2 WorldToScreen(Vector3 target, int width, int height, float[] viewMatrix /*float[16]*/)
        {
            Vector2 _worldToScreenPos;
            Vector3 to;
            float w = 0.0f;
            float[] viewmatrix = new float[16];
            viewmatrix = viewMatrix;

            to.X = viewmatrix[0] * target.X + viewmatrix[1] * target.Y + viewmatrix[2] * target.Z + viewmatrix[3];
            to.Y = viewmatrix[4] * target.X + viewmatrix[5] * target.Y + viewmatrix[6] * target.Z + viewmatrix[7];

            w = viewmatrix[12] * target.X + viewmatrix[13] * target.Y + viewmatrix[14] * target.Z + viewmatrix[15];

            // behind us
            if (w < 0.01f)
                return new Vector2(0, 0);

            to.X *= (1.0f / w);
            to.Y *= (1.0f / w);

            //int width = Main.ScreenSize.Width;
            //int height = Main.ScreenSize.Height;

            float x = width / 2;
            float y = height / 2;

            x += 0.5f * to.X * width + 0.5f;
            y -= 0.5f * to.Y * height + 0.5f;

            to.X = x;
            to.Y = y;

            _worldToScreenPos.X = to.X;
            _worldToScreenPos.Y = to.Y;
            return _worldToScreenPos;
        }

        public static IntPtr FindDMAAddy(IntPtr ptr, int[] offsets) // https://stackoverflow.com/questions/35788512/c-sharp-need-to-add-one-offset-to-two-addresses-that-leave-up-to-my-value
        {
            var buffer = new byte[IntPtr.Size];
            foreach (int i in offsets)
            {
                Imports.ReadProcessMemory((int)m_pProcessHandle, (int)ptr, buffer, buffer.Length, ref m_iNumberOfBytesRead);

                ptr = (IntPtr.Size == 4)
                ? IntPtr.Add(new IntPtr(BitConverter.ToInt32(buffer, 0)), i)
                : ptr = IntPtr.Add(new IntPtr(BitConverter.ToInt64(buffer, 0)), i);
            }
            return ptr;
        }
        #endregion

        #region SigScan
        public static int FindPattern(Module module, byte[] pattern, string mask)
        {
            byte[] moduleBytes = new byte[module.Size];

            if (ReadBytes(module.Address, ref moduleBytes))
            {
                for (int i = 0; i < module.Size; i++)
                {
                    bool found = true;

                    for (int l = 0; l < mask.Length; l++)
                    {
                        found = mask[l] == '?' || moduleBytes[l + i] == pattern[l];

                        if (!found)
                            break;
                    }

                    if (found)
                        return i;
                }
            }

            return 0;
        }
        public static Int32 FindPattern(Module module, string sig, int offset, int extra, bool relative)
        {
            byte[] moduleDump = new byte[module.Size];
            int moduleAddress = module.Address;

            if (ReadBytes(module.Address, ref moduleDump))
            {
                byte[] pattern = SignatureToPattern(sig);
                string mask = GetSignatureMask(sig);
                IntPtr address = IntPtr.Zero;

                for (int i = 0; i < module.Size; i++)
                {
                    if (address == IntPtr.Zero && pattern.Length + i < module.Size)
                    {
                        bool isSuccess = true;

                        for (int k = 0; k < pattern.Length; k++)
                        {
                            if (mask[k] == '?')
                                continue;


                            if (pattern[k] != moduleDump[i + k])
                                isSuccess = false;
                        }

                        if (!isSuccess) continue;

                        if (address == IntPtr.Zero)
                        {
                            if (relative)
                            {
                                return BitConverter.ToInt32(ReadBytes(module.Address + i + offset, 4), 0) + extra - module.Address;
                            }
                            else
                            {
                                return BitConverter.ToInt32(ReadBytes(module.Address + i + offset, 4), 0) + extra;
                            }
                        }
                    }
                }
            }

            return -1;
        }

        private static byte[] SignatureToPattern(string sig)
        {
            string[] parts = sig.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            byte[] patternArray = new byte[parts.Length];

            for (int i = 0; i < parts.Length; i++)
            {
                if (parts[i] == "?")
                {
                    patternArray[i] = 0;
                    continue;
                }

                if (!byte.TryParse(parts[i], System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.DefaultThreadCurrentCulture, out patternArray[i]))
                {
                    throw new Exception();
                }
            }

            return patternArray;
        }

        private static string GetSignatureMask(string sig)
        {
            string[] parts = sig.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            string mask = "";

            for (int i = 0; i < parts.Length; i++)
            {
                if (parts[i] == "?")
                {
                    mask += "?";
                }
                else
                {
                    mask += "x";
                }
            }

            return mask;
        }

        private static string GetFullfilledMask(byte[] buffer)
        {
            string result = "";
            for (int i = 0; i < buffer.Length; i++)
            {
                result += "x";
            }
            return result;
        }
        #endregion

        #region Module Object
        public class Module
        {
            public Process process;
            public string Name;
            public int Address;
            public int Size;

            public Module(Process proc, string nName)
            {
                process = proc;
                Name = nName;
                Get();
            }

            private void Get()
            {
                foreach (ProcessModule m in process.Modules)
                {
                    if (m.ModuleName == Name)
                    {
                        Address = (Int32)m.BaseAddress;
                        Size = (Int32)m.ModuleMemorySize;
                    }
                }
            }

            public string ToString()
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Process => " + process.ProcessName);
                sb.AppendLine("Name    => " + Name);
                sb.AppendLine("Address => 0x" + Address.ToString("X"));
                sb.AppendLine("Size    => 0x" + Size.ToString("X"));
                return sb.ToString();
            }
        }
        #endregion

        #region Transformation

        public static float[] ConvertToFloatArray(byte[] bytes)
        {
            if (bytes.Length % 4 != 0) throw new ArgumentException();

            float[] floats = new float[bytes.Length / 4];

            for (int i = 0; i < floats.Length; i++) floats[i] = BitConverter.ToSingle(bytes, i * 4);

            return floats;
        }

        private static T ByteArrayToStructure<T>(byte[] bytes) where T : struct
        {
            GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);

            try
            {
                return (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
            }
            finally
            {
                handle.Free();
            }
        }

        private static byte[] StructureToByteArray(object obj)
        {
            int length = Marshal.SizeOf(obj);

            byte[] array = new byte[length];

            IntPtr pointer = Marshal.AllocHGlobal(length);

            Marshal.StructureToPtr(obj, pointer, true);
            Marshal.Copy(pointer, array, 0, length);
            Marshal.FreeHGlobal(pointer);

            return array;
        }

        #endregion

        #region Constants

        const int PROCESS_VM_OPERATION = 0x0008;
        const int PROCESS_VM_READ = 0x0010;
        const int PROCESS_VM_WRITE = 0x0020;

        #endregion

        #region Imports
        public static class Imports
        {
            [DllImport("user32.dll")]
            public static extern bool GetWindowRect(IntPtr hWnd, Rectangle rect);

            [DllImport("user32.dll")] //watcher
            public static extern bool SetForegroundWindow(IntPtr hWnd);

            [DllImport("user32.dll")]
            public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

            [DllImport("user32.dll")]
            public static extern bool PostMessage(IntPtr hWnd, UInt32 Msg, int wParam, int lParam);

            [DllImport("user32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

            [DllImportAttribute("user32.dll")] //move menu
            public static extern bool ReleaseCapture();

            [DllImportAttribute("user32.dll")]
            public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
            //colo
            [DllImport("user32.dll")]
            public static extern bool GetCursorPos(ref Point lpPoint);

            [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
            public static extern int BitBlt(IntPtr hDC, int x, int y, int nWidth, int nHeight, IntPtr hSrcDC, int xSrc, int ySrc, int dwRop);

            [DllImport("user32.dll")]
            public static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);

            [DllImport("kernel32.dll")]
            public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

            [DllImport("kernel32.dll")]
            public static extern bool ReadProcessMemory(int hProcess, int lpBaseAddress, byte[] buffer, int size, ref int lpNumberOfBytesRead);

            [DllImport("kernel32.dll")]
            public static extern bool CloseHandle(int hObject);
            public static string CutString(string mystring)
            {
                char[] chArray = mystring.ToCharArray();
                string str = "";
                for (int i = 0; i < mystring.Length; i++)
                {
                    if ((chArray[i] == ' ') && (chArray[i + 1] == ' '))
                    {
                        return str;
                    }
                    if (chArray[i] == '\0')
                    {
                        return str;
                    }
                    str = str + chArray[i].ToString();
                }
                return mystring.TrimEnd(new char[] { '0' });
            }

            [DllImport("kernel32.dll")]
            public static extern bool WriteProcessMemory(int hProcess, int lpBaseAddress, byte[] buffer, int size, out int lpNumberOfBytesWritten);

            [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
            public static extern IntPtr GetForegroundWindow();

            [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            public static extern int GetWindowThreadProcessId(IntPtr handle, out int processId);

            [DllImport("User32.dll")]
            public static extern short GetAsyncKeyState(System.Windows.Forms.Keys vKey);
        }
        #endregion
    }
}