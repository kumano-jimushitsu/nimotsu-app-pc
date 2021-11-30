using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;

namespace RegisterParcelsFromPC
{
    class Operation
    {
        public string owner_room_name, owner_ryosei_name, staff_room_name, staff_ryosei_name;
        //メンバ変数を使うのと引数で使うのの使い分けの仕方がわからない


        public string connStr;
        public Operation(string connStr)
        {
            this.connStr = connStr;
        }


        public void execute_sql(string sqrstr)
        {

            using (var conn = new SqlConnection(connStr))
            {
                //接続
                conn.Open();

                //コマンド実行
                SqlCommand command_register = new SqlCommand(sqrstr, conn);
                command_register.ExecuteNonQuery();

                //接続断
                conn.Close();
            }
        }

        public List<string> get_all_uid(string sqlstr)
        {//受取時：ある寮生名義の事務室にある荷物の荷物番号をすべて取得しList<int>で返す→11/3 stringで返すよう変更
         //定期チェック時：削除可能なイベントのuidをすべて取得しListで返す
            List<string> CurrentParcels = new List<string>();
            SqlConnection con = new SqlConnection(connStr);
            con.Open();
            try
            {
                SqlCommand com = new SqlCommand(sqlstr, con);
                SqlDataReader sdr = com.ExecuteReader();

                while (sdr.Read() == true)
                {
                    string uid = (string)sdr["uid"];
                    CurrentParcels.Add(uid);
                }
                sdr.Close();
                com.Dispose();
            }
            finally
            {
                con.Close();
            }
            return CurrentParcels;
        }
        public string select_one_xx(string sqlstr, string xx)
        {
            SqlConnection con = new SqlConnection(connStr);
            con.Open();
            string uid;
            try
            {
                SqlCommand com = new SqlCommand(sqlstr, con);
                SqlDataReader sdr = com.ExecuteReader();
                sdr.Read();

                if (sdr[xx] == DBNull.Value)
                {
                    uid = "null";
                }
                else
                {
                    uid = (string)sdr[xx];
                }
                

                sdr.Close();
                com.Dispose();
            }
            finally
            {
                con.Close();
            }
            return uid;
        }

        public string calculate_registered_time(List<string> ParcelID, DateTime now, string owner_uid)//ここはリファクタリングなど一切していない
        {
            List<DateTime> RegiTime_list = new List<DateTime>();
            string total_past_waittime = "";
            string ParcelID_List = "";
            foreach (string aParcelID in ParcelID)
            {
                ParcelID_List += "'";
                ParcelID_List += aParcelID.ToString();
                ParcelID_List += "'";
                ParcelID_List += ",";
            }
            ParcelID_List = ParcelID_List.Trim(',');

            SqlConnection con = new SqlConnection(connStr);
            con.Open();
            try
            {
                //荷物の登録時間をすべて取得し、regi_time_listに格納
                string sqlstr = $@"select register_datetime from parcels where uid in ({ParcelID_List})";
                SqlCommand com = new SqlCommand(sqlstr, con);
                SqlDataReader sdr = com.ExecuteReader();

                while (sdr.Read() == true)
                {
                    DateTime regi_time = (DateTime)sdr["register_datetime"];
                    RegiTime_list.Add(regi_time);
                }

                sdr.Close();
                com.Dispose();

                //寮生のこれまでの累計時間を取得し、total_past_waittimeに格納
                string sqlstr2 = $@"select parcels_total_waittime from ryosei where uid='{owner_uid}'";
                SqlCommand com2 = new SqlCommand(sqlstr2, con);
                SqlDataReader sdr2 = com2.ExecuteReader();

                while (sdr2.Read() == true)
                {
                    total_past_waittime = (string)sdr2["parcels_total_waittime"];
                }

                sdr2.Close();
                com2.Dispose();
            }
            finally
            {
                con.Close();
            }

            //もっといいやり方があるかもしれないが、一旦これで良し
            int total_second = 0;
            //これまでの累計時間を秒になおして格納
            string[] past_working = total_past_waittime.Split(':');//23:15:24 23時間15分24秒
            total_second += int.Parse(past_working[0]) * 3600;
            total_second += int.Parse(past_working[1]) * 60;
            total_second += int.Parse(past_working[2]);

            //今回取得し加算する時間を秒に直して格納
            TimeSpan totalTimeSpan = new TimeSpan(0, 0, 0, 0);//{370.06:16:25.6079800} 370日6時間16分25秒  24時間を超えない場合は{00:00:03.8976284}のように、～日の部分がないので処理を変える
            foreach (DateTime aRegiTime in RegiTime_list)
            {
                totalTimeSpan += now - aRegiTime;
            }

            string[] working1 = totalTimeSpan.ToString().Split('.');//"370.06:16:25.6079800" 
            if (working1.Length == 3)//24時間を超える場合
            {
                total_second += int.Parse(working1[0]) * 24 * 60 * 60;
                string[] working2 = working1[1].Split(':');//06.16.25
                total_second += int.Parse(working2[0]) * 60 * 60;
                total_second += int.Parse(working2[1]) * 60;
                total_second += int.Parse(working2[2]);

            }
            else//2しかあり得ないはず、24時間を超えない場合
            {
                string[] working2 = working1[0].Split(':');//06.16.25
                total_second += int.Parse(working2[0]) * 60 * 60;
                total_second += int.Parse(working2[1]) * 60;
                total_second += int.Parse(working2[2]);
            }

            //秒数を、hour:min:secの形のstringに変換
            string new_totalTime = "";
            new_totalTime += (total_second / 3600).ToString();
            new_totalTime += ":";
            total_second %= 3600;
            new_totalTime += (total_second / 60).ToString("00");
            new_totalTime += ":";
            total_second %= 60;
            new_totalTime += total_second.ToString("00");
            return new_totalTime;
        }
        public void calculate_registered_time2(string parcelID)//ここはリファクタリングなど一切していない
        {
            DateTime regi_time, rele_time;
            string owner_uid;
            string total_past_waittime;

            SqlConnection con = new SqlConnection(connStr);
            con.Open();
            try
            {
                //荷物の登録時間をすべて取得し、regi_time_listに格納
                string sqlstr = $@"select register_datetime,release_datetime,owner_uid from parcels where uid ='{parcelID}'";
                SqlCommand com = new SqlCommand(sqlstr, con);
                SqlDataReader sdr = com.ExecuteReader();

                sdr.Read();

                owner_uid = (string)sdr["owner_uid"];
                regi_time = (DateTime)sdr["register_datetime"];
                rele_time = (DateTime)sdr["release_datetime"];


                sdr.Close();
                com.Dispose();

                //寮生のこれまでの累計時間を取得し、total_past_waittimeに格納
                string sqlstr2 = $@"select parcels_total_waittime from ryosei where uid='{owner_uid}'";
                SqlCommand com2 = new SqlCommand(sqlstr2, con);
                SqlDataReader sdr2 = com2.ExecuteReader();

                sdr2.Read();
                
                total_past_waittime = (string)sdr2["parcels_total_waittime"];
                

                sdr2.Close();
                com2.Dispose();
            }
            finally
            {
                con.Close();
            }

            //もっといいやり方があるかもしれないが、一旦これで良し
            int total_second = 0;
            //これまでの累計時間を秒になおして格納
            string[] past_working = total_past_waittime.Split(':');//23:15:24 23時間15分24秒
            total_second += int.Parse(past_working[0]) * 3600;
            total_second += int.Parse(past_working[1]) * 60;
            total_second += int.Parse(past_working[2]);

            //今回取得し加算する時間を秒に直して格納
            TimeSpan totalTimeSpan = new TimeSpan(0, 0, 0, 0);//{370.06:16:25.6079800} 370日6時間16分25秒  24時間を超えない場合は{00:00:03.8976284}のように、～日の部分がないので処理を変える

                totalTimeSpan += rele_time-regi_time;

            string[] working1 = totalTimeSpan.ToString().Split('.');//"370.06:16:25.6079800" もしくは　"06:16:25.6079800"
            if (working1.Length == 3)//24時間を超える場合
            {
                total_second += int.Parse(working1[0]) * 24 * 60 * 60;
                string[] working2 = working1[1].Split(':');//06.16.25
                total_second += int.Parse(working2[0]) * 60 * 60;
                total_second += int.Parse(working2[1]) * 60;
                total_second += int.Parse(working2[2]);

            }
            else//2しかあり得ないはず、24時間を超えない場合
            {
                string[] working2 = working1[0].Split(':');//06.16.25
                total_second += int.Parse(working2[0]) * 60 * 60;
                total_second += int.Parse(working2[1]) * 60;
                total_second += int.Parse(working2[2]);
            }

            //秒数を、hour:min:secの形のstringに変換
            string new_totalTime = "";
            new_totalTime += (total_second / 3600).ToString();
            new_totalTime += ":";
            total_second %= 3600;
            new_totalTime += (total_second / 60).ToString("00");
            new_totalTime += ":";
            total_second %= 60;
            new_totalTime += total_second.ToString("00");
            MakeSQLCommand make = new MakeSQLCommand();
            string sql=make.toUpdate_ryosei_totalwaittime(owner_uid, new_totalTime);
            execute_sql(sql);
        }

    }

}
