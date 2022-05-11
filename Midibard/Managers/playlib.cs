using Dalamud.Game;
using Dalamud.Game.Gui;
using Dalamud.Logging;
using MidiBard.Managers;
using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Collections.Generic;

namespace playlibnamespace
{
    public static class playlib
    {
        private unsafe delegate byte SendActionDelegate(long a1, int a2, void* a3, byte a4);

        private delegate void SetToneUIDelegate(long windowPtr, uint tone);

        private static SendActionDelegate SendActionNative;

        private static Func<string, IntPtr> getWindowByName;

        private static SetToneUIDelegate SetToneUI;

        public unsafe static void init(object plugin)
        {
            Type type = plugin.GetType().Assembly.GetType("MidiBard.DalamudApi.api", throwOnError: true);
            SigScanner sigScanner = (SigScanner)type.GetProperty("SigScanner", BindingFlags.Static | BindingFlags.Public)!.GetValue(null);
            GameGui gui = (GameGui)type.GetProperty("GameGui", BindingFlags.Static | BindingFlags.Public)!.GetValue(null);
            getWindowByName = ((string s) => gui.GetAddonByName(s, 1));
            IntPtr ptr;
            try
            {
                ptr = sigScanner.ScanText("48 8B C4 44 88 48 20 53");
            }
            catch (Exception)
            {
                PluginLog.LogWarning("Exception!");
                ptr = sigScanner.ScanText("E8 ?? ?? ?? ?? 8B 44 24 20 C1 E8 05");
            }

            PluginLog.LogWarning("SendActionNative ADDR: " + MainModuleRva(ptr)); // v6.11 +0x50CE50 void Component::GUI::AtkUnitBase.FireCallback(longlong* param_1, undefined4 param_2, undefined8 param_3, char param_4)
            SendActionNative = Marshal.GetDelegateForFunctionPointer<SendActionDelegate>(ptr);
            PluginLog.LogWarning("SetToneUI ADDR: " + MainModuleRva(sigScanner.ScanText("83 FA 04 77 4E")));
            PluginLog.LogWarning("SetToneUI ADDR2: " + sigScanner.ScanText("83 FA 04 77 4E").ToString("X8"));
            SetToneUI = Marshal.GetDelegateForFunctionPointer<SetToneUIDelegate>(sigScanner.ScanText("83 FA 04 77 4E"));
        }

        public static string MainModuleRva(IntPtr ptr)
        {
            var modules = Process.GetCurrentProcess().Modules;
            List<ProcessModule> mh = new();
            for (int i = 0; i < modules.Count; i++)
                mh.Add(modules[i]);

            mh.Sort((x, y) => (long)x.BaseAddress > (long)y.BaseAddress ? -1 : 1);
            foreach (var module in mh)
            {
                if ((long)module.BaseAddress <= (long)ptr)
                    return $"[{module.ModuleName}+0x{(long)ptr - (long)module.BaseAddress:X}]";
            }
            return $"[0x{(long)ptr:X}]";
        }

        private unsafe static void SendAction(nint ptr, params ulong[] param)
        {
            if (param.Length % 2 != 0)
            {
                throw new ArgumentException("The parameter length must be an integer multiple of 2.");
            }

            if (ptr == IntPtr.Zero)
            {
                throw new ArgumentException("input pointer is null");
            }

            int a = param.Length / 2;
            fixed (ulong* a2 = param)
            {
                SendActionNative(ptr, a, a2, 1);
            }
        }

        public static bool PressKey(int keynumber, ref int offset, ref int octave)
        {
            //PluginLog.LogVerbose("Presskey: " + keynumber + " " + offset + " " + octave);

            Testhooks.Instance.noteOn(keynumber+39);

            return true;
            //if (TargetWindowPtr(out bool miniMode, out IntPtr targetWindowPtr))
            //{
            //    offset = 0;
            //    octave = 0;
            //    if (miniMode)
            //    {
            //        keynumber = ConvertMiniKeyNumber(keynumber, ref offset, ref octave);
            //    }

            //    IntPtr ptr = targetWindowPtr;
            //    ulong[] obj = new ulong[4]
            //    {
            //        3uL,
            //        1uL,
            //        4uL,
            //        0uL
            //    };
            //    obj[3] = (ulong)keynumber;
            //    SendAction(ptr, obj);
            //    return true;
            //}

            //return false;
        }

        public static bool ReleaseKey(int ReleaseKey)
        {
            //PluginLog.LogVerbose("ReleaseKey: " + ReleaseKey);

            Testhooks.Instance.noteOff();

            return true;
            //if (TargetWindowPtr(out bool miniMode, out IntPtr targetWindowPtr))
            //{
            //    if (miniMode)
            //    {
            //        keynumber = ConvertMiniKeyNumber(keynumber);
            //    }

            //    IntPtr ptr = targetWindowPtr;
            //    ulong[] obj = new ulong[4]
            //    {
            //        3uL,
            //        2uL,
            //        4uL,
            //        0uL
            //    };
            //    obj[3] = (ulong)keynumber;
            //    SendAction(ptr, obj);
            //    return true;
            //}

            //return false;
        }

        private static int ConvertMiniKeyNumber(int keynumber)
        {
            keynumber -= 12;
            if (keynumber >= 0)
            {
                if (keynumber > 12)
                {
                    keynumber -= 12;
                }
            }
            else
            {
                keynumber += 12;
            }

            return keynumber;
        }

        private static int ConvertMiniKeyNumber(int keynumber, ref int offset, ref int octave)
        {
            keynumber -= 12;
            if (keynumber >= 0)
            {
                if (keynumber > 12)
                {
                    keynumber -= 12;
                    offset = 12;
                    octave = 1;
                }
            }
            else
            {
                keynumber += 12;
                offset = -12;
                octave = -1;
            }

            return keynumber;
        }

        private static bool TargetWindowPtr(out bool miniMode, out IntPtr targetWindowPtr)
        {
            targetWindowPtr = getWindowByName("PerformanceMode");
            if (targetWindowPtr != IntPtr.Zero)
            {
                miniMode = true;
                return true;
            }

            targetWindowPtr = getWindowByName("PerformanceModeWide");
            if (targetWindowPtr != IntPtr.Zero)
            {
                miniMode = false;
                return true;
            }

            miniMode = false;
            return false;
        }

        public static bool ConfirmReceiveReadyCheck()
        {
            nint num = getWindowByName("PerformanceReadyCheckReceive");
            if (num == IntPtr.Zero)
            {
                return false;
            }

            SendAction(num, 3uL, 2uL);
            return true;
        }

        public static bool GuitarSwitchTone(int tone)
        {
            nint num = getWindowByName("PerformanceToneChange");
            if (num == IntPtr.Zero)
            {
                return false;
            }

            SendAction(num, 3uL, 0uL, 3uL, (ulong)tone);
            SetToneUI((long)(IntPtr)num, (uint)tone);
            return true;
        }

        public static bool BeginReadyCheck()
        {
            nint num = getWindowByName("PerformanceMetronome");
            if (num == IntPtr.Zero)
            {
                return false;
            }

            SendAction(num, 3uL, 2uL, 2uL, 0uL);
            return true;
        }

        public static bool ConfirmBeginReadyCheck()
        {
            nint num = getWindowByName("PerformanceReadyCheck");
            if (num == IntPtr.Zero)
            {
                return false;
            }

            SendAction(num, 3uL, 2uL);
            return true;
        }
    }
}