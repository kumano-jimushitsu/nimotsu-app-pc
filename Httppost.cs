using System;
using System.Collections.Generic;
using System.Web;
using System.Net;
using System.IO;
using System.Text;
using System.Text.Json;
using Newtonsoft.Json;
using System.Net.Http;
using System.Configuration;

namespace RegisterParcelsFromPC
{
    class Httppost
    {
        string connStr = ConfigurationManager.AppSettings["connStr"];
        public string token = ConfigurationManager.AppSettings["token"];
        //https://api.slack.com/apps/A02CPEV24TV/install-on-team?success=1
        //2021/10/10 スラック側でbotが無効になっていたのでreinstall、その際に変更された。
        public string channel = "Task";
        //public string user_code;
        public string channel_code;
        //public string message_str;//呼び出すときに作っておく　日本語でよい


        public void posting_DM(string user_code, string message_str)
        {
            string jsonstr_from_slack = "init";
            //DMに送るためには、DMのchannel_code(仮称）が必要。user_idとは別。
            //conversations.openにbotのtokenとuser_idをpostで送れば取得できる
            /*
             このサイトで試せる https://so-zou.jp/web-app/network/post/
             url https://slack.com/api/conversations.open
             entity body に以下を入力
                token=xoxb-2214954337954-2428290007717-2pMn0wGuBG4KDmudnV98t5Tx
                users=U026B4NDH0T

            xoxb-2214954337954-2428290007717-yEzqRsgGebTQ5BJbqWv9UPeL
             */
            //公式ドキュメント
            //        https://api.slack.com/methods/conversations.open

            try
            {
                //文字コードを指定する
                System.Text.Encoding enc =
                    System.Text.Encoding.GetEncoding("UTF-8");//slackを相手にすると、shift_jisだとだめだがUTF-8だと行ける


                //WebRequestの作成
                System.Net.WebRequest req =
                    System.Net.WebRequest.Create($"https://slack.com/api/conversations.open");

                //POST送信する文字列を作成
                string postData =
                    $"token={token}&users={user_code}";
                //バイト型配列に変換
                byte[] postDataBytes = System.Text.Encoding.ASCII.GetBytes(postData);
                //メソッドにPOSTを指定
                req.Method = "POST";
                //ContentTypeを"application/x-www-form-urlencoded"にする
                req.ContentType = "application/x-www-form-urlencoded";
                //POST送信するデータの長さを指定
                req.ContentLength = postDataBytes.Length;



                //データをPOST送信するためのStreamを取得
                System.IO.Stream reqStream = req.GetRequestStream();
                //送信するデータを書き込む
                reqStream.Write(postDataBytes, 0, postDataBytes.Length);
                reqStream.Close();

                //サーバーからの応答を受信するためのWebResponseを取得
                System.Net.WebResponse res = req.GetResponse();
                //応答データを受信するためのStreamを取得
                System.IO.Stream resStream = res.GetResponseStream();
                //受信して表示
                System.IO.StreamReader sr = new System.IO.StreamReader(resStream, enc);
                jsonstr_from_slack = sr.ReadToEnd();//{"ok":true,"no_op":true,"already_open":true,"channel":{"id":"D02CGGQABPG"}}
                Root decirial1 = JsonConvert.DeserializeObject<Root>(jsonstr_from_slack);
                //jsondicというdicに格納  一旦取り出せたことにする
                //var jsondic = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonstr_from_slack);
                //channel_code = jsondic["id"];
                channel_code = decirial1.channel.id;

                //↑これがconversations.openによるchannel_code取得の儀

                //↓これが実際のPOST

                //POST送信する文字列を作成
                string postData2 =
                    $"token={token}&channel={channel_code}&text=" +
                        System.Web.HttpUtility.UrlEncode(message_str, enc);
                //バイト型配列に変換
                byte[] postData2Bytes = System.Text.Encoding.ASCII.GetBytes(postData2);

                //WebRequestの作成
                System.Net.WebRequest req2 =
                    System.Net.WebRequest.Create("https://slack.com/api/chat.postMessage");
                //メソッドにPOSTを指定
                req2.Method = "POST";
                //ContentTypeを"application/x-www-form-urlencoded"にする
                req2.ContentType = "application/x-www-form-urlencoded";
                //POST送信するデータの長さを指定
                req2.ContentLength = postData2Bytes.Length;

                //データをPOST送信するためのStreamを取得
                System.IO.Stream req2Stream = req2.GetRequestStream();
                //送信するデータを書き込む
                req2Stream.Write(postData2Bytes, 0, postData2Bytes.Length);
                req2Stream.Close();

                //サーバーからの応答を受信するためのWebResponseを取得
                System.Net.WebResponse res2 = req2.GetResponse();
                //応答データを受信するためのStreamを取得
                System.IO.Stream res2Stream = res2.GetResponseStream();
                //受信して表示
                System.IO.StreamReader sr2 = new System.IO.StreamReader(res2Stream, enc);

                //閉じる
                sr.Close();

                //slack_event テーブルに書き込み
                MakeSQLCommand make=new MakeSQLCommand();
                string sql= make.toInsert_slack_event(1, 1, user_code, message_str, "", "");

                Operation operation = new Operation(connStr);
                operation.execute_sql(sql);
            }
            catch (Exception e) 
            {
                //slack_event テーブルに書き込み
                MakeSQLCommand make = new MakeSQLCommand();
                string sql = make.toInsert_slack_event(0, 1, user_code, message_str, jsonstr_from_slack, "");

                Operation operation = new Operation(connStr);
                operation.execute_sql(sql);

                Console.WriteLine(e.ToString());
            }
            

        }
        public async void posting_DM_image(string user_code,string filename, string message_str)
        {
            //DMに送るためには、DMのchannel_code(仮称）が必要。user_idとは別。
            //conversations.openにbotのtokenとuser_idをpostで送れば取得できる
            /*
             このサイトで試せる https://so-zou.jp/web-app/network/post/
             url https://slack.com/api/conversations.open
             entity body に以下を入力
                token=xoxb-2214954337954-2428290007717-2pMn0wGuBG4KDmudnV98t5Tx
                users=U026B4NDH0T

            xoxb-2214954337954-2428290007717-yEzqRsgGebTQ5BJbqWv9UPeL
             */
            //公式ドキュメント
            //        https://api.slack.com/methods/conversations.open

            try
            {
                //文字コードを指定する
                System.Text.Encoding enc =
                    System.Text.Encoding.GetEncoding("UTF-8");//slackを相手にすると、shift_jisだとだめだがUTF-8だと行ける


                //WebRequestの作成
                System.Net.WebRequest req =
                    System.Net.WebRequest.Create($"https://slack.com/api/conversations.open");

                //POST送信する文字列を作成
                string postData =
                    $"token={token}&users={user_code}";
                //バイト型配列に変換
                byte[] postDataBytes = System.Text.Encoding.ASCII.GetBytes(postData);
                //メソッドにPOSTを指定
                req.Method = "POST";
                //ContentTypeを"application/x-www-form-urlencoded"にする
                req.ContentType = "application/x-www-form-urlencoded";
                //POST送信するデータの長さを指定
                req.ContentLength = postDataBytes.Length;



                //データをPOST送信するためのStreamを取得
                System.IO.Stream reqStream = req.GetRequestStream();
                //送信するデータを書き込む
                reqStream.Write(postDataBytes, 0, postDataBytes.Length);
                reqStream.Close();

                //サーバーからの応答を受信するためのWebResponseを取得
                System.Net.WebResponse res = req.GetResponse();
                //応答データを受信するためのStreamを取得
                System.IO.Stream resStream = res.GetResponseStream();
                //受信して表示
                System.IO.StreamReader sr = new System.IO.StreamReader(resStream, enc);
                string jsonstr_from_slack = sr.ReadToEnd();//{"ok":true,"no_op":true,"already_open":true,"channel":{"id":"D02CGGQABPG"}}
                Root decirial1 = JsonConvert.DeserializeObject<Root>(jsonstr_from_slack);
                //jsondicというdicに格納  一旦取り出せたことにする
                //var jsondic = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonstr_from_slack);
                //channel_code = jsondic["id"];
                channel_code = decirial1.channel.id;
                //channel_code = "D02CP93LZFC";//向後君のやつ

                //↑これがconversations.openによるchannel_code取得の儀


                //画像をpostに乗っけようの儀

                using (var httpClient = new HttpClient())
                {
                    using (var request = new HttpRequestMessage(new HttpMethod("POST"), "https://slack.com/api/files.upload"))
                    {
                        request.Headers.TryAddWithoutValidation("Authorization", "Bearer "+token);

                        var multipartContent = new MultipartFormDataContent();
                        multipartContent.Add(new ByteArrayContent(File.ReadAllBytes(filename)), "file", Path.GetFileName(filename));
                        multipartContent.Add(new StringContent(message_str), "initial_comment");
                        multipartContent.Add(new StringContent(channel_code), "channels");
                        request.Content = multipartContent;

                        var response = await httpClient.SendAsync(request);
                    }
                }

                //↑ここまで



                //slack_event テーブルに書き込み
                MakeSQLCommand make = new MakeSQLCommand();
                string sql = make.toInsert_slack_event(1, 2, user_code, message_str, "", "");

                Operation operation = new Operation(connStr);
                operation.execute_sql(sql);
            }
            catch (Exception e)
            {
                //slack_event テーブルに書き込み
                MakeSQLCommand make = new MakeSQLCommand();
                string sql = make.toInsert_slack_event(0, 2, user_code, message_str, e.ToString(), "");

                Operation operation = new Operation(connStr);
                operation.execute_sql(sql);
                Console.WriteLine(e.ToString());
            }


        }


    }
}
