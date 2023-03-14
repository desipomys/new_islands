using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using System;
using System.Threading;

public class FileSaver
{
    /// <summary>
    /// 只清空文件夹内容，不删除文件夹本身
    /// </summary>
    /// <param name="srcPath"></param>
    static void deleteDirContent(string srcPath)
    {
        //DirectoryInfo di = new DirectoryInfo(srcPath);
        //di.Delete(true);
        try
        {
            DirectoryInfo dir = new DirectoryInfo(srcPath);
            FileSystemInfo[] fileinfo = dir.GetFileSystemInfos();  //返回目录中所有文件和子目录
            foreach (FileSystemInfo i in fileinfo)
            {
                if (i is DirectoryInfo)            //判断是否文件夹
                {
                    DirectoryInfo subdir = new DirectoryInfo(i.FullName);
                    subdir.Delete(true);          //删除子目录和文件
                }
                else
                {
                    File.Delete(i.FullName);      //删除指定文件
                }
            }
        }
        catch (Exception e)
        {
            throw;
        }
    }

    #region 游戏性
    public static void DeleteSave(string mapPath)
    {
        string path = Path.Combine(System.Environment.CurrentDirectory, "Assets/Resources/Saves", mapPath);
        //deleteDir(path);
        Directory.Delete(path,true);
    }
    public static void CopyGenedMapToMap(string savePath,string mapName)
    {
       string genedMapName= Path.Combine(System.Environment.CurrentDirectory, "Assets/Resources/Saves", savePath,"genedMap/",mapName);
        string targetPath=Path.Combine(System.Environment.CurrentDirectory, "Assets/Resources/Saves", savePath,"map/");
        deleteDirContent(targetPath);
        CopyDirectory(genedMapName,targetPath);
    }
    public static string[] GetAllSavePropertyPath(string mainpath)
    {
        List<string> savepaths = new List<string>();
        DirectoryInfo dif = new DirectoryInfo(mainpath);
        DirectoryInfo[] dics = dif.GetDirectories();
        foreach (DirectoryInfo item in dics)
        {
            if (item.GetFiles("proper.txt").Length > 0 && item.GetFiles("data.txt").Length > 0)
            {
                savepaths.Add(item.FullName);
            }
        }
        return savepaths.ToArray();
    }
    public static bool IsMapLoaded(string mapPath)
    {
        Debug.Log(mapPath + "已加载？");
        if (!CheckMapFolderAvalible(mapPath)) return false;
        string path = Path.Combine(System.Environment.CurrentDirectory, "Assets/Resources/Saves", mapPath, "map");
        Debug.Log(Path.Combine(path, "chunk_0_0.txt"));
        if (File.Exists(Path.Combine(path, "chunk_0_0.txt"))&& File.Exists(Path.Combine(path, "entityblock_0_0.txt")))
        {

            return true;
        }
        return false;
    }
    public static bool CheckMapFolderAvalible(string mapPath)
    {
        string path = Path.Combine(System.Environment.CurrentDirectory, "Assets/Resources/Saves", mapPath, "map");
        if (!Directory.Exists(path))//是否有地图路径
        {
            return false;
        }
        //if (!File.Exists(Path.Combine(path, "entityblock.txt"))) return false;
        if (!File.Exists(Path.Combine(path, "players.txt"))) return false;//地图目录内是否有chunk,player,entity
        if (!File.Exists(Path.Combine(path, "entitys.txt"))) return false;
        if (!File.Exists(Path.Combine(path, "proper.txt"))) return false;
        return true;
    }
    public static string GetCurrentMapPrefabsData(string mapPath)
    {
        return GetFile(Path.Combine(System.Environment.CurrentDirectory, "Assets/Resources/Saves/", mapPath, "map/" + "proper.txt"));
    }
    public static bool SetCurrentMapPrefabsData(string mapPath,string data)
    {
       return SaveFile(Path.Combine(System.Environment.CurrentDirectory, "Assets/Resources/Saves/", mapPath, "map/" + "proper.txt"), data);
    }
    public static string GetCurrentMapEngineData(string savePath)
    {
        return GetFile(Path.Combine(System.Environment.CurrentDirectory, "Assets/Resources/Saves/", savePath, "map/engine.txt"));
    }
    public static void SetCurrentMapEngineData(string savePath,string data)
    {
        SaveFile(Path.Combine(System.Environment.CurrentDirectory, "Assets/Resources/Saves/", savePath, "map/engine.txt"), data);
    }
     public static string GetSaveSetting(string savePath)
    {
        return GetFile(Path.Combine(System.Environment.CurrentDirectory, "Assets/Resources/Saves/", savePath, "setting.txt"));
    }
    public static void SetSaveSetting(string savePath,string data)
    {
        SaveFile(Path.Combine(System.Environment.CurrentDirectory, "Assets/Resources/Saves/", savePath, "setting.txt"), data);
    }

    public static string GetAllMapPrefabsData(string savePath)
    {
        return GetFile(Path.Combine(System.Environment.CurrentDirectory, "Assets/Resources/Saves/", savePath, "mapnodes.txt"));
    }
    public static string GetAllMapGraphData(string savePath)
    {
        return GetFile(Path.Combine(System.Environment.CurrentDirectory, "Assets/Resources/Saves/", savePath, "mapgraph.txt"));
    }
    public static void SetAllMapPrefabsData(string savePath,string data)
    {
        SaveFile(Path.Combine(System.Environment.CurrentDirectory, "Assets/Resources/Saves/", savePath, "mapnodes.txt"),data);
    }
    public static void SetAllMapGraphData(string savePath,string data)
    {
         SaveFile(Path.Combine(System.Environment.CurrentDirectory, "Assets/Resources/Saves/", savePath, "mapgraph.txt"), data);
    }
    public static string GetGenedMapEngine(string savePath,string MapPath)
    {
        return GetFile(Path.Combine(System.Environment.CurrentDirectory, "Assets/Resources/Saves/", savePath,"genedMap/",MapPath, "engine.txt"));
    }
    public static void SetGenedMapEngine(string savePath,string MapPath, string data)
    {
        SaveFile(Path.Combine(System.Environment.CurrentDirectory, "Assets/Resources/Saves/", savePath, "genedMap/", MapPath, "engine.txt"), data);
    }

    public static bool CheckSaveEngineLoaded(string saveName)
    {
        return !string.IsNullOrEmpty(GetFileInSave(saveName, "engine"));
    }
    public static string GetSaveEngineData(string saveName)
    {
        return GetFileInSave(saveName, "engine");
    }
    public static bool SetSaveEngineData(string saveName,string data)
    {
        return SaveFileInSave(saveName, "engine",data);
    }

    #region Entity
    public static string GetEntityFile(string mapPath, int x, int y)
    {
        return GetFile(Path.Combine(System.Environment.CurrentDirectory, "Assets/Resources/Saves/", mapPath, "map/" + "entity_" + x.ToString() + "_" + y.ToString() + ".txt"));
    }
    public static bool HasEntityFile(string mapPath, int x, int y)
    {
        return HasFile(Path.Combine(System.Environment.CurrentDirectory, "Assets/Resources/Saves/", mapPath, "map/" + "entity_"
        + x.ToString() + "_" + y.ToString() + ".txt"));
    }
    public static void SetEntityFile(string mapPath, int x, int y, string data)
    {
        SaveFile(Path.Combine(System.Environment.CurrentDirectory, "Assets/Resources/Saves/", mapPath, "map/" + "entity_"
        + x.ToString() + "_" + y.ToString() + ".txt"), data);
    }
    #endregion

    #region EBlockFile
    public static string GetEBlockFile(string mapPath,int x,int y)
    {
        return GetFile(Path.Combine(System.Environment.CurrentDirectory, "Assets/Resources/Saves/", mapPath, "map/" + "entityblock_" + x.ToString() + "_" + y.ToString() + ".txt"));
    }
    public static bool HasEBlockFile(string mapPath, int x, int y)
    {
        return HasFile(Path.Combine(System.Environment.CurrentDirectory, "Assets/Resources/Saves/", mapPath, "map/" + "entityblock_"
        + x.ToString() + "_" + y.ToString() + ".txt"));
    }
    public static void SetEBlockFile(string mapPath,int x,int y, string data)
    {
        SaveFile(Path.Combine(System.Environment.CurrentDirectory, "Assets/Resources/Saves/", mapPath, "map/" + "entityblock_"
        + x.ToString() + "_" + y.ToString() + ".txt"), data);
    }
    #endregion


    #region terrainFile
    public static string GetTerrainFile(string mapPath, int x, int y)
    {
        return GetFile(Path.Combine(System.Environment.CurrentDirectory, "Assets/Resources/Saves/", mapPath, "map/" + "chunk_"
        + x.ToString() + "_" + y.ToString() + ".txt"));
    }
    public static bool HasTerrainFile(string mapPath, int x, int y)
    {
        return HasFile(Path.Combine(System.Environment.CurrentDirectory, "Assets/Resources/Saves/", mapPath, "map/" + "chunk_"
        + x.ToString() + "_" + y.ToString() + ".txt"));
    }
    public static void SetTerrainFile(string mapPath, int x, int y, string data)
    {
        SaveFile(Path.Combine(System.Environment.CurrentDirectory, "Assets/Resources/Saves/", mapPath, "map/" + "chunk_"
        + x.ToString() + "_" + y.ToString() + ".txt"), data);
    }

    #endregion

    public static string GetMapPlayerCharDatas(string mapPath)
    {
        return GetFile(Path.Combine(System.Environment.CurrentDirectory, "Assets/Resources/Saves/", mapPath, "map/"
        + "players.txt"));
    }
    public static void SetMapPlayerCharDatas(string mapPath, string data)
    {
        SaveFile(Path.Combine(System.Environment.CurrentDirectory, "Assets/Resources/Saves/", mapPath, "map/" + "players"
        + ".txt"), data);
    }
    /// <summary>
    /// 获取存档文件夹下fileName为名的文件
    /// </summary>
    /// <param name="path"></param>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public static string GetFileInSave(string path, string fileName)
    {
        return GetFileUtility(Path.Combine(System.Environment.CurrentDirectory, "Assets/Resources/Saves/", path, fileName + ".txt"));
    }
    public static bool SaveFileInSave(string path, string fileName,string data)
    {
        return SaveFileUtility(Path.Combine(System.Environment.CurrentDirectory, "Assets/Resources/Saves/", path, fileName + ".txt"),data);
    }

    public static void SavePlayerData(string path, string data)
    {
        Debug.Log("保存playerdata" + Path.Combine(System.Environment.CurrentDirectory, "Assets/Resources/Saves/", path, "data.txt"));
        SaveFileUtility(Path.Combine(System.Environment.CurrentDirectory, "Assets/Resources/Saves/", path, "data.txt"), data);
    }
    public static void DeleteMapFolder(string mappath)
    {
        string fullpath = Path.Combine(System.Environment.CurrentDirectory, "Assets/Resources/Saves/", mappath, "map");
        Delete(fullpath);
       
    }
    /// <summary>
    /// 只生成文件不写入数据
    /// </summary>
    /// <param name="mappath"></param>
    public static void CreateMapFolder(string mappath)
    {
        string fullpath = Path.Combine(System.Environment.CurrentDirectory, "Assets/Resources/Saves/", mappath, "map/proper.txt");
        SaveFile(fullpath, "");
        fullpath = Path.Combine(System.Environment.CurrentDirectory, "Assets/Resources/Saves/", mappath, "map/entitys.txt");
        SaveFile(fullpath, "");//等containerentity完成后要把生成entity.txt的逻辑转到containerentity
        //fullpath = Path.Combine(System.Environment.CurrentDirectory, "Assets/Resources/Saves/", mappath, "map/players.txt");
        //SaveFile(fullpath, "");
    }
    public static void SaveMapPrefabsData(string path, string data)
    {
        Debug.Log("保存mapprefab");
        SaveFile(Path.Combine(System.Environment.CurrentDirectory, "Assets/Resources/Saves/", path, "map/proper.txt"), data);
    }

    public static void SaveMovEngineData(string path, string data)
    {
        Debug.Log("保存moveng");
        SaveFileUtility(Path.Combine(System.Environment.CurrentDirectory, "Assets/Resources/Saves/", path, "engine.txt"), data);
    }
    
    public static void SaveSaveData(string path, string data)
    {
        Debug.Log("保存save");
        SaveFileUtility(Path.Combine(System.Environment.CurrentDirectory, "Assets/Resources/Saves/", path, "proper.txt"), data);
    }
    #endregion



    public static string GetFileWithDecrypt(string path)
    {
        return null;
    }
    public static string GetFileWithFullPath(string path)//临时方案
    {
        string fpath = path;
        //Debug.Log(fpath);
        try
        {
            using (StreamReader sr = new StreamReader(fpath))
            {
                return sr.ReadToEnd();
            }
        }
        catch
        {
            Debug.Log("找不到文件" + path);
            return "";
        }

    }
    public static bool HasFile(string path)
    {
        return File.Exists(path);
    }
    public static string GetFile(string path)//临时方案
    {
        return GetFileUtility(path);
        //Debug.Log("压缩读取" + path);
        //return GetFileWithCompress(path);
    }
    public static string[] GetAllFilesInDirectory(string path, string fname)
    {
        string[] fs = Directory.GetFiles(path);
        List<string> paths = new List<string>();
        for (int i = 0; i < fs.Length; i++)
        {
            if (Path.GetFileName(fs[i]) == path)
            {
                paths.Add(fs[i]);
            }
        }

        return paths.ToArray();
    }

    public static bool SaveFile(string path, string data)//临时方案
    {
        return SaveFileUtility(path, data);
        //Debug.Log("压缩保存"+path);
        //return SaveFileWithCompress(path, data);
    }
    public static void SaveFileWithEncrypt(string data, string path)
    {

    }
    [Obsolete]
    public static void Save(string data, string saveName, string path)
    {

        FileStream fs = new FileStream(Application.dataPath + "//" + path, FileMode.OpenOrCreate);
        fs.Write(Encoding.UTF8.GetBytes(AesDecryptor_Base64(data, saveName)), 0, 0);
        //应异步存取存档文件
    }
    public static void Delete(string path)
    {
        DirectoryInfo di = new DirectoryInfo(path);
        di.Delete(true);
    }
    
    #region 通用
    public static void CopyDirectory(string srcPath, string destPath)
    {
        try
        {
            DirectoryInfo dir = new DirectoryInfo(srcPath);
            FileSystemInfo[] fileinfo = dir.GetFileSystemInfos();  //获取目录下（不包含子目录）的文件和子目录
            foreach (FileSystemInfo i in fileinfo)
        {
                if (i is DirectoryInfo)     //判断是否文件夹
                {
                    if (!Directory.Exists(destPath+"\\"+i.Name))
                    {
                        Directory.CreateDirectory(destPath + "\\" + i.Name);   //目标目录下不存在此文件夹即创建子文件夹
                    }
                    CopyDirectory(i.FullName, destPath + "\\" + i.Name);    //递归调用复制子文件夹
                }
                else
                {
                    File.Copy(i.FullName, destPath + "\\" + i.Name,true);      //不是文件夹即复制文件，true表示可以覆盖同名文件
                }
        }
        }
        catch (Exception e)
        {
            throw;
        }
    }
    public static bool SaveFileUtility(string path, string data)//临时方案
    {
        string fpath = System.IO.Path.Combine(System.Environment.CurrentDirectory, "Assets/Resources/", path);

        if (!Directory.Exists(Path.GetDirectoryName(fpath)))
        {
            Debug.Log(Path.GetDirectoryName(fpath));
            Directory.CreateDirectory(Path.GetDirectoryName(fpath));
        }

        try
        {
            using (StreamWriter sr = new StreamWriter(fpath))
            {
                sr.Write(data);
                return true;
            }
        }
        catch
        {
            FileStream fs = File.Create(fpath);  //创建文件
            fs.Close();

            using (StreamWriter sr = new StreamWriter(fpath))
            {
                sr.Write(data);
                return true;
            }
            return false;
        }

    }
    public static string GetFileUtility(string path)//临时方案
    {
        string fpath = System.IO.Path.Combine(System.Environment.CurrentDirectory, "Assets/Resources/", path);
        //Debug.Log(fpath);
        try
        {
            //Thread oThread = new Thread(new ThreadStart(Test2));
            using (StreamReader sr = new StreamReader(fpath))
            {
                return sr.ReadToEnd();
            }
        }
        catch
        {
            FileInfo fi = new FileInfo(fpath);
            if (fi.Directory.Exists)
            {
                FileStream fs = File.Create(fpath);  //创建文件
                fs.Close();

                using (StreamReader sr = new StreamReader(fpath))
                {
                    return sr.ReadToEnd();
                }
            }
            else
            {
                fi.Directory.Create();
                FileStream fs = File.Create(fpath);  //创建文件
                fs.Close();

                using (StreamReader sr = new StreamReader(fpath))
                {
                    return sr.ReadToEnd();
                }
            }
        }
        return "";
    }
    #endregion

    #region 加解密
    /// <summary>
    /// AES 算法加密(ECB模式) 将明文加密，加密后进行base64编码，返回密文
    /// </summary>
    /// <param name="EncryptStr">明文</param>
    /// <param name="Key">密钥</param>
    /// <returns>加密后base64编码的密文</returns>
    public static string AesEncryptor_Base64(string EncryptStr, string Key)
    {
        try
        {
            //byte[] keyArray = Encoding.UTF8.GetBytes(Key);
            SHA256CryptoServiceProvider sha256 = new SHA256CryptoServiceProvider();
            byte[] keyArray = sha256.ComputeHash(Encoding.UTF8.GetBytes(Key));
            //byte[] keyArray = Convert.FromBase64String(Key);
            byte[] toEncryptArray = Encoding.UTF8.GetBytes(EncryptStr);

            RijndaelManaged rDel = new RijndaelManaged();
            rDel.Key = keyArray;
            rDel.Mode = CipherMode.ECB;
            rDel.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = rDel.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }
        catch (Exception ex)
        {
            return null;
        }
    }

    /// <summary>
    /// AES 算法解密(ECB模式) 将密文base64解码进行解密，返回明文
    /// </summary>
    /// <param name="DecryptStr">密文</param>
    /// <param name="Key">密钥</param>
    /// <returns>明文</returns>
    public static string AesDecryptor_Base64(string DecryptStr, string Key)
    {
        try
        {
            //byte[] keyArray = Encoding.UTF8.GetBytes(Key);
            SHA256CryptoServiceProvider sha256 = new SHA256CryptoServiceProvider();
            byte[] keyArray = sha256.ComputeHash(Encoding.UTF8.GetBytes(Key));
            byte[] toEncryptArray = Convert.FromBase64String(DecryptStr);

            RijndaelManaged rDel = new RijndaelManaged();
            rDel.Key = keyArray;
            rDel.Mode = CipherMode.ECB;
            rDel.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = rDel.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            return Encoding.UTF8.GetString(resultArray);//  UTF8Encoding.UTF8.GetString(resultArray);
        }
        catch (Exception ex)
        {
            return null;
        }
    }
    #endregion

    #region 压缩
    public static bool SaveFileWithCompress(string path, string data)
    {
        return SaveFileUtility(path, StringCompresser.GZipCompressString(data));
    }
    public static string GetFileWithCompress(string path)
    {
        return StringCompresser.GZipDecompressString(GetFileUtility(path));
    }

    #endregion

    static string ThreadGetFile(string fpath)
    {
        try
        {
            //Thread oThread = new Thread(new ThreadStart(Test2));
            using (StreamReader sr = new StreamReader(fpath))
            {
                return sr.ReadToEnd();
            }
        }
        catch
        {
            FileStream fs = File.Create(fpath);  //创建文件
            fs.Close();

            using (StreamReader sr = new StreamReader(fpath))
            {
                return sr.ReadToEnd();
            }
        }
    }
}
