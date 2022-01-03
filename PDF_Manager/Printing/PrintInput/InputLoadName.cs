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
    internal class InputLoadName
    {
        private Dictionary<string, object> value = new Dictionary<string, object>();

        public List<string[]> LoadName(PdfDoc mc, Dictionary<string, object> value_)
        {
            value = value_;

            var target = JObject.FromObject(value["load"]).ToObject<Dictionary<string, object>>();

            // 集まったデータはここに格納する
            List<string[]> loadname_data = new List<string[]>();

            for (int i = 0; i < target.Count; i++)
            {
                var item = JObject.FromObject(target.ElementAt(i).Value);

                string[] line = new String[8];
                line[0] = target.ElementAt(i).Key;
                line[1] = mc.TypeChange(item["rate"]);
                line[2] = mc.TypeChange(item["symbol"]);
                line[3] = mc.TypeChange(item["name"]);
                line[4] = mc.TypeChange(item["fix_node"]);
                line[5] = mc.TypeChange(item["element"]);
                line[6] = mc.TypeChange(item["fix_member"]);
                line[7] = mc.TypeChange(item["joint"]);
                loadname_data.Add(line);
            }
            return loadname_data;
        }

        public void LoadNamePDF(PdfDoc mc, List<string[]> loadnameData)
        {
            int bottomCell = mc.bottomCell;

            // 全行数の取得
            double count = (loadnameData.Count + ((loadnameData.Count / bottomCell) + 1) * 4) * mc.single_Yrow;
            //  改ページ判定
            mc.DataCountKeep(count);// 全行の取得

            //  タイトルの印刷
            mc.PrintContent("荷重名称データ", 0);
            mc.CurrentRow(2);

            //　ヘッダー
            string[,] header_content = {
                { "Case", "割増", "", "","","構造系条件","",""},
                { "No", "係数", "記号", "荷重名称", "支点","断面","バネ","結合"}
            };

            // ヘッダーのx方向の余白
            int[,] header_Xspacing = {
                 { 10, 50, 100, 205, 300,370,380,420 },
                 { 10, 50, 100, 205, 310,350,390,430 },
            };

            mc.Header(header_content, header_Xspacing);

            // ボディーのx方向の余白
            int[,] body_Xspacing = {
               { 17, 57, 85, 140, 317,357,397,437 },
            };

            for (int i = 0; i < loadnameData.Count; i++)
            {
                for (int j = 0; j < loadnameData[i].Length; j++)
                {
                    mc.CurrentColumn(body_Xspacing[0, j]); //x方向移動
                    if (j == 2) // 記号，名称のみ左詰め
                    {
                        var text = mc.GetText(loadnameData[i][j], 10);  // 10文字を超えたら削除
                        mc.PrintContent(text, 1);  // print
                    }
                    else if (j == 3) // 荷重名称，名称のみ左詰め
                    {
                        var text = mc.GetText(loadnameData[i][j], 18);　//　18文字を超えたら削除
                        mc.PrintContent(text, 1);  // print
                    }
                    else
                    {
                        mc.PrintContent(loadnameData[i][j]);  // print
                    }
                }
                mc.CurrentRow(1);
            }

        }
    }
}

