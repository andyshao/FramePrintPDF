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
    internal class PrintExport
    {
        public void Export(PdfDoc mc, object[] dataset)
        {
            // node
            if (dataset[0] != null)
            {
                InputNode node = new InputNode();
                // gfx登録
                node.NodePDF(mc, (List<List<string[]>>)dataset[0]);
            }

            // member
            if (dataset[3] != null)
            {
                InputMember member = new InputMember();
                // gfx登録
                member.MemberPDF(mc, (List<string[]>)dataset[3]);
            }

            // element
            if (dataset[1] != null && dataset[2] != null)
            {
                InputElement element = new InputElement();
                // gfx登録 (mc,tltle,data)
                element.ElementPDF(mc, (List<string>)dataset[1], (List<List<string[]>>)dataset[2]);
            }

            // fixnode
            if (dataset[4] != null && dataset[5] != null)
            {
                InputFixNode fixnode = new InputFixNode();
                // gfx登録 (mc,tltle,data)
                fixnode.FixNodePDF(mc, (List<string>)dataset[4], (List<List<string[]>>)dataset[5]);
            }

            // joint
            if (dataset[6] != null && dataset[7] != null)
            {
                InputJoint joint = new InputJoint();
                // gfx登録 (mc,tltle,data)
                joint.JointPDF(mc, (List<string>)dataset[6], (List<List<string[]>>)dataset[7]);
            }

            // noticepoints
            if (dataset[8] != null)
            {
                InputNoticePoints noticepoints = new InputNoticePoints();
                // gfx登録 (mc,tltle,data)
                noticepoints.NoticePointsPDF(mc, (List<List<string[]>>)dataset[8]);
            }

            // fixmember
            if (dataset[9] != null && dataset[10] != null)
            {
                InputFixMember fixmember = new InputFixMember();
                // gfx登録 (mc,tltle,data)
                fixmember.FixMemberPDF(mc, (List<string>)dataset[9], (List<List<string[]>>)dataset[10]);
            }

            // shell
            if (dataset[11] != null)
            {
                InputShell shell = new InputShell();
                // gfx登録 (mc,tltle,data)
                shell.ShellPDF(mc, (List<List<string[]>>)dataset[11]);
            }

            // loadname
            if (dataset[12] != null)
            {
                InputLoadName loadname = new InputLoadName();
                // gfx登録 (mc,tltle,data)
                loadname.LoadNamePDF(mc, (List<string[]>)dataset[12]);
            }

            // load
            if (dataset[13] != null && dataset[14] != null)
            {
                InputLoad load = new InputLoad();
                // gfx登録 (mc,tltle,data)
                load.LoadPDF(mc, (List<string>)dataset[13], (List<List<List<string[]>>>)dataset[14]);
            }

            // define
            if (dataset[15] != null)
            {
                InputDefine define = new InputDefine();
                // gfx登録 (mc,tltle,data)
                define.DefinePDF(mc, (List<List<string[]>>) dataset[15]);
            }

            // combine
            if (dataset[16] != null)
            {
                InputCombine combine = new InputCombine();
                // gfx登録 (mc,tltle,data)
                combine.CombinePDF(mc, (List<List<string[]>>)dataset[16]);
            }

            // pickup
            if (dataset[17] != null)
            {
                InputPickup pickup = new InputPickup();
                // gfx登録 (mc,tltle,data)
                pickup.PickupPDF(mc, (List<List<string[]>>)dataset[17]);
            }
        }
    }
}
