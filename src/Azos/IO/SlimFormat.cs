/*<FILE_LICENSE>
 * Azos (A to Z Application Operating System) Framework
 * The A to Z Foundation (a.k.a. Azist) licenses this file to you under the MIT license.
 * See the LICENSE file in the project root for more information.
</FILE_LICENSE>*/

using System;
using System.Text;

namespace Azos.IO
{
    /// <summary>
    /// A format that writes into binary files in an efficient way using variable-length integers, strings and meta handles.
    /// Developers may derive new formats that support custom serialization of their business-related types. This may increase performance dramatically.
    /// For example, in a drawing application a new format may derive from SlimFormat to natively serialize Point and PolarPoint structs to yield faster serialization times.
    /// Azos.Serialization.Slim.SlimSlimSerializer is capable of SlimFormat-derived format injection, in which case it will automatically discover new types that are directly supported
    /// by the format.
    /// </summary>
    public class SlimFormat : StreamerFormat<SlimReader, SlimWriter>
    {
       public const int MAX_BYTE_ARRAY_LEN =  512 * //mb
                                             1024 * //kb
                                             1024;

       public const int MAX_INT_ARRAY_LEN =  MAX_BYTE_ARRAY_LEN / 4;

       public const int MAX_LONG_ARRAY_LEN =  MAX_BYTE_ARRAY_LEN / 8;

       public const int MAX_DOUBLE_ARRAY_LEN =  MAX_BYTE_ARRAY_LEN / 8;

       public const int MAX_FLOAT_ARRAY_LEN =  MAX_BYTE_ARRAY_LEN / 4;

       public const int MAX_DECIMAL_ARRAY_LEN =  MAX_BYTE_ARRAY_LEN / 13;

       public const int MAX_STRING_ARRAY_CNT =  MAX_BYTE_ARRAY_LEN / 48;


       public const int STR_BUF_SZ = 96 * 1024;// ensure placement in LOH
                                               // in many business cases Slim Serializes pretty big chunks of text:
                                               // NLS pairs containing full page product markup (2-8 Kbytes of text per ISO Lang)
                                               // pre-serialized JSON fragments i.e. 6-8 kb

       public const int MAX_STR_LEN = (STR_BUF_SZ / 3) - 16; //3 bytes per UTF8 character - 16 BOM etc.
                                                              //this is done on purpose NOT to call
                                                              //Encoding.GetByteCount()

       [ThreadStatic] internal static byte[] ts_Buff32;

       [ThreadStatic] internal static byte[] ts_StrBuff;

       protected SlimFormat() : base()
       {
          TypeSchema = new Serialization.Slim.TypeSchema(this);
       }

       private static SlimFormat s_Instance = new SlimFormat();

       /// <summary>
       /// Returns a singleton format instance
       /// </summary>
       public static SlimFormat Instance
       {
         get { return s_Instance;}
       }


       public override Type ReaderType
       {
           get { return typeof(SlimReader); }
       }

       public override Type WriterType
       {
           get { return typeof(SlimWriter); }
       }


       /// <summary>
       /// Internally references type schema
       /// </summary>
       internal readonly Serialization.Slim.TypeSchema TypeSchema;


       public override SlimReader MakeReadingStreamer(Encoding encoding = null)
       {
           return new SlimReader();
       }

       public override SlimWriter MakeWritingStreamer(Encoding encoding = null)
       {
           return new SlimWriter();
       }
    }
}
