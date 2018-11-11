using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace LoadEncryptedAssembly
{
    internal class RunCommand
    {
        private static void RunAssembly(Byte[] buffer)
        {
            Console.WriteLine("Run the embedded assembly");
            var assembly = Assembly.Load(buffer);

            var arguments = new List<Object>();
            foreach(var p in assembly.EntryPoint.GetParameters())
            {
                if (p.ParameterType.IsArray)
                {
                    arguments.Add(Array.CreateInstance(p.ParameterType.GetElementType(), 0));
                }
                else
                {
                    arguments.Add(null);
                }
            }
            assembly.EntryPoint.Invoke(null, arguments.ToArray());
        }

        private static Byte[] LoadEmbeddedResource()
        {
            Byte[] buffer = null;
            var hResourceInfo = NativeMethods.FindResource(IntPtr.Zero, "ENC_DATA", "RT_RCDATA");
            var size = NativeMethods.SizeofResource(IntPtr.Zero, hResourceInfo);
            var hResource = NativeMethods.LoadResource(IntPtr.Zero, hResourceInfo);
            if (hResource != IntPtr.Zero)
            {
                var resource = NativeMethods.LockResource(hResource);

                buffer = new Byte[size];
                Marshal.Copy(resource, buffer, 0, (int)size);
                NativeMethods.FreeResource(hResource);
            }

            return buffer;
        }

        public static void Run()
        {
            var buffer = LoadEmbeddedResource();
            if (buffer != null)
            {
                // read the assembly
                using (var memStream = new MemoryStream(buffer))
                using (var binaryReader = new BinaryReader(memStream))
                {
                    var password = binaryReader.ReadString();
                    var peBuffer = binaryReader.ReadBytes(buffer.Length);
                    Encryption.VmDecrypt(peBuffer, password);
                    RunAssembly(peBuffer);
                }
            }          
        }
    }
}
