﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using System.Threading.Tasks;
using AutoMapper;
using FirePlatform.Services;
using FirePlatform.Utils.AlgorithmHelpers;
using FirePlatform.WebApi.Model;
using FirePlatform.WebApi.Model.Requests;
using FirePlatform.WebApi.Model.Responses;
using FirePlatform.WebApi.Model.Template;
using FirePlatform.WebApi.Services;
using FirePlatform.WebApi.Services.Parser;
using FirePlatform.WebApi.Services.Tools;
using LZString;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using NCalc;
using Newtonsoft.Json;
using System.Runtime.Serialization.Formatters.Binary;

namespace FirePlatform.WebApi.Controllers
{
    [ApiController]
    public class CalculationsController : BaseController
    {
        readonly ICalculationService _calculationService;
        public static List<ItemDataPerUser> ItemDataPerUsers { get; set; }
        public static List<PictureResponse> Pictures { get; set; }

        public CalculationsController
            (
                Service service,
                IMapper mapper,
                ICalculationService calculationService
            )
                              : base(service, mapper)
        {
            _calculationService = calculationService;
        }
        static CalculationsController()
        {
            ItemDataPerUsers = new List<ItemDataPerUser>();
            Pictures = new List<PictureResponse>();
        }

        [HttpGet("api")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [EnableCors("AllowAll")]
        [AllowAnonymous]
        public OkObjectResult Init()
        {
            return Ok("It works");
        }

        [HttpPost("api/[controller]/LoadTmp")]
        [EnableCors("AllowAll")]
        [AllowAnonymous]
        public OkObjectResult Load([FromBody] TemplateModel request)
        {
            List<ItemGroup> res;
            var content = Download(request);

            res = Parser.PrepareControls(content);

            var isExistsUser = ItemDataPerUsers.Any(x => x.UserId == request.UserId);
            if (isExistsUser)
            {
                foreach (var data in ItemDataPerUsers)
                {
                    if (data.UserId == request.UserId)
                    {
                        if (request.IsRightTemplate)
                        {
                            data.UsersTmpRight = res;
                        }
                        else
                        {
                            data.UsersTmpLeft = res;
                        }
                        break;
                    }
                }
            }
            else
            {
                var itemDataPerUser = request.IsRightTemplate ? new ItemDataPerUser
                {
                    UserId = request.UserId,
                    UsersTmpRight = res
                } :
                new ItemDataPerUser
                {
                    UserId = request.UserId,
                    UsersTmpLeft = res
                };

                ItemDataPerUsers.Add(itemDataPerUser);
            }
            List<Item> ite = new List<Item>();
            foreach (var i in res)
            {
                ite.AddRange(i.Items.Where(x => x.IsVisible).ToList());
            }

            foreach (var group in res)
            {
                foreach (var item in group?.Items)
                    if (item != null && item.Picture?.Data != null)
                    {
                        Pictures.Add(new PictureResponse()
                        {
                            NumID = item.NumID,
                            Picture = new Picture()
                            {
                                Data = item.Picture.Data,
                                Name = item.Picture.Name,
                                Id = item.Picture.Id
                            },
                            GroupID = item.GroupID,
                        });
                        item.Picture.Data = null;
                        item.Picture.ToFetch = true;
                    }
            }
            res.ForEach(x =>
                x.IsRightTemplate = request.IsRightTemplate
            );

            return Ok(res);
        }

        [HttpGet("api/[controller]/ClearTemplates")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [EnableCors("AllowAll")]
        [Authorize]
        public ActionResult ClearTemplateDataPerUser(int userId)
        {
            //var existingDataperUser = ItemDataPerUsers.FirstOrDefault(x => x.UserId == userId);
            //if (existingDataperUser != null)
            //    existingDataperUser.UsersTmp = new List<ItemGroup>();

            return Ok();
        }

        [HttpGet("api/[controller]/FetchPicture")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [EnableCors("AllowAll")]
        [Authorize]
        public ActionResult FetchPictureForItem(int numberId, int groupId)
        {
            var response = Pictures.FirstOrDefault(x => x.NumID == numberId && x.GroupID == groupId);
            return Ok(response);
        }

        [HttpGet("api/[controller]/test-calc")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [EnableCors("AllowAll")]
        [Authorize]
        public OkObjectResult TestCalc()
        {
            var parameters = new Dictionary<string, object>()
            {
                { "a", null },
                { "b", null },
                { "c", true },
                { "d", false },
                { "e", 1 },
                {"f", "sdasdasd" }
            };
            a("c || d", parameters);
            a("a && b", parameters);
            a("a || b", parameters);
            a("a && c", parameters);
            a("a || c", parameters);
            a("a && e<=1", parameters);
            a("a>=1 && e<=1", parameters);
            a("a >= 0", parameters);
            a("f == a", parameters);
            a("'a' == a", parameters);
            a("'a' == f", parameters);
            void a(string formula, Dictionary<string, object> param)
            {
                try
                {
                    var expression = new Expression(formula, EvaluateOptions.IgnoreCase)
                    {
                        Parameters = parameters
                    };
                    expression.Evaluate();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"{formula} - {ex.Message} \n");
                }
            }
            return Ok(true);
        }

        [HttpPost("api/[controller]/Preselection")]       
        [EnableCors("AllowAll")]
        [AllowAnonymous]
        public OkObjectResult Preselection([FromBody] PreselectionRequest request)
        {
            var UsersTmp = request.IsRightTemplate ?
                                   ItemDataPerUsers.FirstOrDefault(x => x.UserId == request.UserId).UsersTmpRight :
                                   ItemDataPerUsers.FirstOrDefault(x => x.UserId == request.UserId).UsersTmpLeft;

            foreach (var group in UsersTmp)
            {
                foreach (var item in group.Items)
                {
                    if (item.Type == ItemType.Combo.ToString())
                    {
                        if (request.PreselectionEnabled)
                        {
                            if (string.IsNullOrWhiteSpace(item.Value as string))
                            {
                                item.Value = item.ComboItems?.FirstOrDefault().GroupKey;
                            }
                        }
                        else
                        {
                            item.Value = null;
                        }
                    }
                }
            }

            foreach (var group in UsersTmp)
            {
                group.UpdateGroup();
                foreach (var item in group.Items)
                {
                    if (group.IsVisible)
                        item.UpdateItem();
                    else
                        item.IsVisible = false;
                }
            }
            var resultGroups = new List<ItemGroup>();
            UsersTmp.ForEach(x => resultGroups.Add(new ItemGroup()
            {
                IndexGroup = x.IndexGroup,
                IsVisible = x.IsVisible
            }));

            var changedItems = new List<Item>();
            UsersTmp.ForEach(x => x.Items?.ForEach(y => changedItems.Add(y)));

            changedItems = changedItems.Where(x => x.IsVisible || x.IsVisible != x.IsVisiblePrev || !string.IsNullOrWhiteSpace(x.Formula)).ToList();
            (List<ItemGroup>, List<Item>) res = (groups: resultGroups, items: changedItems);
            return Ok(res);
        }
        [HttpGet("api/[controller]/Set")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [EnableCors("AllowAll")]
        [Authorize]
        public async Task<OkObjectResult> Set(int groupId = 0, int itemId = 0, string value = "", int userId = 0, bool isRightTemplate = false)
        {
            var res = Tuple.Create<List<ItemGroup>, List<Item>>(null, null);
            try
            {
                var startDate = DateTime.Now;
                var UsersTmp = isRightTemplate ? ItemDataPerUsers.FirstOrDefault(x => x.UserId == userId).UsersTmpRight :
                                                 ItemDataPerUsers.FirstOrDefault(x => x.UserId == userId).UsersTmpLeft;
                var selectedGroup = UsersTmp.FirstOrDefault(x => x.IndexGroup == groupId);
                var selectedItem = selectedGroup.Items.FirstOrDefault(x => x.NumID == itemId);
                object newValue = null;
                if (selectedItem.Type == ItemType.Num.ToString())
                {
                    newValue = double.Parse(value);
                }
                else if (selectedItem.Type == ItemType.Check.ToString())
                {
                    newValue = bool.Parse(value);
                }
                else if (selectedItem.Type == ItemType.Combo.ToString())
                {
                    selectedItem.NameVarible = value;
                    newValue = true;
                }
                selectedItem.Value = newValue;

                foreach (var group in UsersTmp)
                {
                    group.UpdateGroup();
                    foreach (var item in group.Items)
                    {
                        if (group.IsVisible)
                            item.UpdateItem();
                        else
                            item.IsVisible = false;
                    }
                }

                var resultGroups = new List<ItemGroup>();
                UsersTmp.ForEach(x => resultGroups.Add(new ItemGroup()
                {
                    IndexGroup = x.IndexGroup,
                    IsVisible = x.IsVisible
                }));

                var changedItems = new List<Item>();
                UsersTmp.ForEach(x => x.Items?.ForEach(y => changedItems.Add(y)));

                changedItems = changedItems.Where(x => x.IsVisible || x.IsVisible != x.IsVisiblePrev || !string.IsNullOrWhiteSpace(x.Formula)).ToList();
                res = Tuple.Create<List<ItemGroup>, List<Item>>(resultGroups, changedItems); ;


                // var result = DateTime.Now - startDate;
                // Debug.WriteLine($"[SET VALUE] - Time - minutes : {result.Minutes} or seconds : {result.Seconds}");
                foreach (var item in res.Item2)
                {
                    if (item.Picture?.Data != null)
                    {
                        Pictures.Add(new PictureResponse()
                        {
                            NumID = item.NumID,
                            Picture = new Picture()
                            {
                                Data = item.Picture.Data,
                                Name = item.Picture.Name,
                                Id = item.Picture.Id
                            },
                            GroupID = item.GroupID,
                        });
                    }
                }

                //foreach (var item in res.Item2)
                //{
                //    if (item.Picture != null && item.Picture.Data != null)
                //    {
                //        item.Picture.Data = null;
                //        item.Picture.ToFetch = true;
                //    }
                //}
                //var itemss = res.Item2.Where(x => x.Picture != null && x.Picture.Data != null);
            }
            catch (Exception ex)
            {

            }

            //await SaveCustomTemplate(new CustomTamplate() { MainName = "A", Name = "B" });
            return Ok(res);
        }

        [HttpGet("api/[controller]/LoadTemplates")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [EnableCors("AllowAll")]
        [Authorize]
        public OkObjectResult LoadTemplatesTest()
        {
            var templates = LoadTemplates();
            return Ok(templates);
        }


        [HttpPost("api/[controller]/SaveTemplate")]
        [EnableCors("AllowAll")]
        [AllowAnonymous]
        public async Task<OkObjectResult> SaveCustomTemplate([FromBody] CustomTamplate template)
        {
            var tmp = ItemDataPerUsers.FirstOrDefault(x => x.UserId == 1).UsersTmpLeft;
            var modified = tmp.Where(x => x.Items.Any(y => y.InitialValue != y.Value)).ToArray();
            var bytes = modified.Serialize();
            var service = Service.GetUserTemplatesService();
            await service.Save(new Models.Models.User() { Id = 1 }, template.MainName, template.Name, bytes);
            return Ok(true);
        }

        private string Download(TemplateModel templateModel)
        {
            var templates = LoadTemplates();

            var selectedTmp = templates?.FirstOrDefault(x => x.Lng == templateModel.Lng && x.ShortName == templateModel.ShortName && x.Stage == templateModel.Stage);
            string file_contents = string.Empty;
            using (var wc = new System.Net.WebClient())
            {
                wc.Encoding = System.Text.Encoding.UTF8;// GetEncoding("Windows-1250"); 
                file_contents = wc.DownloadString(selectedTmp.Link);
            }
            return file_contents;
        }

        private IList<TemplateModel> LoadTemplates()
        {
            var result = new List<TemplateModel>();
            string data = string.Empty;
            using (var wc = new System.Net.WebClient())
            {
                data = wc.DownloadString("https://docs.google.com/spreadsheets/d/1hh0mYlkmSvRQwgiIhAnnEOmHKCpg293empAKl1Kj2mc/export?format=csv&id=1hh0mYlkmSvRQwgiIhAnnEOmHKCpg293empAKl1Kj2mc&gid=0");
            }
            var items = data?.Split("\r\n").ToArray();
            if (items != null)
                for (int i = 1; i < items.Length; i++)
                {
                    var item = items[i];
                    var parts = item.Split(',');
                    result.Add(new TemplateModel()
                    {
                        Lng = parts[0],
                        ShortName = parts[1],
                        LongName = parts[2],
                        Stage = parts[3],
                        Type = parts[4],
                        Topic = parts[5],
                        Link = parts[6]
                    });
                }
            return result;
        }
    }
}