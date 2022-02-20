using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;

namespace RegisterParcelsFromPC
{
    partial class Periodic_check : Form1
    {
        MakeSQLCommand sqlstr = new MakeSQLCommand();


        public void periodical_check()
        {
            try
            {

                //10秒に一度呼ばれる
                //deletable_eventを更新する
                //具体的に言うと、登録もしくは受取イベントで、is_finishedが0であり、5分以上経過しているイベントに対して
                //・slackを送信する（荷物受け取り引き渡しはryosei_uidとparcel_uid、slack_idの3点が必要、本人確認はparcel_uidは不要）
                //・is_finishedを1にする

                DateTime dt = DateTime.Now;
                int deletable_time = int.Parse(ConfigurationManager.AppSettings["deletable_time"]);
                string real_time = dt.ToString("yyyy-MM-dd HH:mm:ss");//実際の時間
                string base_time = dt.AddSeconds(-1 * deletable_time).ToString("yyyy-MM-dd HH:mm:ss");//操作取り消しの時間を考慮した時間

                List<string> target_uid = ope.get_all_parcels_uid(sqlstr.toGetList_PeriodicCheck(base_time,"1"));//先に受け取りの更新対象の一覧（parcel_uidの一覧）を取得
                target_uid.AddRange(ope.get_all_parcels_uid(sqlstr.toGetList_PeriodicCheck(base_time, "2")));//先に引き渡しの更新対象の一覧（parcel_uidの一覧）を取得
                target_uid.AddRange(ope.get_all_parcels_uid(sqlstr.toGetList_PeriodicCheck(real_time, "20")));
                foreach (string an_event in target_uid)
                {
                    string msg;
                    switch (get_eventtype_from_eventuid(an_event))
                    {
                        case "1":
                            msg = "こんあじり！　事務室に荷物が届いたよ！ (" + get_registerdatetime_from_eventuid(an_event) + ")";
                            send_slack(get_parceluid_from_eventuid(an_event), msg, 1);

                            break;
                        case "2":
                            string parceluid = get_parceluid_from_eventuid(an_event);
                            msg = "荷物が受取されたよ！。(" + get_registerdatetime_from_eventuid(an_event) + "事務室到着分)";
                            send_slack(parceluid, msg, 2);
                            ope.calculate_registered_time2(parceluid);
                            break;
                        case "20":
                            msg = "本人確認が完了したよ！これから荷物が来たらお知らせするね！よろしく！";
                            send_slack_complete_identify(get_ryoseiuid_from_eventuid(an_event), msg,20);
                            break;
                    }


                }


                ope.execute_sql(sqlstr.toPeriodicCheck(base_time,"1"));//is_finished=1にするupdate文をかます
                ope.execute_sql(sqlstr.toPeriodicCheck(base_time, "2"));//is_finished=1にするupdate文をかます
                ope.execute_sql(sqlstr.toPeriodicCheck(real_time, "20"));//is_finished=1にするupdate文をかます


                //更新対象があったときのみ、イベントテーブルを再描画
                if (target_uid.Count > 0)
                {
                    show_parcels_eventTable();
                }


                //一時間に一度通信をする（slackのトークン無効時間がよくわからないため）
                DateTime now = DateTime.Now;
                if (int.Parse(now.Minute.ToString()) == 0 && int.Parse(now.Second.ToString()) < 10)
                {
                    Httppost httppost = new Httppost();
                    string user_code = ConfigurationManager.AppSettings["slack_testuser"];
                    string message_str = $"定期通信だよ！";
                    httppost.posting_DM(user_code, message_str);
                }

            }
            catch
            {

            }
            finally
            {

                //数秒に一回起動してるので、インスタンスを破棄しないとメモリ食いまくってそのうち落ちる
                Dispose();
            }

        }
        public string get_eventtype_from_eventuid(string event_uid)
        {

            string event_type = ope.select_one_xx(sqlstr.toSelect_eventtype_from_eventuid(event_uid), "event_type");
            return event_type;
        }
        public string get_registerdatetime_from_eventuid(string event_uid)
        {

            string parcel_uid = ope.select_one_xx(sqlstr.toSelect_registerdatetime_from_eventuid(event_uid), "created_at");
            return parcel_uid;
        }

        public string get_ryoseiuid_from_eventuid(string event_uid)
        {
            //event uidをキーとしてryosei_uidを取得する

            string ryosei_uid = ope.select_one_xx(sqlstr.toSelect_ryoseiuid_from_eventuid(event_uid), "ryosei_uid");
            return ryosei_uid;
        }

        public string get_parceluid_from_eventuid(string event_uid)
        {
            //event uidをキーとしてparcels_uidを取得する

            string parcel_uid = ope.select_one_xx(sqlstr.toSelect_parceluid_from_eventuid(event_uid), "parcel_uid");
            return parcel_uid;
        }

        public void send_slack(string parcel_uid, string message_str, int event_type)
        {
            //QRコードを一時的に作成するパスの名前

            string nowtime = DateTime.Now.ToString("HH-mm-ss");
            string filename = $@"C:\temp\{nowtime}.gif";


            //parcel uidをキーとしてowner_uidを取得,owner_uidをキーとしてslack_uidを取得する

            string owner_uid = ope.select_one_xx(sqlstr.toSelect_ryoseiuid_from_parceluid(parcel_uid), "owner_uid");
            string slack_uid = ope.select_one_xx(sqlstr.toSelect_slackid_from_ryoseiuid(owner_uid), "slack_id");
            int ryosei_status = ope.select_one_xx_int(sqlstr.toSelect_status_from_ryoseiuid(owner_uid), "status");
            if (slack_uid == "" || slack_uid == "null"||ryosei_status==5) return;

            Httppost httppost = new Httppost();
            switch (event_type)
            {
                case 1:
                    //qrコード作成
                    QrCode qr = new QrCode();
                    qr.QRcodeCreate(parcel_uid, filename);

                    //slackに送信
                    //"U026B4NDH0T";//俺のやつ
                    //"U026C106W0K"; //わたるの
                    httppost.posting_DM_image(slack_uid, filename, message_str);

                    File.Delete(filename);

                    break;
                case 2:
                    httppost.posting_DM(slack_uid, message_str);
                    break;
            }

        }

        //parcels_uidではなくowrner_uidから送信する。
        public void send_slack_complete_identify(string owner_uid, string message_str, int event_type)
        {
            //QRコードを一時的に作成するパスの名前

            string nowtime = DateTime.Now.ToString("HH-mm-ss");
            string filename = $@"C:\temp\{nowtime}.gif";


            //parcel uidをキーとしてowner_uidを取得,owner_uidをキーとしてslack_uidを取得する

            string slack_uid = ope.select_one_xx(sqlstr.toSelect_slackid_from_ryoseiuid(owner_uid), "slack_id");
            int ryosei_status = ope.select_one_xx_int(sqlstr.toSelect_status_from_ryoseiuid(owner_uid), "status");
            if (slack_uid == "" || slack_uid == "null" || ryosei_status == 5) return;

            Httppost httppost = new Httppost();
            switch (event_type)
            {
                
                case 20:
                    httppost.posting_DM(slack_uid, message_str);
                    break;
            }

        }
    }
}
