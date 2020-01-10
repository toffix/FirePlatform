﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using FirePlatform.WebApi.Services.Tools;
using Newtonsoft.Json;

namespace FirePlatform.WebApi.Model.Template
{
    public class ComboItem
    {
        private string _displayName = String.Empty;
        private string _groupKey = String.Empty;

        public string DisplayName { get => _displayName; set => _displayName = value?.Trim().ToLower() ?? string.Empty; }
        public string GroupKey { get => _groupKey; set => _groupKey = value?.Trim().ToLower() ?? string.Empty; }

        public string[] Keys
        {
            get => GroupKey?.Split(',');
        }
        public bool IsVisible { get; set; }

        public ComboItem()
        {
            _visConditionNameVaribles = new List<string>();
            IsVisible = true;
        }

        [NonSerialized]
        private List<string> _visConditionNameVaribles;
        [JsonIgnore]
        public List<string> VisConditionNameVaribles
        {
            get => _visConditionNameVaribles;
            set => _visConditionNameVaribles = value;
        }
        [NonSerialized]
        private string _visCondition = String.Empty;
        [JsonIgnore]
        public string VisCondition
        {
            get => _visCondition;
            set
            {
                _visCondition = value?.Trim().ToLower() ?? string.Empty;
                if (!string.IsNullOrEmpty(_visCondition))
                {
                    VisConditionNameVaribles.AddRange(CalculationTools.GetVaribliNames(_visCondition));
                    VisConditionNameVaribles = VisConditionNameVaribles.Distinct().ToList();
                }
            }
        }

        [NonSerialized]
        private List<KeyValuePair<string, List<DataDependItem>>> _dependToItems;
        [JsonIgnore]
        public List<KeyValuePair<string, List<DataDependItem>>> DependToItems { get => _dependToItems; set => _dependToItems = value; }
    }
}
