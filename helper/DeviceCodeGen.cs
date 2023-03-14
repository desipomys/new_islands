using System.Management; 
//using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class DeviceCodeGen//生成设备码
{
    public static long Gen()
    {
            try
            {
                //获取CPU序列号代码
                /*long cpuInfo = 0;//cpu序列号
                ManagementClass mc = new ManagementClass("Win32_Processor");
                ManagementObjectCollection moc = mc.GetInstances();
                foreach(ManagementObject mo in moc)
                {
                    cpuInfo = mo.Properties["ProcessorId"].Value;
                }
                moc=null;
                mc=null;*/
                return (long)Random.Range(0,1000000);
            }
            catch
            {
                return 0;
            }
            finally
            {
            }
    }
}