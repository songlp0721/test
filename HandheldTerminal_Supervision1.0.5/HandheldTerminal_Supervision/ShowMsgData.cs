using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Windows.Forms;

namespace HandheldTerminal_Supervision
{
    public class ShowMsgData
    {
       
        public ShowMsgData(Card card)
        {
            //执行卡内解码
            cardid = card.id;
            try
            {
                planlistno = card.CardDataDic[8].Replace("\0", "").Trim() + card.CardDataDic[9].Replace("\0", "").Trim();
                carrierno = card.CardDataDic[6].Replace("\0", "").Trim();
                incardno = card.CardDataDic[1].Replace("\0", "").Trim();
                mtrlname = card.CardDataDic[44].Trim() + card.CardDataDic[45].Trim() + card.CardDataDic[46].Trim();
                fromdesc = card.CardDataDic[48].Trim() + card.CardDataDic[49].Trim() + card.CardDataDic[50].Trim();
                todesc = card.CardDataDic[52].Trim() + card.CardDataDic[53].Trim() + card.CardDataDic[54].Trim();
                modelname = card.CardDataDic[21].Trim();
                flag = card.CardDataDic[28];
                flow = card.CardDataDic[2];
                drivername = card.CardDataDic[10];
                samplingman = card.CardDataDic[36];
                intendanceman = card.CardDataDic[37];
                unloadcarman = card.CardDataDic[38];
                standwgt = card.CardDataDic[40];
                stivewgt = card.CardDataDic[41];
                inner = card.CardDataDic[42];
                string[] inners = inner.Split('$');
                try
                {
                    incarno = inners[0];//内部车号
                    //driverno = inners[1];//司机编号
                    deptno = inners[1];//部门编号
                }
                catch { }
                //incarno = card.CardDataDic[42];//内部车号[xeliven][2015-10-26]
                place = card.CardDataDic[29];
                char[] places = place.ToCharArray();
                try
                {
                    switch (flow.ToCharArray()[0])
                    {
                        case '0':
                            grossstation = places[3].ToString() + places[4].ToString() + places[5].ToString();
                            break;
                        case '1':
                        case '2':
                            tartstation = places[3].ToString() + places[4].ToString() + places[5].ToString();
                            break;
                    }
                }
                catch { }
           
                samplingstation = places[6].ToString() + places[7].ToString() + places[8].ToString();
                intedancestation = places[9].ToString() + places[10].ToString() + places[11].ToString();
                unloadcarstation = places[12].ToString() + places[13].ToString() + places[14].ToString();
                otherwgt = card.CardDataDic[25].Replace("\0", "").Trim();
                if (otherwgt == "") { otherwgt = "0"; }
                try
                {
                    samplingdatetime = DateTime.ParseExact(card.CardDataDic[32].Replace("\0", "").Trim(), "yyyyMMddHHmmss", null).ToString("yyyy-MM-dd HH:mm:ss");
                }
                catch { }
                try
                {
                    intendancedatetime = DateTime.ParseExact(card.CardDataDic[33].Replace("\0", "").Trim(), "yyyyMMddHHmmss", null).ToString("yyyy-MM-dd HH:mm:ss");
                }
                catch { }
                try
                {
                    unloadcardatetime = DateTime.ParseExact(card.CardDataDic[34].Replace("\0", "").Trim(), "yyyyMMddHHmmss", null).ToString("yyyy-MM-dd HH:mm:ss");
                }
                catch { }
            }
            catch { }

        }

        public string cardid;//卡编号
        public string carrierno;//车号
        public string planlistno; //计划单号;
        public string incardno; //内部编号
        public string mtrlname;//需方
        public string fromdesc;//供方
        public string todesc;//需方
        public string modelname; //规格(外发)
        public string flag;//标志
        public string flow;//流程
        public string drivername;//司机
        public string samplingman;//取样人
        public string intendanceman;//监督人
        public string unloadcarman;//卸车人
        public string samplingdatetime="";//取样时间
        public string intendancedatetime="";//确认时间
        public string unloadcardatetime="";//卸车时间
        public string samplingstation = "";//取样地点
        public string intedancestation = "";//确认地点
        public string unloadcarstation = "";//卸车地点
        public string carcolor="";//车身颜色
        public string retinue = "";//随行人员
        public string affix = "";//随行物品
        public string grosswgt = "";//重检重量
        public string grossstation = "";//重检磅房
        public string grossdatetime = "";//重检时间
        public string loadgorssman = "";//重检人员
        public string tartwgt = "";//空检重量
        public string tartstation = "";//空检磅房
        public string tartdatetime = "";//空检时间
        public string loadtartman = "";//空检人员
        public string place = "";//地点
        public string otherwgt = "";//杂量
        public string carno = "";//车编号
        public string entertime = "";//入厂时间
        public string standwgt = "";//标重
        public string stivewgt = "";//扣粉重
        public string incarno = "";//内部车号[xeliven][2015-10-26]
        public string inner = "";//内部信息
        public string driverno = "";//内部信息
        public string deptno = "";//部门
        


        public delegate void GetWcfOverEventHandle();
        public event GetWcfOverEventHandle GetWcfOverEvent;

        public void GetWcfData()
        {
            #region 查询数据库
            
            try
            {
                PublicStatic.Wcf.BeginGetDataTable(PublicStatic.User, "G01", PublicStatic.CreatePamarDataSet(planlistno, carrierno), new AsyncCallback(CallBack), "AsycState:OK");
            }
            catch (Exception ex)
            {
                MessageBox.Show("获取远程数据库失败,部分数据无法显示!\r\n错误信息:" + ex.Message);
            }
            #endregion
        }
        private void CallBack(IAsyncResult asyncResult)
        {
            //数据格式
            //table0 =为一检信息 oneloadwgt,oneloadman,oneloaddatetime
            //table1 =为二检信息 grosswgt,tartwgt,grossdatetime,tartdatetime,loadgrossman,loadtartman,ticketman,ticketshift,ticketcrew,grossstationno,tartstationno
            //table2 =为车辆信息 cartype ,carcolor,retinue,affix
            DataSet ds  =null;
            
            try
            {
                ds = PublicStatic.Wcf.EndGetDataTable(asyncResult);
            }
            catch (Exception ex) { MessageBox.Show("获取远程数据库失败,部分数据无法显示!\r\n错误信息:" + ex.Message); }
            
            try{
                if (ds != null)
                {   //执行数据库解码
                    
                    DataTable dt0 = ds.Tables[0];//一检信息
                    DataTable dt1 = ds.Tables[1];//二检信息
                    if (dt0.Rows.Count != 0)
                    {
                        switch (flow.ToCharArray()[0])
                        {
                            case '0':
                                grosswgt = dt0.Rows[0]["oneloadwgt"].ToString();//一检毛重
                                grossdatetime = dt0.Rows[0]["oneloaddatetime"].ToString();//一检时间
                                loadgorssman = dt0.Rows[0]["oneloadman"].ToString();//一检人员 
                                if (dt1.Rows.Count != 0)
                                {
                                    tartwgt = dt1.Rows[0]["tartwgt"].ToString();//二检皮重
                                    tartdatetime = dt1.Rows[0]["tartdatetime"].ToString();//二检时间
                                    loadtartman = dt1.Rows[0]["loadtartman"].ToString();//二检人员
                                    tartstation = dt1.Rows[0]["tartstationno"].ToString(); //二检磅房
                                }
                                break;
                            case '1':
                            case '2':
                                tartwgt = dt0.Rows[0]["oneloadwgt"].ToString();//一检皮重
                                tartdatetime = dt0.Rows[0]["oneloaddatetime"].ToString();//一检时间
                                loadtartman = dt0.Rows[0]["oneloadman"].ToString();//一检人员 
                                if (dt1.Rows.Count != 0)
                                {
                                    grosswgt = dt1.Rows[0]["grosswgt"].ToString();//二检毛重
                                    grossdatetime = dt1.Rows[0]["grossdatetime"].ToString();//二检时间
                                    loadgorssman = dt1.Rows[0]["loadgrossman"].ToString();//二检人员 
                                    grossstation = dt1.Rows[0]["grossstationno"].ToString();//二检磅房
                                }
                                break;

                        }
                    }

                    DataTable dt2 = ds.Tables[2];//获取车辆信息;
                    if (dt2.Rows.Count != 0)
                    {
                        carcolor = dt2.Rows[0]["carcolor"].ToString(); //车颜色
                        retinue = dt2.Rows[0]["retinue"].ToString(); //随行人员
                        affix = dt2.Rows[0]["affix"].ToString(); //随行人员
                        carno = dt2.Rows[0]["carno"].ToString(); //随行人员
                    }
                    DataTable dt3 = ds.Tables[3];//获取其他信息
                    if (dt3.Rows.Count != 0)
                    {
                        entertime = dt3.Rows[0]["enterdatetime"].ToString();//入厂时间
                    }
                    if (GetWcfOverEvent != null) { GetWcfOverEvent(); }

                }
                if (ds != null) { ds.Dispose(); }
            }
            catch (Exception ex)
            {
                MessageBox.Show("解码服务器回传信息出错!\r\n错误信息:"+ex.Message);
            }
        }
    }
}
