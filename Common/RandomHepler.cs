//-------------------------------------------------------------------
//��Ȩ���У���Ȩ����(C) 2015 xuxiaozhao.sinaapp.com
//ϵͳ���ƣ���վ���ϵͳ
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
        /// ϵͳ����18λSQLID
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

        #region  �������ַ� GetRandomStr
        /// <summary>
        /// ��ȡһ��ָ�����ȵ�����ַ���
        /// </summary>
        /// <param name="length">����</param>
        /// <returns>��������ַ���</returns>
        public static string GetRandomStr(int length)
        {
            return GetRandomStr(new Random(), length);
        }

        /// <summary>
        /// ����һ�����ָ�����ȵ��ַ���
        /// </summary>
        /// <param name="dictionary">Ҫ����������ַ���</param>
        /// <param name="length"></param>
        /// <returns>��������ַ���</returns>
        public static string GetRandomStr(string dictionary, int length)
        {
            return GetRandomStr(new Random(), dictionary, length);
        }

        /// <summary>
        /// ����һ��0-9��ɵ�ָ�����ȵ�����ַ���
        /// </summary>
        /// <param name="r">�����������</param>
        /// <param name="length">����</param>
        /// <returns>��������ַ���</returns>
        public static string GetRandomStr(Random r, int length)
        {
            return GetRandomStr(r, "0123456789", length);
        }

        /// <summary>
        /// ��������ַ�������
        /// </summary>
        /// <param name="r">�����������</param>
        /// <param name="dictionary">Ҫ����������ַ���</param>
        /// <param name="length">����</param>
        /// <returns>��������ַ���</returns>
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

        #region ��ȡһ�����ظ��������
        public static int[] GetRandom(int Num, int MaxNum)
        {
            Random rd = new Random();
            ArrayList intTempArr = new ArrayList();
            
            if (MaxNum < Num)
                Num = MaxNum;
            int[] intArr = new int[Num];
            //�������intTempArr
            for (int i = 0; i < MaxNum; i++)
            {
                intTempArr.Add(i);
            }

            //���������
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
