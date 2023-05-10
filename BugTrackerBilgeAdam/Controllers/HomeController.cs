using BugTrackerIdentityProject.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BugTrackerIdentityProject.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private ApplicationDbContext _context;
        private readonly UserManager<Kullanici> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public HomeController(ApplicationDbContext context, UserManager<Kullanici> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            var liste = _context.Hatalar.Include(x => x.User).ToList();
            List<HataViewModel> vm = new List<HataViewModel>();
            foreach (var item in liste)
            {
                vm.Add(new HataViewModel
                {
                    KullaniciAdi = item.User.UserName,
                    HataBaslik = item.HataBaslik,
                    HataDetay = item.HataDetay,
                    HataId = item.Id,
                    KullaniciId = item.UserId


                });
            }
            return View(vm);
        }
        [HttpGet]
        public IActionResult Ekle()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Ekle(Hata model)
        {

            model.UserId = _userManager.GetUserId(HttpContext.User); 
            _context.Hatalar.Add(model);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult Guncelle(int id)
        {
            var hata = _context.Hatalar.First(x => x.Id == id);


            return View(hata);
        }

        [HttpPost]
        public IActionResult Guncelle(Hata model)
        {
            _context.Update(model);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        public IActionResult Detay(int id)
        {
            var detay = _context.Hatalar.Include(x => x.User ).Where(x => x.Id == id);
            List<DetayViewModel> Dvm = new List<DetayViewModel>();
            foreach (var item in detay)
            {
                Dvm.Add(new DetayViewModel
                {
                    HataBaslik = item.HataBaslik,
                    HataDetay = item.HataDetay,
                    HataDurum = item.HataDurum,
                    KayıtTarihi = item.KayitTarihi,
                    HataId = item.Id,
                    KullaniciAdi = item.User.UserName,
                    KullaniciId = item.User.Id,
                    

                });
            }
           
            return View(Dvm);
        }
        public IActionResult Sil(int id)
        {
            var result = _context.Hatalar.FirstOrDefault(x => x.Id == id);
            _context.Hatalar.Remove(result);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult Profil(string id)
        {
            var result = _context.Users.First(x => x.Id == id);
            return View(result);
        }
        [HttpGet]
        public IActionResult YorumEkle()
        {
          
            return View();
        }
        [HttpPost]
        public IActionResult YorumEkle(Yorum model)
        {
            
            model.UserId = _userManager.GetUserId(HttpContext.User);
            _context.Yorumlar.Add(model);
            _context.SaveChanges();
            return RedirectToAction("Detay");
        }

    }
}