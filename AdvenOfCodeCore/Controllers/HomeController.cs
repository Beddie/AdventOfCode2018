using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AdvenOfCodeCore.Models;
using Logic;
using System.Text;
using System.Net;
using System.IO;
using System.Net.Http;

namespace AdvenOfCodeCore.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var overviewmodel = RenderDay.GetOverview();
            return View("Index", overviewmodel);
        }

        public ActionResult RenderDayPart()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Details(int id, int part)
        {
            var overviewmodel = RenderDay.GetOverview();

            var AoCDay = overviewmodel.Where(c => c.ID == id).FirstOrDefault();
            if (AoCDay != null)
            {
                var puzzleString = string.Empty;
                var sw = new Stopwatch();
                sw.Start();

                switch (part)
                {
                    case 0:
                        puzzleString = AoCDay.Part1();
                        break;
                    case 1:
                        puzzleString = AoCDay.Part2();
                        break;
                    default:
                        break;
                }
                ViewBag.puzzleString = string.Format("Answer = {0} ({1} ms)", puzzleString, sw.ElapsedMilliseconds);
                sw.Stop();
            }

            return View("Index", overviewmodel);
        }

        public ActionResult AllDetails()
        {
            var puzzleString = string.Empty;
            var sw = new Stopwatch();
            var overviewmodel = RenderDay.GetOverview();
            var sb = new StringBuilder();

            sw.Start();
            var startTime = sw.ElapsedMilliseconds;
            foreach (var day in overviewmodel)
            {
                var day1 = day.Part1();
                var day1Time = sw.ElapsedMilliseconds - startTime;
                var day2 = day.Part2();
                var day2Time = sw.ElapsedMilliseconds - startTime - day1Time;
                sb.AppendLine($"{day.Name} part1 answer: {day1} ({day1Time} ms), part2 answer: {day2} ({day2Time} ms))");
                startTime = sw.ElapsedMilliseconds;
            }
            sw.Stop();
            sb.AppendLine($"Total executionTime {sw.ElapsedMilliseconds} ms");
            ViewBag.puzzleString = sb.ToString();

            return View("Index", overviewmodel);
        }

        //TODO make private leaderboard JSON with cookie WORK :)
        public async Task<ActionResult> GetLeaderBoard()
        {
            var overviewmodel = RenderDay.GetOverview();
            var leaderboard = await GetJSON();
            ViewBag.leaderboard = leaderboard;
            return View("Index", overviewmodel);
        }

        private async Task<string> GetJSON()
        {
            using (var httpClient = new HttpClient())
            {
                var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, "https://adventofcode.com/2018/leaderboard/private/view/205926.json");
                httpRequestMessage.Headers.Add("Cookie", "{\"key\":\"2018 - 11 - 30499\",\"target\":1544504398}");
                var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);
                var httpContent = httpResponseMessage.EnsureSuccessStatusCode();
                string result = await httpContent.Content.ReadAsStringAsync();
                return result;
            }
        }
    }
}
