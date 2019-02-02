﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FirePlatform.Models.Containers;
using FirePlatform.Services;
using FirePlatform.WebApi.Model;
using FirePlatform.WebApi.Model.Responses;
using FirePlatform.WebApi.Services;
using FirePlatform.WebApi.Services.Parser;
using Microsoft.AspNetCore.Mvc;

namespace FirePlatform.WebApi.Controllers
{
    [ApiController]
    public class CalculationController : BaseController
    {
        public static List<_ItemGroup> UsersTmp { get; set; }

        readonly ICalculationService _calculationService;
        public CalculationController(Service service, IMapper mapper, ICalculationService calculationService)
                              : base(service, mapper)
        {
            _calculationService = calculationService;
        }
        [HttpGet("api/[controller]/loadTmp")]
        [ProducesResponseType(200, Type = typeof(FormResponse))]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<ApiContainer<string>>> Load(int numberTmpl = 1)
        {
            var content = Download(numberTmpl);

            var res = Parser.ParseDoc(content);

            UsersTmp = res;
            return Ok(res);
        }

        [HttpGet("api/[controller]/set")]
        [ProducesResponseType(200, Type = typeof(FormResponse))]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<ApiContainer<string>>> Set(int groupId, int itemId, string value)
        {
            var group = UsersTmp.FirstOrDefault(x => x.IndexGroup == groupId);
            var item = group.Items.FirstOrDefault(x => x.NumID == itemId);
            dynamic newValue = null;
            if (item.Type == ItemType.Num.ToString())
            {
                newValue = Double.Parse(value);
            }
            else if (item.Type == ItemType.Check.ToString())
            {
                newValue = bool.Parse(value);
            }
            else if (item.Type == ItemType.Combo.ToString())
            {
                item.NameVarible = value;
                item.Value = true;
            }
            item.Value = newValue;
            item.NotifyAboutChange();
            return Ok(item.NeedNotifyItems);
        }

        private string Download(int numberTmpl = 1)
        {
            string file_contents = string.Empty;
            using (var wc = new System.Net.WebClient())
            {
                wc.Encoding = System.Text.Encoding.UTF8;// GetEncoding("Windows-1250"); 


                if (numberTmpl == 1) file_contents = wc.DownloadString("https://onedrive.live.com/download.aspx?cid=9214918BD14C3E0C&resid=9214918BD14C3E0C%21771&authkey=ANErohHGOIz32s0");
                if (numberTmpl == 2) file_contents = wc.DownloadString("https://docs.google.com/spreadsheets/u/1/d/1sPt8y5Qi8DyD2kcwr2hi7HdcmYGd0j41wzXSGPgOw-A/export?format=tsv&id=1sPt8y5Qi8DyD2kcwr2hi7HdcmYGd0j41wzXSGPgOw-A&gid=1727084202");
                if (numberTmpl == 3) file_contents = wc.DownloadString("https://docs.google.com/spreadsheets/u/1/d/11ysWSQWAW8KrCJMSDyXXu0dpNxWiDPqMzH8Tk0MJ1aE/export?format=tsv&id=11ysWSQWAW8KrCJMSDyXXu0dpNxWiDPqMzH8Tk0MJ1aE&gid=0");
                if (numberTmpl == 4) file_contents = wc.DownloadString("https://docs.google.com/spreadsheets/u/1/d/1lJ2jBcTE8hKF4zyKYzAXIjPFwf1sJkBWFw2Je_5PD7I/export?format=tsv&id=1lJ2jBcTE8hKF4zyKYzAXIjPFwf1sJkBWFw2Je_5PD7I&gid=0");
                if (numberTmpl == 5) file_contents = wc.DownloadString("https://docs.google.com/spreadsheets/u/1/d/1NEy0c9-fLpjxOeAsFIqM9LlmAYBb9b3hL29-NLke2Cs/export?format=tsv&id=1NEy0c9-fLpjxOeAsFIqM9LlmAYBb9b3hL29-NLke2Cs&gid=0");
                if (numberTmpl == 6) file_contents = wc.DownloadString("https://docs.google.com/spreadsheets/u/1/d/1WXWSW37aglC0O2YDI-ZyADuIrhBusOMPWw8752Rjs_M/export?format=tsv&id=1WXWSW37aglC0O2YDI-ZyADuIrhBusOMPWw8752Rjs_M&gid=0");
                if (numberTmpl == 7) file_contents = wc.DownloadString("https://docs.google.com/spreadsheets/u/1/d/18P5QrPytLq3c7q8epN-spVxmxDcfx-UEPIxqX7kXdqo/export?format=tsv&id=18P5QrPytLq3c7q8epN-spVxmxDcfx-UEPIxqX7kXdqo&gid=0");
                if (numberTmpl == 8) file_contents = wc.DownloadString("https://docs.google.com/spreadsheets/u/1/d/1eP5Q_S5mYm-JBOIiFqTtnQlAm9yidRJjFQNmCP99XFc/export?format=tsv&id=1eP5Q_S5mYm-JBOIiFqTtnQlAm9yidRJjFQNmCP99XFc&gid=0");
                if (numberTmpl == 9) file_contents = wc.DownloadString("https://docs.google.com/spreadsheets/u/1/d/125E669P25ayUVZb5AWmo6_pqriPICyOOpzg_po-yBno/export?format=tsv&id=125E669P25ayUVZb5AWmo6_pqriPICyOOpzg_po-yBno&gid=0");
            }
            return file_contents;
        }
    }
}