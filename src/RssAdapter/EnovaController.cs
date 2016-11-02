using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RssAdapter.Models;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace RssAdapter
{
    public class EnovaController : Controller
    {
        // GET: /<controller>/
        public IActionResult Index()
        {
            return new EnovaRss();
        }
    }
}
