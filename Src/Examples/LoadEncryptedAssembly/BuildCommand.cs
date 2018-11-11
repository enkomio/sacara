using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LoadEncryptedAssembly
{
    internal class BuildCommand
    {
        private static String CreateCopy()
        {
            var extension = Path.GetExtension(Assembly.GetExecutingAssembly().Location);
            var newFilename = Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().Location) + ".build" + extension;
            if (File.Exists(newFilename))
            {
                File.Delete(newFilename);
            }
            File.Copy(Assembly.GetExecutingAssembly().Location, newFilename);
            return newFilename;
        }

        public static void Build(String filename, String password)
        {
            var path = Path.GetFullPath(filename);
            if (!File.Exists(path))
            {
                Console.Error.WriteLine("File {0} doesn't exists.", path);
                return;
            }

            var newFilename = CreateCopy();

            // create the buffer
            var peBuffer = File.ReadAllBytes(filename);
            Encryption.ManagedEncrypt(peBuffer, password);

            using (var memStream = new MemoryStream())
            using (var binaryWriter = new BinaryWriter(memStream))
            {
                binaryWriter.Write(password);
                binaryWriter.Write(peBuffer);
                var resourceBuffer = memStream.GetBuffer();
                var buffer = GCHandle.Alloc(resourceBuffer, GCHandleType.Pinned);

                // update the resource                
                var handle = NativeMethods.BeginUpdateResource(newFilename, false);
                var res = NativeMethods.UpdateResource(handle, "RT_RCDATA", "ENC_DATA", 0, buffer.AddrOfPinnedObject(), Convert.ToUInt32(resourceBuffer.Length));
                NativeMethods.EndUpdateResource(handle, false);
            }

            Console.WriteLine("New file '{0}' generated. Run it to execute the program.", newFilename);
        }
    }
}
