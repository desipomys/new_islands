using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 未测试
/// </summary>
public static class XYHelper 
{
    /// <summary>
    /// 0是x（高位）,1是y（低位）
    /// </summary>
    /// <param name="xy"></param>
    /// <returns></returns>
    public static int[] GetIntXY(int xy)
    {
        int[] temp = new int[2];
        temp[0] = xy >>16;
        temp[1] = xy & 0x0000ffff;
        return temp;
    }
     public static int GetIntX(int xy)
    {
        int temp = xy /0x00010000;
        return temp;
    }
    public static int GetIntY(int xy)
    {
        int temp = xy % 0x00010000;
        return temp;
    }

    public static int ToIntXY(int x,int y)
    {
        int xy = (x << 16);
        xy = xy | (y & 0x0000FFFF);
        return xy;
    }
    /// <summary>
    /// 坐标支持负数
    /// </summary>
    /// <param name="x">-128~127</param>
    /// <param name="y">-128~127</param>
    /// <param name="h">-32768~32767</param>
    /// <returns></returns>
    public static int ToCoord3(int x, int y, int h)//已测试
    {
        int ans = ((x+128) & 0x000000ff) << 24 | ((y+128) & 0x000000ff) << 16 | ((h+32768) & 0x0000ffff);
        return ans;
    }
    /// <summary>
    /// 坐标支持负数
    /// </summary>
    /// <param name="x">-65536*128~65536*128</param>
    /// <param name="y">-65536*128~65536*128</param>
    /// <param name="h">-32768~32767</param>
    /// <returns></returns>
    public static long ToLongCoord3(int x, int y, int h)
    {
        long ans = (((long)(x + 65536 * 128) & 0x0000000000ffffff)) << 40 | ((long)((y + 65536 * 128) & 0x00ffffff)) << 16 | ((long)((h + 32768) & 0x0000ffff));
        return ans;
    }

    public static int[] GetLongXY(long xy)
    {
        int[] temp = new int[2];
        temp[0] = xy.GetX();
        temp[1]= xy.GetY();
        return temp;
    }
    /// <summary>
    /// 已测试
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public static long ToLongXY(int x, int y)
    {
        //long xy = (long)0xFFFFFFFFFFFFFFFF;
       long xy =  ((long)x << 32);
        xy=xy|((long)y&0x00000000FFFFFFFF);
        return xy;
    }
    public static long AddXLong(long b,int x)
    {
        int ax = b.GetX();
        int ay = b.GetY();
        ax += x;
        return ToLongXY(ax,ay);
    }
    public static long AddYLong(long b,int y)
    {
        int ax = b.GetX();
        int ay = b.GetY();
        ay += y;
        return ToLongXY(ax, ay);
    }
    
    public static int GetXLong(long xy)
    {
        return (int)(xy/0x0000000100000000);
    }
    public static int GetYLong(long xy)
    {
        return (int)(xy%0x0000000100000000);
    }
}
