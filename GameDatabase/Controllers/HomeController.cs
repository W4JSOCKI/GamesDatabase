using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using GameDatabase.Models;
using X.PagedList;

namespace GameDatabase.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DataContext _context;

        public HomeController(ILogger<HomeController> logger, DataContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index(string searchString, int? page)
        {   
            var gamesWithDevelopers = _context.Games
                .Join(
                    _context.Developers,
                    game => game.DeveloperId,
                    developer => developer.Id,
                    (game, developer) => new // obiekt anonimowy
                    {
                        GameId = game.Id,
                        GameTitle = game.Title,
                        GamePlatform = game.Platform,
                        GameReleaseYear = game.ReleaseYear,
                        GameGenre = game.Genre,
                        DeveloperName = developer.Name,
                        DeveloperCountry = developer.Country
                    }
                )
                .Where(joined => joined.GameTitle.ToLower().Contains((searchString ?? "").ToLower()))
                .OrderBy(joined => joined.GameId)
                .ToPagedList(page ?? 1, 10);

            return View(gamesWithDevelopers);
        }


        // formularz edycji
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var game = _context.Games.FirstOrDefault(g => g.Id == id);
            if (game == null)
            {
                return NotFound();
            }
            return View(game);
        }

        // przetwarzanie po zapisaniu
        [HttpPost]
        public IActionResult Edit(Game updatedGame)
        {
            if (ModelState.IsValid)
            {
                var existingGame = _context.Games.FirstOrDefault(g => g.Id == updatedGame.Id);

                if (existingGame != null)
                {
                    existingGame.Title = updatedGame.Title;
                    existingGame.Platform = updatedGame.Platform;
                    existingGame.ReleaseYear = updatedGame.ReleaseYear;
                    existingGame.Genre = updatedGame.Genre;
                    existingGame.Developer = updatedGame.Developer;
                    existingGame.DeveloperId = updatedGame.DeveloperId;

                    _context.SaveChanges();

                    return RedirectToAction("Games");
                }
                else
                {
                    return NotFound();
                }
            }

            return View(updatedGame);
        }

        public IActionResult Delete(int id)
        {
            var game = _context.Games.FirstOrDefault(g => g.Id == id);
            if (game == null)
            {
                return NotFound();
            }

            _context.Games.Remove(game);
            _context.SaveChanges();

            return RedirectToAction("Games");
        }

        public IActionResult Games(string searchString, int? page)
        {
            int pageSize = 15;
            int pageNumber = (page ?? 1);
            var products = _context.Games.AsQueryable();
            //filtrowanie gier
            if (!string.IsNullOrEmpty(searchString))
            {
                products = products.Where(p => p.Title.ToLower().Contains(searchString.ToLower()));
            }

            ViewBag.SearchString = searchString;

            int totalItems = products.Count();

            var pagedProducts = products.ToPagedList(pageNumber, pageSize);

            ViewBag.TotalItems = totalItems;
            ViewBag.PageSize = pageSize;
            ViewBag.PageNumber = pageNumber;

            return View(pagedProducts);
        }

        public IActionResult AddGame()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateGame(Game newGame)
        {
            if (ModelState.IsValid)
            {
                // Pobierz ostatnie ID i zwiększ o 1
                var lastId = _context.Games.OrderByDescending(g => g.Id).FirstOrDefault()?.Id ?? 0;
                var newId = lastId + 1;

                // Ustaw nowe właściwości gry na podstawie przekazanego obiektu newGame
                var gameToAdd = new Game
                {
                    Id = newId,
                    Title = newGame.Title,
                    Platform = newGame.Platform,
                    ReleaseYear = newGame.ReleaseYear,
                    Genre = newGame.Genre,
                    DeveloperId = newGame.DeveloperId
                };

                _context.Games.Add(gameToAdd);
                _context.SaveChanges();

                return RedirectToAction("Games");
            }

            return View("AddGame", newGame);
        }

        public IActionResult Developers(string searchString, int? page)
        {
            int pageSize = 15;
            int pageNumber = (page ?? 1);
            var products = _context.Developers.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                products = products.Where(p => p.Name.ToLower().Contains(searchString.ToLower()));
            }

            ViewBag.SearchString = searchString;

            int totalItems = products.Count();

            var pagedProducts = products.ToPagedList(pageNumber, pageSize);

            ViewBag.TotalItems = totalItems;
            ViewBag.PageSize = pageSize;
            ViewBag.PageNumber = pageNumber;

            return View(pagedProducts);
        }



        public IActionResult AddDeveloper()
        {
            return View();
        }

        public IActionResult CreateDeveloper(Developer newDeveloper)
        {
            if (ModelState.IsValid)
            {
                var lastId = _context.Developers.OrderByDescending(g => g.Id).FirstOrDefault()?.Id ?? 0;
                var newId = lastId + 1;

                var devToAdd = new Developer
                {
                    Id = newId,
                    Name = newDeveloper.Name,
                    Country = newDeveloper.Country
                };

                _context.Developers.Add(devToAdd);
                _context.SaveChanges();

                return RedirectToAction("Developers");
            }

            return View("AddDeveloper", newDeveloper);
        }

        [HttpGet]
        public IActionResult EditDev(int id)
        {
            var dev = _context.Developers.FirstOrDefault(g => g.Id == id);
            if (dev == null)
            {
                return NotFound();
            }
            return View(dev);
        }

        [HttpPost]
        public IActionResult EditDev(Developer updatedDev)
        {
            if (ModelState.IsValid)
            {
                var existingDev = _context.Developers.FirstOrDefault(g => g.Id == updatedDev.Id);

                if (existingDev != null)
                {
                    existingDev.Name = updatedDev.Name;
                    existingDev.Country = updatedDev.Country;
                    _context.SaveChanges();
                    return RedirectToAction("Developers");
                }
                else
                {
                    return NotFound();
                }
            }

            return View(updatedDev);
        }

        public IActionResult DeleteDev(int id)
        {
            var dev = _context.Developers.FirstOrDefault(g => g.Id == id);
            if (dev == null)
            {
                return NotFound();
            }

            _context.Developers.Remove(dev);
            _context.SaveChanges();

            return RedirectToAction("Developers");
        }



        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
