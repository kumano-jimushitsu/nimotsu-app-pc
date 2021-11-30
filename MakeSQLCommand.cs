using System;
using System.Collections.Generic;
using System.Text;

namespace RegisterParcelsFromPC
{
    class MakeSQLCommand
    {
        //parcelsテーブル用メンバ変数
        public string parcel_uid, owner_uid, register_staff_uid, release_staff_uid;
        public string owner_room_name, owner_ryosei_name, register_datetime, register_staff_room_name, register_staff_ryosei_name,release_agent_uid;
        public int placement, fragile, is_released;
        public string release_datetime, release_staff_room_name, release_staff_ryosei_name;
        public int checked_count, is_lost;
        public string lost_datetime;
        public int is_returned;
        public string returned_datetime;
        public int is_operation_error, operation_error_type;
        public string note;
        public int parcels_is_deleted;

        //ryoseiテーブル用メンバ変数
        //uid
        public string ryosei_uid;
        public string room_name, ryosei_name, ryosei_name_kana, ryosei_name_alphabet;
        public int block_id, status, parcels_current_count, parcels_total_count;
        public string slack_id;
        public string parcels_total_waittime;
        public int last_event_id;
        public string last_event_datetime;

        //parcel_eventテーブル用メンバ変数
        public string event_uid;
        public string created_at;
        public int event_type;
        public string event_note;
        public int event_is_deleted;
        //room_name, ryosei_name, note;

        //共通項目
        //uid -> parcel, ryosei, event
        //ryoseiテーブルのuidを他のテーブルのクエリなどから呼び出すときの名前
        //created
        //note -> parcels_note, event_note

        public string forShow_ryosei_table()
        {
            string sql = $@"
SELECT 
[room_name] as '部屋番号'
,[ryosei_name] as '氏名'
,[parcels_current_count] as '荷物数'
, '登録' as '登録'
, '受取' as '受取'
,slack_id
,uid
FROM [parcels].[dbo].[ryosei] 
where block_id='{block_id}'
";

            return sql;

        }
        public string forShow_ryosei_table_night_duty_mode()
        {
            string sql = $@"
SELECT 
[room_name] as '部屋番号'
,[ryosei_name] as '氏名'
,[parcels_current_count] as '荷物数'
, '登録' as '登録'
, '受取' as '受取'
,slack_id
,uid
FROM [parcels].[dbo].[ryosei] 
where block_id='{block_id}'
and parcels_current_count >= 1
";

            return sql;

        }
        public string forShow_ryosei_table_for_management(int block_id)
        {
            string where = $"where block_id='{block_id}'";
            if (block_id < 0) where = $"where block_id>11";//9:C34、10:臨キャパで使用している。
            string sql = $@"
SELECT 
uid
,room_name
,ryosei_name
,ryosei_name_kana
,ryosei_name_alphabet
,block_id
,slack_id
,status
,parcels_current_count
,parcels_total_count
,parcels_total_waittime
,created_at
,updated_at
FROM [parcels].[dbo].[ryosei] 
{where}
";

            return sql;

        }
        public string forShow_ryosei_table_dairiRelease(int block_id)
        {
            string where = $"where block_id='{block_id}'";
            if (block_id < 0) where = $"where block_id>11";//9:C34、10:臨キャパで使用している。
            string sql = $@"
SELECT 
uid
,room_name
,ryosei_name
,ryosei_name_kana
,ryosei_name_alphabet
,block_id
,slack_id
,status
FROM [parcels].[dbo].[ryosei] 
{where}
";

            return sql;

        }

        public string forShow_event_table()
        {
            string sql = $@"
SELECT top(50)
case [event_type] when 1 then '登録' when 2 then '受取' when 3 then '削除' when 10 then '当番交代' when 11 then 'モード開始' when 12 then 'モード解除'  else 'その他' end  as '操作種類'
,uid as '#'
,[room_name] as '部屋番号'
,[ryosei_name] as '氏名　　　'
,[created_at] as '操作時刻'
,[note] as '特記事項'
,parcel_uid
,ryosei_uid
,is_finished
FROM parcel_event
where is_deleted=0
order by created_at desc
";//parcel_uidとryosei_uidは非表示に設定している
            return sql;

        }
        public string forShow_confirm_msgbox(string uid)
        {
            string sql = $@"
select * from parcels where uid = '{uid}'
";
            return sql;
        }

        public string toRegister_parcels_table()//
        {
            string sql = $@"
insert into [parcels] 
(owner_uid, owner_room_name,owner_ryosei_name,register_datetime,register_staff_uid, register_staff_room_name,register_staff_ryosei_name,placement,sharing_status) 
values 
(
'{owner_uid}'
,(select room_name from ryosei where uid='{owner_uid}')
,(select ryosei_name from ryosei where uid='{owner_uid}')
,'{register_datetime}'
,'{register_staff_uid}'
,(select room_name from ryosei where uid='{register_staff_uid}')
,(select ryosei_name from ryosei where uid='{register_staff_uid}')
,{placement}
,20
)
";

            return sql;
        }


        public string toRegister_parcelevent_table()
        {
            string sql = $@"
insert into [parcel_event] 
(created_at,event_type,ryosei_uid,room_name,ryosei_name,parcel_uid,sharing_status) 
values 
(
'{register_datetime}'
,{event_type}
,'{owner_uid}'
,(select room_name from ryosei where uid='{owner_uid}')
,(select ryosei_name from ryosei where uid='{owner_uid}')
,(select top(1) uid from parcels order by register_datetime desc) 
,20
)
";

            return sql;

        }

        public string toRegister_ryosei_table()
        {
            string sql = $@"
update ryosei
set 
parcels_current_count=(select parcels_current_count from ryosei where uid='{owner_uid}')+1
,parcels_total_count=(select parcels_total_count from ryosei where uid='{owner_uid}')+1
,last_event_datetime='{register_datetime}'
,sharing_status=21
where uid='{owner_uid}'
";

            return sql;
        }

        public string toRelease_get_all_parcels()
        {

            string sqlstr = $@"
select uid 
from parcels 
where 
owner_uid='{owner_uid}'
and is_released=0 
and is_deleted=0
";
            return sqlstr;
        }

        public string toRelease_parcels_table(List<string> ParcelID,string agent_uid)
        {//agent_uidは、代理受取でない場合は空文字列("")、代理受取の場合はstringが入る。
            is_released = 1;
            string sql = "";
            string agent = "";
            if (agent_uid != "")
            {
                agent = $@",release_agent_uid='{agent_uid}'";
            }
            foreach (string aParcelID in ParcelID)
            {
                sql += $@"
update [parcels] 
set 
is_released = {is_released}
,release_datetime='{release_datetime}'
,release_staff_uid = '{release_staff_uid}'
,release_staff_room_name=(select room_name from ryosei where uid='{release_staff_uid}')
,release_staff_ryosei_name=(select ryosei_name from ryosei where uid='{release_staff_uid}')
{agent}
,sharing_status=21
where uid ='{aParcelID}'
";

            }
            return sql;
        }

        public string toRelease_parcelevent_table(List<string> ParcelID)
        {
            string sql = "";
            event_type = 2;
            foreach (string aParcelID in ParcelID)
            {
                sql += $@"
insert into [parcel_event] 
(created_at,event_type,parcel_uid,ryosei_uid,room_name,ryosei_name,sharing_status) 
values 
(
'{release_datetime}'
,{event_type}
,'{aParcelID}'
,'{owner_uid}'
,(select room_name from ryosei where uid='{owner_uid}')
,(select ryosei_name from ryosei where uid='{owner_uid}')
,20
)
update [parcel_event] 
set
is_finished=1
,sharing_status=21
where parcel_uid='{aParcelID}' and event_type=1
";
            }
            return sql;
        }
        public string toRelease_ryosei_table(List<string> ParcelID)
        {
            int parcel_number = ParcelID.Count;
            string sql = $@"
update ryosei 
set parcels_current_count=(select parcels_current_count from ryosei where uid='{owner_uid}')-{parcel_number}
,last_event_datetime='{release_datetime}'
,sharing_status=21
where uid='{owner_uid}'
";//,parcels_total_waittime='{parcels_total_waittime}'
            return sql;


        }

        public string toDeleteLogically_event_table()
        {
            string sql = $@"
update parcel_event
set is_deleted=1
,sharing_status=21
where uid='{event_uid}'

insert into parcel_event
(created_at,event_type,parcel_uid,ryosei_uid,room_name,ryosei_name,target_event_uid,is_finished,sharing_status) 
values 
(
'{created_at}'
,3
,'{parcel_uid}'
,'{owner_uid}'
,(select room_name from ryosei where uid='{owner_uid}')
,(select ryosei_name from ryosei where uid='{owner_uid}')
,'{event_uid}'
,1
,20
)
";
            return sql;
        }

        public string toDeleteLogically_parcels_table()
        {
            string sql = $@"
update parcels
set is_deleted=1
,sharing_status=21
where uid='{parcel_uid}'";
            if (event_type == 2)
            {
                sql = $@"
update [parcels] 
set 
is_released = 0
,release_datetime=NULL
,release_staff_uid = NULL
,release_staff_room_name=NULL
,release_staff_ryosei_name=NULL
,sharing_status=21
where uid ='{parcel_uid}'
";
            }
            return sql;
        }
        /*        public string toDeleteLogically_parcels_table()
        {
            string sql = $@"
update parcels
set is_deleted=1
where uid='{parcel_uid}'";
            return sql;
        }*/
        public string toDeleteLogically_ryosei_table()
        {
            string sql = $@"
update ryosei
set 
parcels_total_count=(select parcels_total_count from ryosei where uid='{owner_uid}')-1
,parcels_current_count=(select parcels_current_count from ryosei where uid='{owner_uid}')-1
,sharing_status=21
where uid='{owner_uid}'
";
            if (event_type == 2)
            {
                sql = $@"update ryosei
set 
parcels_total_count=(select parcels_total_count from ryosei where uid='{owner_uid}')+1
,parcels_current_count=(select parcels_current_count from ryosei where uid='{owner_uid}')+1
,sharing_status=21
where uid='{owner_uid}'
";
            }
            return sql;
        }


        public string toChangeStaff_event_table()
        {
            event_type = 10;
            string sql = $@"
insert into parcel_event
(created_at,event_type,ryosei_uid,room_name,ryosei_name,parcel_uid,sharing_status) 
values 
(
'{created_at}'
,{event_type}
,'{ryosei_uid}'
,(select room_name from ryosei where uid='{ryosei_uid}')
,(select ryosei_name from ryosei where uid='{ryosei_uid}')
,0
,20
)
";
            return sql;
        }

        public string Register_new_ryosei_table()
        {
            string sql = $@"
insert into [ryosei]
(room_name,ryosei_name,ryosei_name_kana,ryosei_name_alphabet,block_id,slack_id,sharing_status)
values
(
'{room_name}'
,'{ryosei_name}'
,'{ryosei_name_kana}'
,'{ryosei_name_alphabet}'
,{block_id}
,'{slack_id}'
,20
)
";
            return sql;
        }
        public string Register_new_ryosei_atonetime(List<string[]> list)
        {
            //3,A303,前田,敏貴,まえだ,としき,Toshiki Maeda

            string sql = $@"
insert into [ryosei]
(room_name,ryosei_name,ryosei_name_kana,ryosei_name_alphabet,block_id,sharing_status)
values";
            foreach (string[] ryosei in list)
            {
                sql += $@"('{ryosei[1]}' ,'{ryosei[2]}','{ryosei[4]}','{ryosei[6]}',{ryosei[0]},20),";
            }
            sql=sql.Remove(sql.Length - 1);
            return sql;

        }

        public string toChangeMode()
        {
            string sql = $@"
insert into [parcel_event] 
(created_at,event_type,sharing_status) 
values 
(
'{created_at}'
,{event_type} 
,20
)
";
            return sql;
        }

        public string toRegister_slack()
        {
            string sql = $@"
update ryosei
set slack_id = '{slack_id}'
,sharing_status=21
where ryosei_name = '{ryosei_name}'
and room_name = '{room_name}'
";
            return sql;
        }

        public string toPeriodicCheck()
        {
            string sql = $@"
update parcel_event 
set is_finished=1 
,sharing_status=21
where event_type<=2 and is_finished=0 and created_at<'{created_at}'
";
            return sql;
        }
        public string toGetList_PeriodicCheck()
        {
            string sql = $@"
select uid 
from parcel_event
where event_type<=2 and is_finished=0 and created_at<'{created_at}'
";
            return sql;
        }
        public string toEdit_ryosei_for_management()
        {
            string sql = $@"
update ryosei
set 
room_name='{room_name}'
,ryosei_name='{ryosei_name}'
,ryosei_name_kana='{ryosei_name_kana}'
,ryosei_name_alphabet='{ryosei_name_alphabet}'
,slack_id='{slack_id}'
,block_id={block_id}
,status={status}
,sharing_status=21
where uid='{ryosei_uid}';
";
            return sql;
        }
        public string toSelect_eventtype_from_eventuid()
        {
            string sql = $@"
select convert(nvarchar,(select event_type from parcel_event where uid='{event_uid}'))as event_type;
";
            return sql;
        }
        public string toSelect_registerdatetime_from_eventuid()
        {
            string sql = $@"
select convert(nvarchar,(select register_datetime from parcels where uid=(select parcel_uid from parcel_event where uid = '{event_uid}')),120)as created_at;
";
            //120という数字は、sqlserverで定義された日時時刻のフォーマットを指定するための値
            return sql;
        }
        public string toSelect_parceluid_from_eventuid()
        {
            string sql = $@"
select parcel_uid from parcel_event where uid='{event_uid}';
";
            return sql;
        }
        public string toSelect_ryoseiuid_from_parceluid()
        {
            string sql = $@"
select owner_uid from parcels where uid='{parcel_uid}';
";
            return sql;
        }
        public string toSelect_slackid_from_ryoseiuid()
        {
            string sql = $@"
select slack_id from ryosei where uid='{ryosei_uid}';
";
            return sql;
        }
        public string toSelect_distinct_ryosei()
        {
            string sql = $@"
select convert(nvarchar,((select count(*) from ryosei)) - (select count(distinct ryosei_name_kana) from ryosei))as 'diff'
";
            return sql;
        }

        public string toUpdate_ryosei_totalwaittime(string ryosei_uid,string newtime)
        {
            string sql = $@"
update ryosei set parcels_total_waittime='{newtime}',sharing_status=21 where uid='{ryosei_uid}'
";
            return sql;
        }


        public string toSelect_slackid_of_ShomuBucho()
        {
            string sql = $@"
select top(1) slack_id from ryosei where status=4;
";
            return sql;
        }

    }
}
