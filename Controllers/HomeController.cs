using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using dojodachi.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace dojodachi.Controllers
{
    public static class SessionExtensions
    {
        // We can call ".SetObjectAsJson" just like our other session set methods, by passing a key and a valuecopy
        public static void SetObjectAsJson(this ISession session, string key, object value)
        {
            // This helper function simply serializes theobject to JSON and stores it as a string in session
            session.SetString(key, JsonConvert.SerializeObject(value));
        }
        // generic type T is a stand-in indicating that we need to specify the type on retrievalcopy
        public static T GetObjectFromJson<T>(this ISession session, string key)
        {
            string value = session.GetString(key);
            // Upon retrieval the object is deserialized based on the type we specified
            return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
        }
    }
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            if(HttpContext.Session.GetObjectFromJson<dojodachi>("current") == null)
            {
                dojodachi player = new dojodachi();
                HttpContext.Session.SetObjectAsJson("current", player);
                ViewBag.fullness = player.fullness;
                ViewBag.happiness = player.happiness;
                ViewBag.meals = player.meals;
                ViewBag.energy = player.energy;
                ViewBag.status = player.status;
            }
            else if(HttpContext.Session.GetObjectFromJson<dojodachi>("current").fullness >= 100 && HttpContext.Session.GetObjectFromJson<dojodachi>("current").happiness >= 100 && HttpContext.Session.GetObjectFromJson<dojodachi>("current").energy >= 100)
            {
                return RedirectToAction("Win");
            }
            else if(HttpContext.Session.GetObjectFromJson<dojodachi>("current").fullness <= 0 || HttpContext.Session.GetObjectFromJson<dojodachi>("current").happiness <= 0)
            {
                return RedirectToAction("Lose");
            }
            else
            {
                dojodachi player = HttpContext.Session.GetObjectFromJson<dojodachi>("current");
                ViewBag.fullness = player.fullness;
                ViewBag.happiness = player.happiness;
                ViewBag.meals = player.meals;
                ViewBag.energy = player.energy;
                ViewBag.status = player.status;
            }
            return View();
        }

        [HttpGet("Lose")]
        public ViewResult Lose()
        {
            return View();
        }
        [HttpGet("Win")]
        public ViewResult Win()
        {
            return View();
        }

        [HttpPost("restart")]
        public RedirectToActionResult Restart()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }
        [HttpPost("feed")]
        public RedirectToActionResult Feed()
        {
            dojodachi player = HttpContext.Session.GetObjectFromJson<dojodachi>("current");
            Random rand = new Random();
            if (rand.Next(1,5) != 1)
            {
                if (player.meals > 0)
                {
                    player.meals -= 1;
                    TempData["meals"] = -1;
                    int fullness = rand.Next(5,11);
                    TempData["fullness"] = fullness;
                    player.fullness += fullness;
                }
            }
            else
            {
                if (player.meals > 0)
                {
                    player.meals -= 1;
                    TempData["meals"] = -1;
                    TempData["fullness"] = 0;
                }
            }
            player.status = "You fed your Dojodachi! Meals: " + TempData["meals"] + ", Fullness: " + TempData["fullness"];
            HttpContext.Session.SetObjectAsJson("current", player);
            return RedirectToAction("Index");
        }
        [HttpPost("play")]
        public RedirectToActionResult Play()
        {
            dojodachi player = HttpContext.Session.GetObjectFromJson<dojodachi>("current");
            Random rand = new Random();
            if(rand.Next(1,5) != 1)
            {
                player.energy -= 5;
                TempData["energy"] = -5;
                int happiness = rand.Next(5,11);
                player.happiness += happiness;
                TempData["happiness"] = happiness;
            }
            else
            {
                player.energy -= 5;
                TempData["energy"] = -5;
                TempData["happiness"] = 0;
            }
            player.status = "You played with your Dojodachi! Happiness: " + TempData["happiness"] + ", Energy: " + TempData["energy"];
            HttpContext.Session.SetObjectAsJson("current", player);
            return RedirectToAction("Index");
        }
        [HttpPost("work")]
        public RedirectToActionResult Work()
        {
            dojodachi player = HttpContext.Session.GetObjectFromJson<dojodachi>("current");
            Random rand = new Random();
            player.energy -= 5;
            TempData["energy"] = -5;
            int meals = rand.Next(1,4);
            player.meals += meals;
            TempData["meals"] = meals;
            player.status = "You went to work! Energy: " + TempData["energy"] + ", Meals: " + TempData["meals"];
            HttpContext.Session.SetObjectAsJson("current", player);
            return RedirectToAction("Index");
        }
        [HttpPost("sleep")]
        public RedirectToActionResult Sleep()
        {
            dojodachi player = HttpContext.Session.GetObjectFromJson<dojodachi>("current");
            Random rand = new Random();
            player.energy += 15;
            TempData["energy"] = 15;
            player.happiness -= 5;
            TempData["happiness"] = -5;
            player.fullness -= 5;
            TempData["fullness"] = -5;
            player.status = "You went to sleep! Energy: " + TempData["energy"] + ", Happiness: " + TempData["happiness"] + ", Fullness: " + TempData["fullness"];
            HttpContext.Session.SetObjectAsJson("current", player);
            return RedirectToAction("Index");
        }
    }
}
