using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace LzSoft.SysService.Common
{
    public class Provider
    {
        public static string CONNECTIONSTRING = System.Configuration.ConfigurationManager.AppSettings["DATABASE"];

        #region ʧЧ����
        /**
        public static DataTable GetTaskData()
        {
            DataTable tbl = new DataTable();
            using (SqlConnection conn = new SqlConnection(CONNECTIONSTRING))
            {
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT [C_ID],[C_Name],[C_TimeType],[C_RunTime],[C_IsRun] FROM [T_TaskPlan]";
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(tbl);
            }
            return tbl;
        }
        public static List<Task> GetTaskList()
        {
            List<Task> list = new List<Task>();
            DataTable tbl = GetTaskData();
            for (int i = 0; i < tbl.Rows.Count; i++)
            {
                Task t = new Task();
                t.Id = int.Parse(tbl.Rows[i][0].ToString());
                t.TaskName = Convert.ToString(tbl.Rows[i][1].ToString());
                t.TimeType = Convert.ToString(tbl.Rows[i][2].ToString());
                t.RunTime = Convert.ToString(tbl.Rows[i][3].ToString());
                t.Status = Convert.ToString(tbl.Rows[i][4].ToString());

                list.Add(t);
            }
            return list;
        }
        **/
        #endregion

        #region ���¹��λ��Ϣ
        /// <summary>
        /// ��ȡ���λ��Ϣ
        /// </summary>
        /// <param name="isexe"></param>
        /// <returns></returns>
        public static DataTable GetPositionById(string positionid)
        {
            DataTable tbl = new DataTable();
            using (SqlConnection conn = new SqlConnection(CONNECTIONSTRING))
            {
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT [C_SitePositionID],[C_SitesID],[C_Name],[C_CodeUrl],[C_IndustryID],[C_CreateDate],[C_Color],[C_AdType],[C_Keyword],[C_Filter],[C_StyleID],[C_CodeType],[C_Width],[C_Height],[C_TitleLen],[C_TotalRows],[C_PostionState] FROM [T_SitePosition] WHERE [C_SitePositionID]='" + positionid + "'";
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(tbl);

                
            }
            return tbl;
        }
        /// <summary>
        /// ��ȡ���λ״̬��Ϣ
        /// </summary>
        /// <param name="isexe"></param>
        /// <returns></returns>
        public static DataTable GetOnePositionBy(bool isexe)
        {
            DataTable tbl = new DataTable();
            using (SqlConnection conn = new SqlConnection(CONNECTIONSTRING))
            {
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT TOP 1 [C_SitePositionID],[C_LastCheckTime],[C_Status],[C_IsExe] FROM [T_SitePositionStatus] WHERE [C_IsExe] = " + (isexe ? "1" : "0");
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(tbl);
            }
            return tbl;
        }
        /// <summary>
        /// д��״̬���¼���Ĺ��λ
        /// </summary>
        /// <returns></returns>
        public static void InsertNewPosition()
        {
            using (SqlConnection conn = new SqlConnection(CONNECTIONSTRING))
            {
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = "INSERT INTO [T_SitePositionStatus] " +
                "SELECT [C_SitePositionID],GETDATE(),0,0 FROM [T_SitePosition]" +
                "WHERE [C_SitePositionID] NOT IN" +
                "(SELECT [C_SitePositionID] FROM [T_SitePositionStatus])";
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
        /// <summary>
        /// ��ʼ��״ֵ̬
        /// </summary>
        public static void UpdatePositionsStatus( bool isExe)
        {
            using (SqlConnection conn = new SqlConnection(CONNECTIONSTRING))
            {
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = "UPDATE [T_SitePositionStatus] SET [C_IsExe] = " + (isExe ? "1" : "0");
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
        /// <summary>
        /// ����ִ��״̬
        /// </summary>
        public static void UpdatePositionIsExe(string positionid, bool isExe)
        {
            using (SqlConnection conn = new SqlConnection(CONNECTIONSTRING))
            {
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = "UPDATE [T_SitePositionStatus] SET [C_IsExe] = " + (isExe?"1":"0") + " WHERE [C_SitePositionID] = '" + positionid +"'";
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
        /// <summary>
        /// ������վ״̬
        /// </summary>
        public static void UpdatePositionStatus(string positionid, string status)
        {
            using (SqlConnection conn = new SqlConnection(CONNECTIONSTRING))
            {
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = "UPDATE [T_SitePositionStatus] SET [C_Status] = " + status + " WHERE [C_SitePositionID] = '" + positionid + "'";
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
        /// <summary>
        /// ������һ�μ��ʱ��
        /// </summary>
        /// <param name="positionid"></param>
        /// <param name="lastTime"></param>
        public static void UpdatePositionCheckTime(string positionid, DateTime lastTime)
        {
            using (SqlConnection conn = new SqlConnection(CONNECTIONSTRING))
            {
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = "UPDATE [T_SitePositionStatus] SET [C_LastCheckTime] = '" + lastTime.ToString() + "' WHERE [C_SitePositionID] = '" + positionid + "'";
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
        #endregion

        #region ȡ�ù��λ����
        public static DataTable GetPositionList(int pageSize, int pageIndex, out int reCount)
        {
            return GetDataTableByPager("T_SitePosition", "C_SitePositionID", "*", "WHERE [C_Commend] = '1'", "", pageIndex, pageSize, out reCount);
        }
        public static DataTable GetDataTableByPager(string TableName, string PrimaryKey, string SelectFiled, string SqlWhere, string SqlOrder, int PageIndex, int PageSize, out int ReCount)
        {
            DataTable tbl = new DataTable();
            if (PrimaryKey == string.Empty)
                throw new Exception("LzSoft.DAL.D_Common.GetDataTableByPager�Ĵ���,����ֵ����ȷ:�����ֶ����Ʋ���Ϊ��");

            using (SqlConnection conn = new SqlConnection(CONNECTIONSTRING))
            {
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = "SP_Common_Pager";
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@TableName", SqlDbType.VarChar, 500).Value = TableName;
                cmd.Parameters.Add("@PrimaryKey", SqlDbType.VarChar, 500).Value = PrimaryKey;
                cmd.Parameters.Add("@SelectField", SqlDbType.VarChar, 4000).Value = SelectFiled;
                cmd.Parameters.Add("@SqlWhere", SqlDbType.VarChar, 4000).Value = SqlWhere;
                cmd.Parameters.Add("@SqlOrder", SqlDbType.VarChar, 4000).Value = SqlOrder;
                cmd.Parameters.Add("@PageIndex", SqlDbType.Int).Value = PageIndex;
                cmd.Parameters.Add("@PageSize", SqlDbType.Int).Value = PageSize;
                cmd.Parameters.Add("@ReCount", SqlDbType.Int);
                cmd.Parameters[7].Direction = ParameterDirection.Output;
                SqlDataAdapter ad = new SqlDataAdapter(cmd);
                ad.Fill(tbl);
                ReCount = int.Parse(cmd.Parameters[7].Value.ToString());
            }
            return tbl;
        }
        #endregion

        #region д��������Ĺ��λ
        public static void InsertErrorPosition(string positionId,string newid)
        {
            using (SqlConnection conn = new SqlConnection(CONNECTIONSTRING))
            {
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = " INSERT INTO T_SitePositionStatus SELECT '" + newid + "', *,GETDATE() FROM [T_SitePosition] WHERE [C_SitePositionID] = '" + positionId + "'";
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
        #endregion
    }
}
