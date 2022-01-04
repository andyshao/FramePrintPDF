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
    internal class InputCombine
    {
        private Dictionary<string, object> value = new Dictionary<string, object>();

        public List<List<string[]>> Combine(PdfDoc mc, Dictionary<string, object> value_)
        {
            value = value_;
            //nodeデータを取得する
            var target = JObject.FromObject(value["combine"]).ToObject<Dictionary<string, object>>();

            // 集まったデータはここに格納する
            List<List<string[]>> combine_data = new List<List<string[]>>();
            List<string[]> body = new List<string[]>();


            for (int i = 0; i < target.Count; i++)
            {
                string[] line1 = new String[10];
                string[] line2 = new String[10];
                line1[0] = target.ElementAt(i).Key;
                line2[0] = "";

                var item = JObject.FromObject(target.ElementAt(i).Value);

                // 荷重名称
                if (item.ContainsKey("name"))
                {
                    line1[1] = mc.TypeChange(item["name"]);
                }
                else
                {
                    line1[1] = "";
                }
                line2[1] = "";

                //Keyをsortするため
                var itemDic = JObject.FromObject(target.ElementAt(i).Value).ToObject<Dictionary<string, object>>();
                string[] kk = itemDic.Keys.ToArray();
                Array.Sort(kk);

                int count = 0;

                for (int j = 0; j < kk.Length - 2; j++)
                {
                    line1[count + 2] = kk[j].Replace("C", "");
                    line2[count + 2] = mc.TypeChange(item[kk[j]], 2);
                    count++;

                    if (count == 8)
                    {
                        body.Add(line1);
                        body.Add(line2);
                        count = 0;
                        line1 = new String[10];
                        line2 = new String[10];
                        line1[0] = "";
                        line1[1] = "";
                        line2[0] = "";
                        line2[1] = "";
                    }
                }
                if (count > 0)
                {
                    for (int k = 2; k < 10; k++)
                    {
                        line1[k] = line1[k] == null ? "" : line1[k];
                        line2[k] = line2[k] == null ? "" : line2[k];
                    }

                    body.Add(line1);
                    body.Add(line2);
                }
            }
            if (body.Count > 0)
            {
                combine_data.Add(body);
            }
            return combine_data;

        }

        public void CombinePDF(PdfDoc mc, List<List<string[]>> combineData)
        {
            int bottomCell = mc.bottomCell;

            // 全行の取得
            int count = 2;
            for (int i = 0; i < combineData.Count; i++)
            {
                count += (combineData[i].Count + 2) * mc.single_Yrow;
            }
            // 改ページ判定
            mc.DataCountKeep(count);

            //  タイトルの印刷
            mc.PrintContent("Combineデータ", 0);
            mc.CurrentRow(2);
            //　ヘッダー
            string[,] header_content = {
                { "CombNo","荷重名称", "C1", "C2", "C3", "C4" , "C5", "C6", "C7", "C8"}
            };

            // ヘッダーのx方向の余白
            int[,] header_Xspacing = {
                 { 17, 100,203, 233, 263, 293, 323, 353, 383, 413},
            };

            mc.Header(header_content, header_Xspacing);

            // ボディーのx方向の余白
            int[,] body_Xspacing = {
                 { 24, 42,210, 240, 270, 300, 330, 360, 390, 420},
            };

            for (int i = 0; i < combineData.Count; i++)
            {
                for (int j = 0; j < combineData[i].Count; j++)
                {
                    for (int l = 0; l < combineData[i][j].Length; l++)
                    {
                        mc.CurrentColumn(body_Xspacing[0, l]); //x方向移動
                        if (l == 1)
                        {
                            mc.PrintContent(combineData[i][j][l], 1);  // print
                        }
                        else
                        {
                            mc.PrintContent(combineData[i][j][l]);  // print
                        }
                    }
                    mc.CurrentRow(1);
                }
            }

        }
    }
}

