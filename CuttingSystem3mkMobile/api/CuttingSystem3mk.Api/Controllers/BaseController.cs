﻿using AutoMapper;
using CuttingSystem3mk.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CuttingSystem3mk.WebApi.Controllers
{
    public class BaseController : ControllerBase
    {
        public BaseController
            (
                Service service,
                IMapper mapper
            )
        {
            Service = service;
            Mapper = mapper;
        }
        protected Service Service;
        protected IMapper Mapper;
    }
}
