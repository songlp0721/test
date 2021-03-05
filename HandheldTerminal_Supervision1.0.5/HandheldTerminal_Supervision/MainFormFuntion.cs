using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace HandheldTerminal_Supervision
{
    public partial class MainForm
    {
        public enum CommandType:int
        { 
            /// <summary>
            /// 确认
            /// </summary>
            Yes = 0,
            /// <summary>
            /// 取消确认
            /// </summary>
            no= 9,
            /// <summary>
            /// 强制确认
            /// </summary>
            compel= 1,
        }

        /// <summary>
        /// 判断是否符合流程
        /// </summary>
        /// <param name="isok">命令类型</param>
        /// <param name="flownumber">流程编号</param>
        /// <returns>是否符合流程</returns>
        private bool IsAccordWithFlow(CommandType isok, int flownumber)
        {
            
            if (!_cardReadManager.TestCard()) { MessageBox.Show("未读到卡片!"); return false; }
            if (isok == CommandType.compel)
            { //强制取样
                    return true;
            }
            char[] flags = data.flag.ToCharArray();
            string theflow = data.flow.Substring(1);//流程编码

            int k = theflow.IndexOf(flownumber.ToString());
            if (k != 0)//如果不是为第一个流程则
            {
                string previousflow = theflow.Substring(k - 1, 1);
                if (!PublicStatic.Flow2FlagDic.ContainsKey(previousflow)) { MessageBox.Show("流程标识不在字典内,流程标识为:'" + previousflow + "'"); return false; }
                int flagbit = PublicStatic.Flow2FlagDic[previousflow];
                char theflagchar = flags[flagbit];
                if (theflagchar != '1' && theflagchar != '2')
                {
                    MessageBox.Show("尚未获得" + PublicStatic.FlagExplainDic[flagbit] + "不能进行" + PublicStatic.FlagExplainDic[flownumber] + "操作!"); return false;
                }
                return true;
            }
            MessageBox.Show("未知错误!");
            return false;
        }


        //取样确认
        private void Sampling(CommandType isok)
        {
            try
            {
                if (!IsAccordWithFlow(isok, 2)) { return; }

                char[] flags = data.flag.ToCharArray();
                switch (isok)
                {
                    case CommandType.compel:
                    case CommandType.Yes:
                        flags[2] = '1';
                        break;
                    case CommandType.no:
                        flags[2] = '0';
                        break;
                }

                string datetime = DateTime.Now.ToString("yyyyMMddHHmmss");
                char[] places = data.place.ToCharArray();
                char[] sites = PublicStatic.Site.ToCharArray();
                places[6] = sites[0];
                places[7] = sites[1];
                places[8] = sites[2];
                bool iswriteflags = _cardReadManager.WriteCard(28, new StringBuilder().Append(flags).ToString());
                bool iswritedatetime = _cardReadManager.WriteCard(32, datetime);
                bool iswriteuser = _cardReadManager.WriteCard(36, PublicStatic.User);
                bool iswriteplace = _cardReadManager.WriteCard(29, new StringBuilder().Append(places).ToString());
                if (iswriteflags && iswritedatetime && iswriteuser && iswriteplace )
                {
                    if (PublicStatic.IsNetMode)
                    {
                        string mystr = ((int)isok).ToString();
                        try
                        {
                            //写数据库:
                            if (PublicStatic.Wcf.GetDataTable(
                                PublicStatic.User,
                                "S01",
                                PublicStatic.CreateCommandDataSet(data.planlistno, data.carrierno, data.cardid, data.incardno, mystr, DateTime.ParseExact(datetime, "yyyyMMddHHmmss", null).ToString(), data.samplingstation, PublicStatic.User)
                                ) == null)
                            {
                                //写入失败
                            }
                            //播放语音
                            SoundManager.PlaySound(0);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("写入数据库错误!\r\n1.请检查网络\r\n2.是否数据服务器尚未开启\r\n调试信息:" + ex.Message);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("写卡错误!");
                }
                _cardReadManager.Rest();
                _cardReadManager.ReadCard();
            }
            catch (Exception ex)
            {
                MessageBox.Show("取样出错,错误信息:" + ex.Message);
            }
        }
        //监督确认
        private void Intendance(CommandType isok)
        {
            try
            {

                if (!IsAccordWithFlow(isok, 3)) { return; }

                char[] flags = data.flag.ToCharArray();
                switch (isok)
                { 
                    case CommandType.Yes:
                    case CommandType.compel:
                        flags[3] = '1';
                        break;
                    case CommandType.no:
                        flags[3] = '0';
                        break;
                }
                string datetime = DateTime.Now.ToString("yyyyMMddHHmmss");
                char[] places = data.place.ToCharArray();
                char[] sites = PublicStatic.Site.ToCharArray();
                places[9] = sites[0];
                places[10] = sites[1];
                places[11] = sites[2];
                bool iswriteflag = _cardReadManager.WriteCard(28, new StringBuilder().Append(flags).ToString());
                bool iswritedatetime = _cardReadManager.WriteCard(33, datetime);
                bool iswriteuser = _cardReadManager.WriteCard(37, PublicStatic.User);
                bool iswriteplace = _cardReadManager.WriteCard(29, new StringBuilder().Append(places).ToString());
                if (iswriteflag && iswritedatetime && iswriteuser && iswriteplace)
                {
                    if (PublicStatic.IsNetMode)
                    {
                        try
                        {
                            //写数据库:
                            if (PublicStatic.Wcf.GetDataTable(
                                PublicStatic.User,
                                "S02",
                                PublicStatic.CreateCommandDataSet(data.planlistno, data.carrierno, data.cardid, data.incardno, ((int)isok).ToString(), DateTime.ParseExact(datetime, "yyyyMMddHHmmss", null).ToString(), PublicStatic.Site, PublicStatic.User)
                                ) == null
                                )
                            {
                                //写入失败
                            }
                            //播放语音
                            SoundManager.PlaySound(0);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("写入数据库错误!\r\n1.请检查网络\r\n2.是否数据服务器尚未开启\r\n调试信息:" + ex.Message);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("写卡失败!");
                }
                _cardReadManager.Rest();
                _cardReadManager.ReadCard();
            }
            catch (Exception ex)
            {
                MessageBox.Show("监督出错,错误信息:" + ex.Message);
            }
        }
        //卸车确认
        private void UnloadCar(CommandType isok)
        {
            try
            {
                if (!IsAccordWithFlow(isok, 4)) { return; }

                data.otherwgt = textBox19.Text;//获取杂量
                data.stivewgt = textBox20.Text;//获取扣粉

                char[] flags = data.flag.ToCharArray();
                switch (isok)
                {
                    case CommandType.Yes:
                    case CommandType.compel:
                        flags[4] = '1';
                        break;
                    case CommandType.no:
                        flags[4] = '0';
                        break;
                }
                string datetime = DateTime.Now.ToString("yyyyMMddHHmmss");
                char[] places = data.place.ToCharArray();
                char[] sites = PublicStatic.Site.ToCharArray();
                places[12] = sites[0];
                places[13] = sites[1];
                places[14] = sites[2];

                bool iswriteflag = _cardReadManager.WriteCard(28, new StringBuilder().Append(flags).ToString());
                bool iswritedatetime = _cardReadManager.WriteCard(34, datetime);
                bool iswriteuser = _cardReadManager.WriteCard(38, PublicStatic.User);
                bool iswriteplace = _cardReadManager.WriteCard(29, new StringBuilder().Append(places).ToString());
                bool iswriteother = _cardReadManager.WriteCard(25, data.otherwgt); //扣杂
                bool iswritePM2pot5 = _cardReadManager.WriteCard(41,data.stivewgt);//扣粉

                _cardReadManager.WriteCard(42, textBox21.Text.Trim()  + "$" + comboBox1.Text.Trim());//写入内部卡号[xeliven][2015-10-26]



                if (iswriteflag && iswritedatetime && iswriteuser && iswriteplace && iswriteother&&iswritePM2pot5)
                {
                    if (PublicStatic.IsNetMode)
                    {
                        try
                        {
                            //写数据库:
                            if (
                                PublicStatic.Wcf.GetDataTable(
                                PublicStatic.User,
                                "S03",
                                PublicStatic.CreateCommandDataSet(data.planlistno, data.carrierno, data.cardid, data.incardno, ((int)isok).ToString(), DateTime.ParseExact(datetime, "yyyyMMddHHmmss", null).ToString(), PublicStatic.Site, PublicStatic.User, data.otherwgt,data.stivewgt)
                                ) == null
                                )
                            {
                                //写入失败
                            }
                            //播放语音
                            SoundManager.PlaySound(0);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("写入数据库错误!\r\n1.请检查网络\r\n2.是否数据服务器尚未开启\r\n调试信息:" + ex.Message);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("写卡失败!");
                }
                _cardReadManager.Rest();
                _cardReadManager.ReadCard();
            }
            catch (Exception ex)
            {
                MessageBox.Show("监督出错,错误信息:" + ex.Message);
            }
        }
        //退库确认
        private void RetrnOfGoods(CommandType isok)
        {//退库
            try
            {
                if (!IsAccordWithFlow(isok, 4)) { return; }
                char[] flags = data.flag.ToCharArray();
                switch (isok)
                {
                    case CommandType.Yes:
                    case CommandType.compel:
                        flags[14] = '1';
                        break;
                    case CommandType.no:
                        flags[14] = '0';
                        break;
                }
                string datetime = DateTime.Now.ToString("yyyyMMddHHmmss");
                char[] places = data.place.ToCharArray();
                char[] sites = PublicStatic.Site.ToCharArray();
                places[9] = sites[0];
                places[10] = sites[1];
                places[11] = sites[2];
                bool iswriteflag = _cardReadManager.WriteCard(28, new StringBuilder().Append(flags).ToString());
                bool iswritedatetime = _cardReadManager.WriteCard(33, datetime);
                bool iswriteuser = _cardReadManager.WriteCard(37, PublicStatic.User);
                bool iswriteplace = _cardReadManager.WriteCard(29, new StringBuilder().Append(places).ToString());
                if (iswriteflag && iswritedatetime && iswriteuser && iswriteplace)
                {
                    if (PublicStatic.IsNetMode)
                    {
                        try
                        {

                            //写数据库:
                            if (PublicStatic.Wcf.GetDataTable(
                                PublicStatic.User,
                                "S04",
                                PublicStatic.CreateCommandDataSet(data.planlistno, data.carrierno, data.cardid, data.incardno, ((int)isok).ToString(), DateTime.ParseExact(datetime, "yyyyMMddHHmmss", null).ToString(), PublicStatic.Site, PublicStatic.User)
                                ) == null
                                )
                            {
                                //写入失败
                            }
                            //播放语音
                            SoundManager.PlaySound(0);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("写入数据库错误!\r\n1.请检查网络\r\n2.是否数据服务器尚未开启\r\n调试信息:" + ex.Message);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("写卡失败!");
                }
                _cardReadManager.Rest();
                _cardReadManager.ReadCard();
            }
            catch (Exception ex)
            {
                MessageBox.Show("退库出错,错误信息:" + ex.Message);
            }
        }
        //出厂确认
        private void AwayConfirm(CommandType isok)
        {//出厂确认
            try
            {
                if (!_cardReadManager.TestCard()) { MessageBox.Show("未读到卡片!"); return; }
                char[] flags = data.flag.ToCharArray();
                if ((flags[5] != '1' && flags[5] != '2') && _IsIntendance && isok != CommandType.compel)
                { MessageBox.Show("尚未进行一检\r\n不允许进行出厂操作!"); SoundManager.PlaySound(0); return; }
                if (flags[15] != '1' && _IsIntendance && _IsSampling && isok != CommandType.compel)
                { MessageBox.Show("尚未进行二检\r\n不允许进行出厂操作!"); SoundManager.PlaySound(0); return; }
                string datetime = "";
                switch (isok)
                {
                    case CommandType.Yes:
                    case CommandType.compel:
                        flags[0] = '1';
                        break;
                    case CommandType.no:
                        flags[0] = '0';
                        break;

                }
                datetime = DateTime.Now.ToString("yyyyMMddHHmmss");
                char[] places = data.place.ToCharArray();
                char[] sites = PublicStatic.Site.ToCharArray();
                places[9] = sites[0];
                places[10] = sites[1];
                places[11] = sites[2];
                bool iswriteflag = _cardReadManager.WriteCard(28, new StringBuilder().Append(flags).ToString());
                if (iswriteflag)// && iswritedatetime && iswriteuser && iswriteplace)
                {
                    if (PublicStatic.IsNetMode)
                    {
                        try
                        {
                            //写数据库:
                            if (PublicStatic.Wcf.GetDataTable(
                                PublicStatic.User,
                                "S05",
                                PublicStatic.CreateCommandDataSet(data.planlistno, data.carrierno, data.cardid, data.incardno, ((int)isok).ToString(), DateTime.ParseExact(datetime, "yyyyMMddHHmmss", null).ToString(), PublicStatic.Site, PublicStatic.User)
                                ) == null
                                )
                            {
                                //写入失败
                            }
                            //播放语音
                            SoundManager.PlaySound(0);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("写入数据库错误!\r\n1.请检查网络\r\n2.是否数据服务器尚未开启\r\n调试信息:" + ex.Message);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("写卡失败!");
                }
                _cardReadManager.Rest();
                _cardReadManager.ReadCard();
            }
            catch (Exception ex)
            {
                MessageBox.Show("出厂出错,错误信息:" + ex.Message);
            }
        }
    }
}
