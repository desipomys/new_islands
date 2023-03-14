using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Compression;
using System.Data;

public class StringCompresser 
{
    /// <summary>
    /// ��ѹ
    /// </summary>
    /// <param name="Value"></param>
    /// <returns></returns>
    public static DataSet GetDatasetByString(string Value)
    {
        DataSet ds = new DataSet();
        string CC = GZipDecompressString(Value);
        System.IO.StringReader Sr = new StringReader(CC);
        ds.ReadXml(Sr);
        return ds;
    }
    /// <summary>
    /// ����DATASETѹ���ַ���
    /// </summary>
    /// <param name="ds"></param>
    /// <returns></returns>
    public static string GetStringByDataset(string ds)
    {
        return GZipCompressString(ds);
    }
    /// <summary>
    /// �������ַ�����GZip�㷨ѹ���󣬷���Base64�����ַ�
    /// </summary>
    /// <param name="rawString">��Ҫѹ�����ַ���</param>
    /// <returns>ѹ�����Base64������ַ���</returns>
    public static string GZipCompressString(string rawString)
    {
        if (string.IsNullOrEmpty(rawString) || rawString.Length == 0)
        {
            return "";
        }
        else
        {
            byte[] rawData = System.Text.Encoding.UTF8.GetBytes(rawString.ToString());
            byte[] zippedData = Compress(rawData);
            return (string)(Convert.ToBase64String(zippedData));
        }

    }
    /// <summary>
    /// GZipѹ��
    /// </summary>
    /// <param name="rawData"></param>
    /// <returns></returns>
    public static byte[] Compress(byte[] rawData)
    {
        MemoryStream ms = new MemoryStream();
        GZipStream compressedzipStream = new GZipStream(ms, CompressionMode.Compress, true);
        compressedzipStream.Write(rawData, 0, rawData.Length);
        compressedzipStream.Close();
        return ms.ToArray();
    }
    /// <summary>
    /// ������Ķ������ַ���������GZip�㷨��ѹ��
    /// </summary>
    /// <param name="zippedString">��GZipѹ����Ķ������ַ���</param>
    /// <returns>ԭʼδѹ���ַ���</returns>
    public static string GZipDecompressString(string zippedString)
    {
        if (string.IsNullOrEmpty(zippedString) || zippedString.Length == 0)
        {
            return "";
        }
        else
        {
            byte[] zippedData = Convert.FromBase64String(zippedString.ToString());
            return (string)(System.Text.Encoding.UTF8.GetString(Decompress(zippedData)));
        }
    }
    /// <summary>
    /// ZIP��ѹ
    /// </summary>
    /// <param name="zippedData"></param>
    /// <returns></returns>
    public static byte[] Decompress(byte[] zippedData)
    {
        MemoryStream ms = new MemoryStream(zippedData);
        GZipStream compressedzipStream = new GZipStream(ms, CompressionMode.Decompress);
        MemoryStream outBuffer = new MemoryStream();
        byte[] block = new byte[1024];
        while (true)
        {
            int bytesRead = compressedzipStream.Read(block, 0, block.Length);
            if (bytesRead <= 0)
                break;
            else
                outBuffer.Write(block, 0, bytesRead);
        }
        compressedzipStream.Close();
        return outBuffer.ToArray();
    }
}

