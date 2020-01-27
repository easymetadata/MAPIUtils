using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.IO;

namespace Mapi_Decode_ConvId
/// https://blogs.msmvps.com/infinitec/2007/03/08/decoding-entry-ids/
///https://blogs.msmvps.com/infinitec/2007/03/25/constructing-owa-2007-item-ids-from-webdav-items/
///

{
    public static class ExchangeStoreReference
    {
        public static string DecodeEntryId(BinaryReader reader, string baseUrl)
        {
            Guid folderId;
            ulong folderCnt;

            // First reserved field
            var freserved1 = reader.ReadUInt32();

            // Now comes the store guid.
            var storeguid = reader.ReadBytes(16);
            Guid storeguidG = new Guid(storeguid);
            // Next reserved field
            var freserved2 = reader.ReadUInt16();

            folderId = ReadGuid(reader);
            folderCnt = SwapUInt64(reader.ReadUInt64());

            if (!baseUrl.EndsWith("/"))
            {
                baseUrl += "/";
            }

            if (reader.BaseStream.Length - reader.BaseStream.Position >= 24)
            {
                Guid messageId;
                messageId = ReadGuid(reader);
                ulong messageCnt;
                messageCnt = SwapUInt64(reader.ReadUInt64());

                Console.WriteLine("Position: {0}", reader.BaseStream.Position);

                var sOut = string.Format("{0}, {1}, {2}, {3}, {4}, {5}, {6}", freserved1, storeguidG, freserved2, folderId, folderCnt, messageId, messageCnt);

                Console.WriteLine("freserved1, storeguid, freserved2, folderId, folderCnt, messageId, messageCnt");
                Console.WriteLine(sOut);
                //baseUrl += string.Format(CultureInfo.CurrentCulture, baseUrl + "/ -FlatUrlSpace -/{ 0:N}-{ 1:x}/{ 2:N}-{ 3:x}", folderId, folderCnt, messageId, messageCnt);

                Console.WriteLine("storeguid: {0}", storeguidG);
                
                Console.WriteLine("Bytes 71-79: {0}", reader.ReadUInt64());

                Console.WriteLine("Position: {0}", reader.BaseStream.Position);

                var freserved3 = reader.ReadUInt32();
                Console.WriteLine("freserved3: {0}", freserved3);

                var buffer = new byte[38];

                reader.BaseStream.Seek(8, SeekOrigin.Begin);
                reader.Read(buffer, 0, 32);

                //var muid_guid = new Guid(buffer);
                //var muid = reader.Read(buffer, 9, 32);
                Console.WriteLine("muid: {0}", Encoding.ASCII.GetString(buffer));

                reader.BaseStream.Seek(86, SeekOrigin.Begin);
                reader.Read(buffer, 0, 32);
                var messageId2 = ReadGuid(reader);

                Console.WriteLine("messageId2: {0}", messageId2);
            }
            else
            {
                baseUrl += string.Format(CultureInfo.CurrentCulture, baseUrl + "/ -FlatUrlSpace -/{ 0:N}-{ 1:x}", folderId, folderCnt);
            }

            return baseUrl;
        }

        public static string DecodeEntryIdOnly(BinaryReader reader)
        {
            var baseUrl = string.Empty;

            Guid folderId;
            ulong folderCnt;

            // First reserved field
            reader.ReadUInt32();

            // Now comes the store guid.
            reader.ReadBytes(16);

            // Next reserved field
            reader.ReadUInt16();

            folderId = ReadGuid(reader);
            folderCnt = SwapUInt64(reader.ReadUInt64());


            string smessageId = string.Empty;
            if (reader.BaseStream.Length - reader.BaseStream.Position >= 24)
            {
                Guid messageId;
                messageId = ReadGuid(reader);
                ulong messageCnt;
                messageCnt = SwapUInt64(reader.ReadUInt64());
                //baseUrl += string.Format(CultureInfo.CurrentCulture, baseUrl + "/ -FlatUrlSpace -/{ 0:N}-{ 1:x}/{ 2:N}-{ 3:x}", folderId, folderCnt, messageId, messageCnt);
                smessageId = messageId.ToString();
            }
            else
            {
                //baseUrl += string.Format(CultureInfo.CurrentCulture, baseUrl + "/ -FlatUrlSpace -/{ 0:N}-{ 1:x}", folderId, folderCnt);
            }

            return smessageId;
        }

        private static Guid ReadGuid(BinaryReader reader)
        {
            int a;
            short b, c;
            a = SwapInt(reader.ReadUInt32());
            b = reader.ReadInt16();
            c = SwapShort(reader.ReadUInt16());
            return new Guid(a, b, c, reader.ReadBytes(8));
        }

        private static short SwapShort(ushort value)
        {
            unchecked
            {
                ushort result;
                result = (ushort)(((value & 0xFF00) >> 8) | ((value & 0x00FF) << 8));

                //ushort to short
                var sOut = short.Parse(result.ToString("X"), NumberStyles.HexNumber);
                return (short)sOut;
            }
        }

        private static int SwapInt(uint value)
        {
            uint result;
            result = ((value & 0xFF000000) >> 24) |
            ((value & 0x00FF0000) >> 8) |
            ((value & 0x0000FF00) << 8) |
            ((value & 0x000000FF) << 24);
            unchecked
            {
                return (int)result;
            }
        }

        private static ulong SwapUInt64(ulong value)
        {
            //uint uv = Convert.ToUInt32(value);

            uint lo;
            //uint hi;
            ulong result;
            lo = (uint)(value & 0xffffffff);
            uint hi = (uint)(value >> (32)) & 0xffffffff;

            lo = ((lo & 0xFF000000) >> 8) |
            ((lo & 0x00FF0000) << 8) |
            ((lo & 0x0000FF00) >> 8) |
            ((lo & 0x000000FF) << 8);

            hi = ((hi & 0xFF000000) >> 8) |
            ((hi & 0x00FF0000) << 8) |
            ((hi & 0x0000FF00) >> 8) |
            ((hi & 0x000000FF) << 8);
            result = (((ulong)lo) << 32) | hi;

            return result;
        }
    }
}
