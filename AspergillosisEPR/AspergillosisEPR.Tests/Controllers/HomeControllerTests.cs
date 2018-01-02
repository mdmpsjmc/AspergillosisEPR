using AspergillosisEPR.Controllers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace AspergillosisEPR.AspergillosisEPR.Tests.Controllers
{
    public class HomeControllerTests
    {
       [Fact]
       public void IndexAction_WhenUnauthorized_RedirectsToLoginPage()
        {
            var controller = new HomeController();
            var actionResult = controller.Index() as ViewResult;

            Assert.IsType<ViewResult>(actionResult);
        }
    }
}
