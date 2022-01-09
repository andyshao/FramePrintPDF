﻿using Newtonsoft.Json.Linq;
using PDF_Manager.Printing;
using PdfSharpCore;
using PdfSharpCore.Drawing;
using PdfSharpCore.Fonts;
using PdfSharpCore.Pdf;
using PdfSharpCore.Utils;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Json;
using System.Text.Json;
using Newtonsoft.Json;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace PDF_Manager.Printing
{
    internal class ResultReacAnnexing
    {
        private Dictionary<string, object> value = new Dictionary<string, object>();
        List<string> title = new List<string>();
        string[] type = {
            "x方向の支点反力 最大",
            "x方向の支点反力 最小",
            "y方向の支点反力 最大",
            "y方向の支点反力 最小",
            "z方向の支点反力 最大",
            "z方向の支点反力 最小",
            "x軸回りの回転反力 最大",
            "x軸回りの回転反力 最小",
            "y軸回りの回転反力 最大",
            "y軸回りの回転反力 最小",
            "z軸回りの回転反力 最大",
            "z軸回りの回転反力 最小",
        };
        List<List<List<string[]>>> dataCombine = new List<List<List<string[]>>>();
        List<List<List<string[]>>> dataPickup = new List<List<List<string[]>>>();
        public List<List<List<string[]>>> dataLL = new List<List<List<string[]>>>();

        /// <summary>
        /// Combine/Pickup反力データの読み取り
        /// </summary>
        /// <param name="mc">PdfDoc</param>
        /// <param name="value_">全データ</param>
        /// <param name="key">combine,pickupのいずれか</param>
        public void ReacAnnexing(PdfDoc mc, Dictionary<string, object> value_, string key)
        {
            value = value_;
            //nodeデータを取得する
            var target = JObject.FromObject(value["reac" + key]).ToObject<Dictionary<string, object>>();

            // 集まったデータはここに格納する
            title = new List<string>();
            switch (key)
            {
                case "Combine":
                    dataCombine = new List<List<List<string[]>>>();
                    break;
                case "Pickup":
                    dataPickup = new List<List<List<string[]>>>();
                    break;
            }

            for (int i = 0; i < target.Count; i++)
            {
                var Elem = JObject.FromObject(target.ElementAt(i).Value).ToObject<Dictionary<string, object>>();

                // タイトルを入れる．
                title.Add("Case." + target.ElementAt(i).Key);

                dataTreat(mc, Elem, key);
            }

        }

        /// <summary>
        /// 基本形以外のデータを取得する（ResultReac.csの判定でLLであった場合もここで読み取る）
        /// </summary>
        /// <param name="mc">PdfDoc</param>
        /// <param name="Elem">1caseぶんのデータ</param>
        /// <param name="key">combine,pickup,LLのいずれか</param>
        public void dataTreat(PdfDoc mc, Dictionary<string, object> Elem, string key)
        {
            List<List<string[]>> table = new List<List<string[]>>();
            for (int j = 0; j < Elem.Count; j++)
            {
                Dictionary<string, object> elist = JObject.FromObject(Elem.ElementAt(j).Value).ToObject<Dictionary<string, object>>();

                List<string[]> body = new List<string[]>();

                for (int k = 0; k < elist.Count; k++)
                {
                    var item = JObject.FromObject(elist.ElementAt(k).Value); ;
                    string[] line = new String[8];

                    line[0] = mc.TypeChange(elist.ElementAt(k).Key);
                    line[1] = mc.TypeChange(item["tx"], 2);
                    line[2] = mc.TypeChange(item["ty"], 2);
                    line[3] = mc.TypeChange(item["tz"], 2);
                    line[4] = mc.TypeChange(item["mx"], 2);
                    line[5] = mc.TypeChange(item["my"], 2);
                    line[6] = mc.TypeChange(item["mz"], 2);
                    line[7] = mc.TypeChange(item["case"]);

                    body.Add(line);
                }
                table.Add(body);
            }

            //keyに応じたListに挿入する
            switch (key)
            {
                case "Combine":
                    dataCombine.Add(table);
                    break;
                case "Pickup":
                    dataPickup.Add(table);
                    break;
                case "LL":
                    dataLL.Add(table);
                    break;
            }
        }

        /// <summary>
        /// Combine/Pickup/LL反力データのPDF書き込み（LLのみcase1つ当たりの処理）
        /// </summary>
        /// <param name="mc">PdfDoc</param>
        /// <param name="key">combine,pickup,LLのいずれか</param>
        /// <param name="title_LL">LLにかぎりケース番号を取得 ex)case2</param>
        /// <param name="LL_count">dataLLの何番目に必要なデータがあるか</param>
        public void ReacAnnexingPDF(PdfDoc mc, string key, string title_LL = "", int LL_count = 0)
        {
            //　ヘッダー
            string[,] header_content = {
                { "SUPPORT", "TX", "TY", "TZ", "MX", "MY","MZ","組合せ" },
                { "",  "(kN)", "(kN)", "(kN)", "(kN・m)", "(kN・m)", "(kN・m)","" },
            };

            // ヘッダーのx方向の余白
            int[,] header_Xspacing = {
                { 18, 70, 130, 190, 250, 310, 370,420 },
                { 18, 70, 130, 190, 250, 310, 370,420 },
            };

            // ボディーのx方向の余白　-1
            int[,] body_Xspacing = {
                { 23, 85, 145, 215, 265, 325,385,435 }
            };

            mc.header_content = header_content;
            mc.header_Xspacing = header_Xspacing;
            mc.body_Xspacing = body_Xspacing;

            switch (key)
            {
                case "Combine":
                    mc.PrintResultAnnexingReady("reac", key, title, type, dataCombine, 14);
                    break;

                case "Pickup":
                    mc.PrintResultAnnexingReady("reac", key, title, type, dataPickup, 14);
                    break;

                case "LL":
                    mc.PrintResultAnnexing(title_LL, type, dataLL[LL_count], 14);
                    break;
            }


            //// 全行の取得
            //int count = 2;
            //for (int i = 0; i < title.Count; i++)
            //{
            //    for (int j = 0; j < data[i].Count; j++)
            //    {
            //        for (int k = 0; k < data[i][j].Count; k++)
            //        {
            //            count += (data[i].Count * 5 + data[i][j].Count * 2 + data[i][j][k].Length) * mc.single_Yrow + 1;
            //        }
            //    }
            //}

            //// 改ページ判定
            //mc.DataCountKeep(count, "reac" + key);

            // 印刷
            //mc.PrintResultAnnexing(title, type, data, header_content, header_Xspacing, body_Xspacing,10);

        }
    }
}
