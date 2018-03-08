using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AspergillosisEPR.Lib.Flash;

namespace AspergillosisEPR.Controllers
{
    public class BaseController : Controller
    {

        public void SetFlash(FlashMessageType type, string text)
        {
            TempData["FlashMessage.Type"] = type;
            TempData["FlashMessage.Text"] = text;
        }

    }
}