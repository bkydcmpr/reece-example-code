using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Reece.Example.MVC.UnitTesting.Biz.Interfaces;
using Reece.Example.MVC.UnitTesting.Models;

namespace Reece.Example.MVC.UnitTesting.Controllers
{
    public class ProspectController : Controller
    {
        public IProspectRequest ProspectRequest { get; set; }

        public ActionResult New()
        {
            return View(new Prospect());
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult New( Prospect prospect )
        {
            Guid id = ProspectRequest.AddProspect(prospect.Name, prospect.Phone);
            Prospect result = new Prospect()
                                  {
                                      ID = id,
                                      Name = prospect.Name,
                                      Phone = prospect.Phone
                                  };
            return Json(result);
        }

    }
}
