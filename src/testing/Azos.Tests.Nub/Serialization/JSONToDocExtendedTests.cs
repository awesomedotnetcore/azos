/*<FILE_LICENSE>
 * Azos (A to Z Application Operating System) Framework
 * The A to Z Foundation (a.k.a. Azist) licenses this file to you under the MIT license.
 * See the LICENSE file in the project root for more information.
</FILE_LICENSE>*/

using System;
using System.Diagnostics;

using Azos.Scripting;

using Azos.Serialization.JSON;
using Azos.Collections;
using System.IO;
using Azos.Data;
using Azos.Time;
using Azos.Financial;
using System.Collections.Generic;

namespace Azos.Tests.Nub.Serialization
{
  [Runnable]
  public class JSONToDocExtendedTests
  {

    public struct CustomStructType : IJsonWritable, IJsonReadable
    {
      public CustomStructType(string text)
      {
        Text = text;
        Length = text==null? 0: text.Length;
      }

      public string Text;
      public int Length;

      public (bool match, IJsonReadable self) ReadAsJson(object data, bool fromUI, JsonReader.NameBinding? nameBinding)
      {
        if (data==null) return (false, this);

        var str = data as string;
        if (str==null) str = data.ToString();

        Text = str;
        Length = str.Length;
        return (true, this);
      }

      public void WriteAsJson(TextWriter wri, int nestingLevel, JsonWritingOptions options = null)
      {
        JsonWriter.EncodeString(wri, Text, options);
      }
    }

    public class CustomDoc1 : TypedDoc
    {
      [Field] public string ID{ get;set;}
      [Field] public CustomStructType Data{ get;set;}
    }

    public class CustomDoc2 : TypedDoc
    {
      [Field] public string ID { get; set; }
      [Field] public CustomStructType? Data { get; set; }
    }

    public class CustomDoc3 : TypedDoc
    {
      [Field] public string ID { get; set; }
      [Field] public CustomStructType[] Data { get; set; }
    }

    public class CustomDoc4 : TypedDoc
    {
      [Field] public string ID { get; set; }
      [Field] public List<CustomStructType> Data { get; set; }
    }


    [Run]
    public void CustomWritableReadable_1()
    {
      var d1 = new CustomDoc1{ ID = "meduza1", Data =  new CustomStructType("Custom string 1") };

      var json = d1.ToJson(JsonWritingOptions.PrettyPrintRowsAsMap);
      Console.WriteLine(json);
      var jsonMap = json.JsonToDataObject() as JsonDataMap;

      var d2 = new CustomDoc1();
      JsonReader.ToDoc(d2, jsonMap);

      Aver.AreEqual(d1.ID, d2.ID);
      Aver.AreEqual(d1.Data.Text, d2.Data.Text);
      Aver.AreEqual(d1.Data.Length, d2.Data.Length);
    }

    [Run]
    public void CustomWritableReadable_2()
    {
      var d1 = new CustomDoc2 { ID = "meduza2", Data = new CustomStructType("Custom string 2") };

      var json = d1.ToJson(JsonWritingOptions.PrettyPrintRowsAsMap);
      Console.WriteLine(json);
      var jsonMap = json.JsonToDataObject() as JsonDataMap;

      var d2 = new CustomDoc2();
      JsonReader.ToDoc(d2, jsonMap);

      Aver.AreEqual(d1.ID, d2.ID);
      Aver.AreEqual(d1.Data.Value.Text, d2.Data.Value.Text);
      Aver.AreEqual(d1.Data.Value.Length, d2.Data.Value.Length);
    }

    [Run]
    public void CustomWritableReadable_3()
    {
      var d1 = new CustomDoc3 { ID = "meduza3", Data = new CustomStructType[]{ new CustomStructType("Custom string 3"), new CustomStructType("Gold for toad") } };

      var json = d1.ToJson(JsonWritingOptions.PrettyPrintRowsAsMap);
      Console.WriteLine(json);
      var jsonMap = json.JsonToDataObject() as JsonDataMap;

      var d2 = new CustomDoc3();
      JsonReader.ToDoc(d2, jsonMap);

      Aver.AreEqual(d1.ID, d2.ID);
      Aver.AreEqual(d1.Data[0].Text, d2.Data[0].Text);
      Aver.AreEqual(d1.Data[0].Length, d2.Data[0].Length);
    }

    [Run]
    public void CustomWritableReadable_4()
    {
      var d1 = new CustomDoc4 { ID = "meduza4", Data = new List<CustomStructType> { new CustomStructType("Custom string 4") } };

      var json = d1.ToJson(JsonWritingOptions.PrettyPrintRowsAsMap);
      Console.WriteLine(json);
      var jsonMap = json.JsonToDataObject() as JsonDataMap;

      var d2 = new CustomDoc4();
      JsonReader.ToDoc(d2, jsonMap);

      Aver.AreEqual(d1.ID, d2.ID);
      Aver.AreEqual(d1.Data[0].Text, d2.Data[0].Text);
      Aver.AreEqual(d1.Data[0].Length, d2.Data[0].Length);
    }



    public enum MetalType{ Gold, Silver, Platinum, Ferrum}

    public class WithVariousStructsDoc : TypedDoc
    {
      [Field] public GDID       Gdid{ get;set;}
      [Field] public GDIDSymbol GdidSymbol { get; set; }
      [Field] public Guid       Guid { get; set; }
      [Field] public Atom       Atom { get; set; }
      [Field] public TimeSpan   Timespan { get; set; }
      [Field] public DateTime   DateTime { get; set; }
      [Field] public NLSMap     Nls { get; set; }
      [Field] public DateRange  DateRange { get; set; }
      [Field] public Amount     Amount { get; set; }
      [Field] public FID        Fid { get; set; }
      [Field] public MetalType  Metal { get; set; }
    }

    public class WithVariousNullableStructsDoc : TypedDoc
    {
      [Field] public GDID?       Gdid        { get; set; }
      [Field] public GDIDSymbol? GdidSymbol  { get; set; }
      [Field] public Guid?       Guid        { get; set; }
      [Field] public Atom?       Atom        { get; set; }
      [Field] public TimeSpan?   Timespan    { get; set; }
      [Field] public DateTime?   DateTime    { get; set; }
      [Field] public NLSMap?     Nls         { get; set; }
      [Field] public DateRange?  DateRange   { get; set; }
      [Field] public Amount?     Amount      { get; set; }
      [Field] public FID?        Fid         { get; set; }
      [Field] public MetalType?  Metal       { get; set; }
      [Field] public StringMap   StringMap   { get; set; }

      [Field] public GDID[]   GdidArray   { get; set; }
      [Field] public string[] StringArray { get; set; }
      [Field] public char[]   CharArray   { get; set; }

      [Field] public WithVariousNullableStructsDoc Inner { get; set; }
      [Field] public WithVariousNullableStructsDoc[] InnerArray { get; set; }
      [Field] public List<WithVariousNullableStructsDoc> InnerList { get; set; }
    }


    [Run]
    public void Test_WithVariousStructsDoc_All()
    {
      var d1 = new WithVariousStructsDoc
      {
        Gdid = new GDID(1, 2, 3),
        GdidSymbol = new GDIDSymbol(new GDID(1, 2, 3), "abrkadabra"),
        Guid = Guid.NewGuid(),
        Atom = new Atom(),
        Timespan = TimeSpan.FromSeconds(123),
        DateTime = new DateTime(1980, 2, 3),
        Nls = new NLSMap("{eng: {n: 'a', d: 'b'}}"),
        DateRange = new DateRange(new DateTime(1980, 2, 2), new DateTime(1990, 3, 3)),
        Amount = new Amount("usd", 34.78m),
        Fid = new FID(1234),
        Metal = MetalType.Platinum
      };
      var json = d1.ToJson(JsonWritingOptions.PrettyPrintRowsAsMap);
      Console.WriteLine(json);
      var map = json.JsonToDataObject() as JsonDataMap;

      var d2 = new WithVariousStructsDoc();
      JsonReader.ToDoc(d2, map);

      Aver.AreEqual(d1.Gdid, d2.Gdid);
      Aver.AreEqual(d1.GdidSymbol, d2.GdidSymbol);
      Aver.AreEqual(d1.Guid, d2.Guid);
      Aver.AreEqual(d1.Atom, d2.Atom);
      Aver.AreEqual(d1.Timespan, d2.Timespan);
      Aver.AreEqual(d1.DateTime, d2.DateTime);
      Aver.AreEqual(d1.Nls["eng"].Name, d2.Nls["eng"].Name);
      Aver.AreEqual(d1.DateRange, d2.DateRange);
      Aver.AreEqual(d1.Amount, d2.Amount);
      Aver.AreEqual(d1.Fid, d2.Fid);
      Aver.IsTrue( d1.Metal == d2.Metal );
    }


    [Run]
    public void Test_WithVariousNullableStructsDoc_GDID()
    {
      var d1 = new WithVariousNullableStructsDoc{ Gdid = new GDID(1,2,3)   };
      var json = d1.ToJson(JsonWritingOptions.PrettyPrintRowsAsMap);
      Console.WriteLine(json);
      var map = json.JsonToDataObject() as JsonDataMap;

      var d2 = new WithVariousNullableStructsDoc();
      JsonReader.ToDoc(d2, map);

      Aver.AreEqual(d1.Gdid, d2.Gdid);
    }

    [Run]
    public void Test_WithVariousNullableStructsDoc_GDIDSymbol()
    {
      var d1 = new WithVariousNullableStructsDoc { GdidSymbol = new GDIDSymbol(new GDID(1, 2, 3), "abrkadabra") };
      var json = d1.ToJson(JsonWritingOptions.PrettyPrintRowsAsMap);
      Console.WriteLine(json);
      var map = json.JsonToDataObject() as JsonDataMap;

      var d2 = new WithVariousNullableStructsDoc();
      JsonReader.ToDoc(d2, map);

      Aver.AreEqual(d1.GdidSymbol, d2.GdidSymbol);
    }

    [Run]
    public void Test_WithVariousNullableStructsDoc_Guid()
    {
      var d1 = new WithVariousNullableStructsDoc { Guid = Guid.NewGuid() };
      var json = d1.ToJson(JsonWritingOptions.PrettyPrintRowsAsMap);
      Console.WriteLine(json);
      var map = json.JsonToDataObject() as JsonDataMap;

      var d2 = new WithVariousNullableStructsDoc();
      JsonReader.ToDoc(d2, map);

      Aver.AreEqual(d1.Guid, d2.Guid);
    }

    [Run]
    public void Test_WithVariousNullableStructsDoc_Atom()
    {
      var d1 = new WithVariousNullableStructsDoc { Atom = Atom.Encode("mus-ic_0") };
      var json = d1.ToJson(JsonWritingOptions.PrettyPrintRowsAsMap);
      Console.WriteLine(json);
      var map = json.JsonToDataObject() as JsonDataMap;

      var d2 = new WithVariousNullableStructsDoc();
      JsonReader.ToDoc(d2, map);

      Aver.AreEqual(d1.Atom, d2.Atom);
    }


    [Run]
    public void Test_WithVariousNullableStructsDoc_Timespan()
    {
      var d1 = new WithVariousNullableStructsDoc { Timespan = TimeSpan.FromSeconds(123) };
      var json = d1.ToJson(JsonWritingOptions.PrettyPrintRowsAsMap);
      Console.WriteLine(json);
      var map = json.JsonToDataObject() as JsonDataMap;

      var d2 = new WithVariousNullableStructsDoc();
      JsonReader.ToDoc(d2, map);

      Aver.AreEqual(d1.Timespan, d2.Timespan);
    }

    [Run]
    public void Test_WithVariousNullableStructsDoc_DateTime()
    {
      var d1 = new WithVariousNullableStructsDoc { DateTime = new DateTime(1980, 2, 3) };
      var json = d1.ToJson(JsonWritingOptions.PrettyPrintRowsAsMap);
      Console.WriteLine(json);
      var map = json.JsonToDataObject() as JsonDataMap;

      var d2 = new WithVariousNullableStructsDoc();
      JsonReader.ToDoc(d2, map);

      Aver.AreEqual(d1.DateTime, d2.DateTime);
    }

    [Run]
    public void Test_WithVariousNullableStructsDoc_Nls()
    {
      var d1 = new WithVariousNullableStructsDoc { Nls = new NLSMap("{eng: {n: 'a', d: 'b'}}") };
      var json = d1.ToJson(JsonWritingOptions.PrettyPrintRowsAsMap);
      Console.WriteLine(json);
      var map = json.JsonToDataObject() as JsonDataMap;

      var d2 = new WithVariousNullableStructsDoc();
      JsonReader.ToDoc(d2, map);

      Aver.AreEqual(d1.Nls.Value["eng"].Name, d2.Nls.Value["eng"].Name);
    }

    [Run]
    public void Test_WithVariousNullableStructsDoc_DateRange()
    {
      var d1 = new WithVariousNullableStructsDoc { DateRange = new DateRange(new DateTime(1980, 2,2), new DateTime(1990,3,3)) };
      var json = d1.ToJson(JsonWritingOptions.PrettyPrintRowsAsMap);
      Console.WriteLine(json);
      var map = json.JsonToDataObject() as JsonDataMap;

      var d2 = new WithVariousNullableStructsDoc();
      JsonReader.ToDoc(d2, map);

      Aver.AreEqual(d1.DateRange, d2.DateRange);
    }

    [Run]
    public void Test_WithVariousNullableStructsDoc_Amount()
    {
      var d1 = new WithVariousNullableStructsDoc { Amount = new Amount("usd", 34.78m) };
      var json = d1.ToJson(JsonWritingOptions.PrettyPrintRowsAsMap);
      Console.WriteLine(json);
      var map = json.JsonToDataObject() as JsonDataMap;

      var d2 = new WithVariousNullableStructsDoc();
      JsonReader.ToDoc(d2, map);

      Aver.AreEqual(d1.Amount, d2.Amount);
    }

    [Run]
    public void Test_WithVariousNullableStructsDoc_Fid()
    {
      var d1 = new WithVariousNullableStructsDoc { Fid = new FID(5678) };
      var json = d1.ToJson(JsonWritingOptions.PrettyPrintRowsAsMap);
      Console.WriteLine(json);
      var map = json.JsonToDataObject() as JsonDataMap;

      var d2 = new WithVariousNullableStructsDoc();
      JsonReader.ToDoc(d2, map);

      Aver.AreEqual(d1.Fid, d2.Fid);
    }

    [Run]
    public void Test_WithVariousNullableStructsDoc_Enum()
    {
      var d1 = new WithVariousNullableStructsDoc { Metal = MetalType.Ferrum };
      var json = d1.ToJson(JsonWritingOptions.PrettyPrintRowsAsMap);
      Console.WriteLine(json);
      var map = json.JsonToDataObject() as JsonDataMap;

      var d2 = new WithVariousNullableStructsDoc();
      JsonReader.ToDoc(d2, map);

      Aver.IsTrue( d1.Metal == d2.Metal );
    }


    [Run]
    public void Test_WithVariousNullableStructsDoc_StringMap()
    {
      var d1 = new WithVariousNullableStructsDoc { StringMap = new StringMap{ {"a","a value"} } };
      var json = d1.ToJson(JsonWritingOptions.PrettyPrintRowsAsMap);
      Console.WriteLine(json);
      var map = json.JsonToDataObject() as JsonDataMap;

      var d2 = new WithVariousNullableStructsDoc();
      JsonReader.ToDoc(d2, map);

      Aver.AreEqual(d1.StringMap["a"], d2.StringMap["a"]);
    }

    [Run]
    public void Test_WithVariousNullableStructsDoc_GdidArray()
    {
      var d1 = new WithVariousNullableStructsDoc { GdidArray = new GDID[] { new GDID(1,2,3), new GDID(2,3,4) } };
      var json = d1.ToJson(JsonWritingOptions.PrettyPrintRowsAsMap);
      Console.WriteLine(json);
      var map = json.JsonToDataObject() as JsonDataMap;

      var d2 = new WithVariousNullableStructsDoc();
      JsonReader.ToDoc(d2, map);

      Aver.AreArraysEquivalent(d1.GdidArray, d2.GdidArray);
    }

    [Run]
    public void Test_WithVariousNullableStructsDoc_StringArray()
    {
      var d1 = new WithVariousNullableStructsDoc { StringArray = new string[]{"German", null, "English"} };
      var json = d1.ToJson(JsonWritingOptions.PrettyPrintRowsAsMap);
      Console.WriteLine(json);
      var map = json.JsonToDataObject() as JsonDataMap;

      var d2 = new WithVariousNullableStructsDoc();
      JsonReader.ToDoc(d2, map);

      Aver.AreArraysEquivalent(d1.StringArray, d2.StringArray);
    }


    [Run]
    public void Test_WithVariousNullableStructsDoc_CharArray()
    {
      var d1 = new WithVariousNullableStructsDoc { CharArray = new char[]{'a', '2', ' '} };
      var json = d1.ToJson(JsonWritingOptions.PrettyPrintRowsAsMap);
      Console.WriteLine(json);
      var map = json.JsonToDataObject() as JsonDataMap;

      var d2 = new WithVariousNullableStructsDoc();
      JsonReader.ToDoc(d2, map);

      Aver.AreArraysEquivalent(d1.CharArray, d2.CharArray);
    }

    [Run]
    public void Test_WithVariousNullableStructsDoc_Inner()
    {
      var d1 = new WithVariousNullableStructsDoc
      {
        Inner = new WithVariousNullableStructsDoc
        {
          Inner = new WithVariousNullableStructsDoc
          {
            Gdid = new GDID(2,3,4)
          }
        }
      };
      var json = d1.ToJson(JsonWritingOptions.PrettyPrintRowsAsMap);
      Console.WriteLine(json);
      var map = json.JsonToDataObject() as JsonDataMap;

      var d2 = new WithVariousNullableStructsDoc();
      JsonReader.ToDoc(d2, map);

      Aver.IsNotNull(d2.Inner);
      Aver.IsNotNull(d2.Inner.Inner);
      Aver.AreEqual(new GDID(2,3,4), d2.Inner.Inner.Gdid);
    }

    [Run]
    public void Test_WithVariousNullableStructsDoc_InnerArray()
    {
      var d1 = new WithVariousNullableStructsDoc
      {
        InnerArray = new WithVariousNullableStructsDoc[]
        {
           new WithVariousNullableStructsDoc{ Atom = Atom.Encode("Baba"), Inner = new WithVariousNullableStructsDoc{ Atom = Atom.Encode("Yaga")}},
           null,
           null,
           new WithVariousNullableStructsDoc{ Atom = Atom.Encode("Kulibin")}
        }
      };
      var json = d1.ToJson(JsonWritingOptions.PrettyPrintRowsAsMap);
      Console.WriteLine(json);
      var map = json.JsonToDataObject() as JsonDataMap;

      var d2 = new WithVariousNullableStructsDoc();
      JsonReader.ToDoc(d2, map);

      Aver.IsNotNull(d2.InnerArray);
      Aver.AreEqual(4, d2.InnerArray.Length);
      Aver.AreEqual("Baba", d2.InnerArray[0].Atom.Value.Value);
      Aver.IsNotNull(d2.InnerArray[0].Inner);
      Aver.AreEqual("Yaga", d2.InnerArray[0].Inner.Atom.Value.Value);

      Aver.IsNull(d2.InnerArray[1]);
      Aver.IsNull(d2.InnerArray[2]);

      Aver.IsNotNull(d2.InnerArray[3]);
      Aver.AreEqual("Kulibin", d2.InnerArray[3].Atom.Value.Value);
    }

    [Run]
    public void Test_WithVariousNullableStructsDoc_InnerList()
    {
      var d1 = new WithVariousNullableStructsDoc
      {
        InnerList = new List<WithVariousNullableStructsDoc>
        {
           new WithVariousNullableStructsDoc{ Atom = Atom.Encode("Baba"), Inner = new WithVariousNullableStructsDoc{ Atom = Atom.Encode("Yaga")}},
           null,
           null,
           new WithVariousNullableStructsDoc{ Atom = Atom.Encode("Kulibin")}
        }
      };
      var json = d1.ToJson(JsonWritingOptions.PrettyPrintRowsAsMap);
      Console.WriteLine(json);
      var map = json.JsonToDataObject() as JsonDataMap;

      var d2 = new WithVariousNullableStructsDoc();
      JsonReader.ToDoc(d2, map);

      Aver.IsNotNull(d2.InnerList);
      Aver.AreEqual(4, d2.InnerList.Count);
      Aver.AreEqual("Baba", d2.InnerList[0].Atom.Value.Value);
      Aver.IsNotNull(d2.InnerList[0].Inner);
      Aver.AreEqual("Yaga", d2.InnerList[0].Inner.Atom.Value.Value);

      Aver.IsNull(d2.InnerList[1]);
      Aver.IsNull(d2.InnerList[2]);

      Aver.IsNotNull(d2.InnerList[3]);
      Aver.AreEqual("Kulibin", d2.InnerList[3].Atom.Value.Value);
    }


  }//class
}//namespace