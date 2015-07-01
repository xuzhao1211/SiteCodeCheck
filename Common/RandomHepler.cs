//-------------------------------------------------------------------
//版权所有：版权所有(C) 2015 xuxiaozhao.sinaapp.com
//系统名称：网站检测系统
using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;

namespace LzSoft.SysService.Common
{
    public class RandomHepler
    {
        static string tmpstr = string.Empty;
        /// <summary>
        /// 系统生成18位SQLID
        /// </summary>
        /// <returns></returns>
        public static string GetSystemID()
        {
            string strFileName = "";

            strFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            while (tmpstr == strFileName)
                strFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            tmpstr = strFileName;

            strFileName += GetRandomStr(1);

            return strFileName;
        }

        #region  获得随机字符 GetRandomStr
        /// <summary>
        /// 获取一个指定长度的随机字符串
        /// </summary>
        /// <param name="length">长度</param>
        /// <returns>返回随机字符串</returns>
        public static string GetRandomStr(int length)
        {
            return GetRandomStr(new Random(), length);
        }

        /// <summary>
        /// 产生一个随机指定长度的字符串
        /// </summary>
        /// <param name="dictionary">要产生随机的字符串</param>
        /// <param name="length"></param>
        /// <returns>返回随机字符串</returns>
        public static string GetRandomStr(string dictionary, int length)
        {
            return GetRandomStr(new Random(), dictionary, length);
        }

        /// <summary>
        /// 产生一个0-9组成的指定长度的随机字符串
        /// </summary>
        /// <param name="r">随机函数对象</param>
        /// <param name="length">长度</param>
        /// <returns>返回随机字符串</returns>
        public static string GetRandomStr(Random r, int length)
        {
            return GetRandomStr(r, "0123456789", length);
        }

        /// <summary>
        /// 产生随机字符串函数
        /// </summary>
        /// <param name="r">随机函数对象</param>
        /// <param name="dictionary">要产生随机的字符串</param>
        /// <param name="length">长度</param>
        /// <returns>返回随机字符串</returns>
        public static string GetRandomStr(Random r, string dictionary, int length)
        {
            if (r == null)
            {
                r = new Random();
            }

            StringBuilder sb = new StringBuilder();
            char[] chars = dictionary.ToCharArray();

            for (int i = 0; i < length; i++)
            {
                sb.Append(chars[r.Next(0, chars.Length)]);
            }

            return sb.ToString();
        }
        #endregion

        #region 获取一组无重复的随机数
        public static int[] GetRandom(int Num, int MaxNum)
        {
            Random rd = new Random();
            ArrayList intTempArr = new ArrayList();
            
            if (MaxNum < Num)
                Num = MaxNum;
            int[] intArr = new int[Num];
            //填充数组intTempArr
            for (int i = 0; i < MaxNum; i++)
            {
                intTempArr.Add(i);
            }

            //生成随机数
            for (int j = 0; j < intArr.Length; j++)
            {
                int temp = rd.Next(intTempArr.Count - 1);
                int tempValue = (int)intTempArr[temp];
                intArr[j] = tempValue;
                intTempArr.RemoveAt(temp);
            }
            return intArr;
        }
        #endregion
    }
}
