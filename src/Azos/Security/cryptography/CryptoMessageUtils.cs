﻿using System;
using System.IO;
using System.Text;

using Azos.Serialization.JSON;

namespace Azos.Security
{
  /// <summary>
  /// Provides extension methods for working with crypto-protected messages
  /// </summary>
  public static class CryptoMessageUtils
  {
    /// <summary>
    /// Protects message into a byte[]. Null is returned for null messages
    /// </summary>
    /// <param name="algorithm">Algorithm to use</param>
    /// <param name="message">Message to protect using Json format</param>
    /// <param name="options">Json format options</param>
    /// <returns>Binary representation of protected message</returns>
    public static byte[] ProtectAsBuffer(this ICryptoMessageAlgorithm algorithm, object message, JsonWritingOptions options = null)
    {
      if (message == null) return null;
      if (options==null) options = JsonWritingOptions.CompactRowsAsMap;
      var raw = JsonWriter.WriteToBuffer(message, options, Encoding.UTF8);
      var result = algorithm.NonNull(nameof(algorithm)).Protect(new ArraySegment<byte>(raw));
      return result;
    }

    /// <summary>
    /// Protects message as web-safe URI string. Null is returned for null messages
    /// </summary>
    /// <param name="algorithm">Algorithm to use</param>
    /// <param name="message">Message to protect using Json format</param>
    /// <param name="options">Json format options</param>
    /// <returns>Web-safe base64-encoded string representation of protected message</returns>
    public static string ProtectAsString(this ICryptoMessageAlgorithm algorithm, object message, JsonWritingOptions options = null)
    {
      var bin = ProtectAsBuffer(algorithm, message, options);
      return bin.ToWebSafeBase64();
    }


    /// <summary>
    /// Tries to decode/unprotect the message, returning null if the protectedMessage does not represent a valid protected message
    /// </summary>
    /// <param name="algorithm">Algorithm to use</param>
    /// <param name="protectedMessage">Protected message content encoded as string</param>
    /// <returns>Unprotected/decoded message or null if the protectedMessage is not valid</returns>
    public static IJsonDataObject Unprotect(this ICryptoMessageAlgorithm algorithm, string protectedMessage)
    {
      var raw = protectedMessage.FromWebSafeBase64();
      if (raw==null) return null;
      return Unprotect(algorithm, new ArraySegment<byte>(raw));
    }

    /// <summary>
    /// Tries to decode/unprotect the message, returning null if the protectedMessage does not represent a valid protected message
    /// </summary>
    /// <param name="algorithm">Algorithm to use</param>
    /// <param name="protectedMessage">Protected message content</param>
    /// <returns>Unprotected/decoded message or null if the protectedMessage is not valid</returns>
    public static IJsonDataObject Unprotect(this ICryptoMessageAlgorithm algorithm, ArraySegment<byte> protectedMessage)
    {
      var raw = algorithm.Unprotect(protectedMessage);
      if (raw==null) return null;
      using(var ms = new MemoryStream(raw))
      {
        try
        {
          return JsonReader.DeserializeDataObject(ms, Encoding.UTF8, true);
        }
        catch
        {
          return null;//corrupted message
        }
      }
    }
  }
}