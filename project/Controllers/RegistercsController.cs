using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using project.Data;
using project.Models;
using MimeKit;
using MailKit;
using System;
using MailKit.Net.Smtp;
using System.Globalization;
using System.Text;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using FluentEmail.Core;
namespace project.Controllers
{
    public class RegistercsController : Controller
    {
        private readonly ApplicationDbContext _context;
        
        public RegistercsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Registercs
        public async Task<IActionResult> Index()
        {
            return _context.Registercs != null ?
                        View(await _context.Registercs.ToListAsync()) :
                        Problem("Entity set 'ApplicationDbContext.Registercs'  is null.");
        }
        public async Task<IActionResult> Register()
        {
            return _context.Registercs != null ?
                         View() :
                         Problem("Entity set 'ApplicationDbContext.Registercs'  is null.");


        }
        public async Task<IActionResult> report(string begin,string end)
        {
            object ob = TempData.Peek("account");
            var toDelete = _context.confirmed_report.Select(a => new confirmed_report { id = a.id }).ToList();
            _context.confirmed_report.RemoveRange(toDelete);
            _context.SaveChanges();
            if (ob.ToString()=="admin")
            {
                if (string.IsNullOrEmpty(begin)==false&&string.IsNullOrEmpty(end)==false)
                {
                    char[] sper1 = { '/' };
                    string[] strlist=begin.Split(sper1);
                    string[] strlist2 = end.Split(sper1);
                    string[] recordlist=new string[1000];
                    int i;
                    string[] day3=new string[1000];
                    string[] mon3=new string[1000];
                    string[] yer3=new string[1000];
                    int day1, day2, mon1, mon2, yer1,yer2;
                    int[] d3t=new int[1000];
                    int[] m3t=new int[1000];
                    int[] y3t = new int[1000];
                    day1 = int.Parse(strlist[1]);
                    day2 = int.Parse(strlist2[1]);
                    mon1 = int.Parse(strlist[0]);
                    mon2 = int.Parse(strlist2[0]);
                    yer1 = int.Parse(strlist[2]);
                    yer2 = int.Parse(strlist2[2]);
                    string[] record = _context.confirmedselect.AsEnumerable().Select(row => row.date.ToString()).ToArray();
                    char[] sper2 = { ' ' };
                    for ( i = 0; i <record.Length; i++)
                        {
                            recordlist[i] = record[i].Split(sper2).First();
                        }
                    for (i = 0; i < record.Length; i++)
                    {
                            day3[i] = recordlist[i].Split(sper1)[1];
                            yer3[i] = recordlist[i].Split(sper1).Last();
                            mon3[i] = recordlist[i].Split(sper1).First();
                    }
                    for(i=0; i < record.Length; i++)
                    {
                        d3t[i] = int.Parse(day3[i]);
                        m3t[i] = int.Parse(mon3[i]);
                        y3t[i] = int.Parse(yer3[i]);
                    }
                    for(i=0; i < record.Length; i++)
                    {
                        DateTime d1 = new DateTime(yer1, mon1, day1);
                        DateTime d2= new DateTime(yer2, mon2, day2);
                        DateTime d3 = new DateTime(y3t[i], m3t[i], d3t[i]);
                        int res = 0;
                        res = DateTime.Compare(d1, d3);
                        int res2=DateTime.Compare(d2, d3);
                        if (res < 0 && res2 >= 0)
                        {
                            var ans = new confirmed_report();
                            var answer = _context.confirmedselect.Where(j => j.date.Contains(record[i])).FirstAsync();
                            ans.firstname = answer.Result.firstname;
                            ans.date = answer.Result.date;
                            ans.lastname = answer.Result.lastname;
                            ans.namedoc = answer.Result.namedoc;
                            _context.Add<confirmed_report>(ans);
                            _context.SaveChanges();
                        }
                    }
                    return RedirectToAction(nameof(record_list));
                }

              
            }
            else
            {
                return RedirectToAction(nameof(login));
            }
            return View();
        }
        public async Task<IActionResult> report_all()
        {
            return _context.Registercs != null ?
                      View(await _context.confirmedselect.ToListAsync()) :
                      Problem("Entity set 'ApplicationDbContext.Registercs'  is null.");
        }
        public async Task<IActionResult> record_list()
        {
            return _context.Registercs != null ?
                      View(await _context.confirmed_report.ToListAsync()) :
                      Problem("Entity set 'ApplicationDbContext.Registercs'  is null.");
        }
        public async Task<IActionResult> select(int? id)
        {
            if (id == null || _context.names == null)
            {
                return NotFound();
            }
            return RedirectToAction(nameof(confirmedselect));
        }
        public async Task<IActionResult> confirmedselect(int? id)
        {
            object check = TempData.Peek("account");
            if (check != null)
            {
                int id2 = (int)id;
                TempData["id"] = id2;
                return View();
            }
            else
            {
                return RedirectToAction(nameof(error));
            }

        }
        public async Task<IActionResult> selected()
        {
            object var = TempData.Peek("email");
            object var2 = TempData.Peek("account");
            if (var2 != null)
            {
                int id = (int)TempData["id"];
                var acc = await _context.names.FirstOrDefaultAsync(m => m.id == id);
                string email = var.ToString();
                var temp = _context.account.FirstOrDefault(m => m.email == email);
                confirmedselect temp2 = new confirmedselect();
                temp2.firstname = temp.firstname;
                temp2.lastname = temp.lastname;
                temp2.date = DateTime.Today.ToString();
                temp2.namedoc = acc.namedoc;
                _context.Add<confirmedselect>(temp2);
                _context.SaveChanges();
                MimeMessage message = new MimeMessage();
                message.From.Add(new MailboxAddress("کینیک پرتو", "faridnemati98@gmail.com"));
                message.To.Add(MailboxAddress.Parse(email));
                message.Subject = "!رزرو نوبت";
                message.Body = new TextPart("plain")
                {
                    Text = "سلام درخواست شما دریافت  و نوبتتان روزو شد برای تایید نهایی نوبت و گرفتن وقت لطفا به شماره تلفن 654****0313 دروقت های اداری تماس بگیرید."
                };
                string emailaddress = "faridnemati98@gmail.com";
                string password = "hbnuavxaunzychob";
                SmtpClient client = new SmtpClient();
                client.Connect("smtp.gmail.com", 465, true);
                client.Authenticate(emailaddress, password);
                client.Send(message);
                return View();
            }
            else
            {
                return RedirectToAction(nameof(error));
            }
        }
        public async Task<IActionResult> confirm_create_password(int code,string password,string repeat_password)
        { 
            if (password != null && repeat_password != null)
            {
                int lenght = password.Length;
                if (lenght>8)
                {
                    if (password == repeat_password)
                    {
                        if (code == (int)TempData["code"])
                        {
                            string email = TempData["email"].ToString();
                            account acc = await _context.account.Where(j => j.email.Contains(email)).FirstAsync();
                            acc.password = password;
                            acc.repeat_password = repeat_password;
                            ViewBag.Message = "Your password is set!";
                            Thread.Sleep(5000);
                            return RedirectToAction(nameof(login));
                        }
                        else
                        {
                            return RedirectToAction(nameof(forgotten));
                        }
                    }
                    else
                    {
                        ViewBag.Message = "Two password field must match!";
                    }
                }
                else
                {
                    ViewBag.Message = "The password must have atleast 8 characters!";
                }
            }
            return View();

        }
        public async Task<IActionResult> forgotten(string email)
        {
            if (!string.IsNullOrEmpty(email))
            {   var check=_context.account.Any(j => j.email.Contains(email));
                if (check)
                {
                    TempData["email"] = email;
                    int code2 = GenerateRandomNo();
                    TempData["code"] = code2;
                    MimeMessage message = new MimeMessage();
                    message.From.Add(new MailboxAddress("کینیک پرتو", "faridnemati98@gmail.com"));
                    message.To.Add(MailboxAddress.Parse(email));
                    message.Subject = "!کد تایید";
                    message.Body = new TextPart("plain")
                    {
                        Text = "سلام درخواست شما دریافت شد کد تایید شما برابر است با:." + code2.ToString()
                    };
                    string emailaddress = "faridnemati98@gmail.com";
                    string password = "hbnuavxaunzychob";
                    SmtpClient client = new SmtpClient();
                    client.Connect("smtp.gmail.com", 465, true);
                    client.Authenticate(emailaddress, password);
                    client.Send(message);
                    return RedirectToAction(nameof(confirm_create_password));
                }
                else
                {
                    ViewBag.Message = "This user does not exist!";
                }
            }
            return View();
        }
        public async Task<IActionResult> cancel(int? id)
        {
            int id2 = (int)id;
            TempData["id2"] = id2;
            return View();
        }
        public async Task<IActionResult> confirm_cancel()
        {
            int id = (int)TempData["id2"];
            var acc = await _context.confirmedselect.FirstOrDefaultAsync(m => m.id == id);
            _context.Remove<confirmedselect>(acc);
            _context.SaveChanges();
            TempData["account"] = "loggedin";
            return View();
        }
        public async Task<IActionResult> lookup()
        {
            if (TempData["account"] != null)
            {
                TempData["account"] = "loggedin";
                var acc = _context.account.Where(j => j.email.Contains(TempData["email"].ToString())).First();
                TempData["email"] = acc.email;
                if (_context.confirmedselect.Any(j => j.firstname.Contains(acc.firstname)) && _context.confirmedselect.Any(j => j.lastname.Contains(acc.lastname)))
                {
                    return _context.confirmedselect != null ?
                         View(await _context.confirmedselect.Where(j => j.firstname.Contains(acc.firstname)).ToListAsync()) :
                         Problem("Entity set 'ApplicationDbContext.names'  is null.");
                }

            }
            else
            {
                return RedirectToAction(nameof(error));
            }
            return View();
        }
        public async Task<IActionResult> error()
        {
            return View();
        }
        public async Task<IActionResult> ad([Bind("id,field,days,price,namedoc")] names acc)
        {
            object na = TempData.Peek("account");
            if (na.ToString()=="admin")
            {
                if (acc.field != null && acc.days != null && acc.namedoc != null)
                {
                    _context.Add<names>(acc);
                    await _context.SaveChangesAsync();
                    ViewBag.Message = "New doctor was added!";
                }
                return View();
            }
            else
            {
                return RedirectToAction(nameof(login));
            }

        }
        public async Task<IActionResult> Delete_name(int? id)
        {
            if (id == null || _context.names == null)
            {
                return NotFound();
            }

            var nam = await _context.names
                .FirstOrDefaultAsync(m => m.id == id);
            if (nam == null)
            {
                return NotFound();
            }

            return View(nam);

        }
        public async Task<IActionResult> Delete_name_cofirmed(int? id)
        {
            if (_context.names== null)
            {
                return Problem("Entity set 'ApplicationDbContext.Registercs'  is null.");
            }
            var name = await _context.names.FindAsync(id);
            if (name != null)
            {
                _context.names.Remove(name);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(remove));
        }
        public async Task<IActionResult> remove()
        {
            object ob = TempData.Peek("account");
            if (ob.ToString()=="admin")
            {
                return RedirectToAction(nameof(Index_2));
            }
            else
            {
                return RedirectToAction(nameof(login));
            }
        }
        public async Task<IActionResult> Index_2()
        {
            object ob = TempData.Peek("account");
            if (ob.ToString() == "admin")
            {
                return _context.names != null ?
                          View(await _context.names.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.Registercs'  is null.");
            }
            else
            {
                return RedirectToAction(nameof(login));
            }
        }
        public async Task<IActionResult> editing_list()
        {
            object ob = TempData.Peek("account");
            if (ob.ToString()=="admin")
            {
                return _context.names != null ?
                          View(await _context.names.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.Registercs'  is null.");
            }
            else
            {
                return RedirectToAction(nameof(bossenter));
            }
        }
        public async Task<IActionResult> editing(int? id)
        {
            if (id == null || _context.names == null)
            {
                return NotFound();
            }

            var nam = await _context.names.FindAsync(id);
            if (nam == null)
            {
                return NotFound();
            }
            return View(nam);
        }
        [HttpPost]
        public async Task<IActionResult> editing(int id, [Bind("id,field,days,price,namedoc")] names nam)
        {
            TempData["account"] = "loggedin";
            if (id != nam.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(nam);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RegistercsExists(nam.id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(editing_list));
            }
            return View(nam);
        }
        public async Task<IActionResult> bossenter()
        {
            TempData["account"] = "admin";
            return View();
        }
        public async Task<IActionResult> logout()
        {
            TempData["account"]=null;
            return RedirectToAction(nameof(login));
        }
        public async Task<IActionResult> choose()
        {
            object value = TempData.Peek("account");
            if (value != null)
            {

                return _context.names != null ?
                             View(await _context.names.ToListAsync()) :
                             Problem("Entity set 'ApplicationDbContext.names'  is null.");
            }
            else
            {
                return RedirectToAction(nameof(error));
            }
        }
        public async Task<IActionResult> login(string email,string password)
        {
            var em2 = _context.account.Any(j => j.email.Contains(email));
            var pass2 = _context.account.Any(j => j.password.Contains(password));
            if (email == "admin" && password == "admin24871")
            {
                return RedirectToAction(nameof(bossenter));
            }
            if (em2==true && pass2==true)
            {
                TempData["email"] = email;
                TempData["account"] = "loggedin";
                return RedirectToAction(nameof(choose));  
            }
            else
            {
                if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(password))
                {
                    if (!em2 || !pass2)
                    {
                        ViewBag.Message = "Username or Password are wrong!";
                    }
                }
            }
            return View();
        }
        public async Task<IActionResult> Validation(int code)
        {
            validation validation = new validation() ;
            Registercs registercs = new Registercs();
            var lastval = _context.validation.OrderByDescending(a => a.id).First();
            var lastreg = _context.Registercs.OrderByDescending(a => a.id).First();
            await _context.SaveChangesAsync();

            if (code != lastval.code &&code!=0)
            {
                DeleteConfirmed(lastreg.id);
                return RedirectToAction(nameof(Register));
            }
            if(code == lastval.code)
            {return base.RedirectToAction(nameof(Models.account));

            }
            return View();
        }

        public async Task<IActionResult> account([Bind("id,firstname,lastname,email,password,repeat_password")] account acc)
        {
            int length = 0;
            var namef = _context.account.Any(j => j.firstname==acc.firstname);
            var namel = _context.account.Any(j => j.lastname==acc.lastname);
            var em= _context.account.Any(j=>j.email==acc.email);
            if (acc.firstname != null && acc.lastname != null && acc.email != null && acc.password != null && Equals(acc.password, acc.repeat_password) == true)
            {
                length = acc.password.Length; 
                if (length > 8)
                {
                    if (!namef && !namel)
                    {
                        if (!em)
                        {

                            _context.Add<account>(acc);
                            await _context.SaveChangesAsync();
                      }
                      else
                        {
                            ViewBag.Message = "This user has already been created!";
                        }
                    }
                    else if (namef && !namel)
                    {
                        if (!em)
                        {

                            _context.Add<account>(acc);
                            await _context.SaveChangesAsync();
                        }
                        else
                        {
                            ViewBag.Message = "This user has already been created!";
                        }
                    }
                    else if(!namef && namel)
                    {
                        if (!em)
                        {

                            _context.Add<account>(acc);
                            await _context.SaveChangesAsync();
                        }
                        else
                        {
                            ViewBag.Message = "This user has already been created!";
                        }
                    } 
                    else
                    {
                        ViewBag.Message = "This user has already been created!";
                   }
               }
                if (length <= 8)
                {
                   ViewBag.Message = "Your password must contain more than 8 characters";
                }
            }
            return View();
        }
        // GET: Registercs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Registercs == null)
            {
                return NotFound();
            }

            var registercs = await _context.Registercs
                .FirstOrDefaultAsync(m => m.id == id);
            if (registercs == null)
            {
                return NotFound();
            }

            return View(registercs);
        }

        // GET: Registercs/Create
      public IActionResult Create()
        {
            return View();
        }

        public int GenerateRandomNo()
        {
            int _min = 1000;
            int _max = 9999;
            Random _rdm = new Random();
            return _rdm.Next(_min, _max);
        }
        // POST: Registercs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,email")] Registercs registercs)
        {  int i = 0;
           MimeMessage message= new MimeMessage();
            int code=GenerateRandomNo();
            message.From.Add(new MailboxAddress("code", "faridnemati98@gmail.com"));
            message.To.Add(MailboxAddress.Parse(registercs.email));
            message.Subject="!کد تایید";
            message.Body = new TextPart("plain")
            {
                Text = "سلام به کلینیک پرتو خوش آمدید کد تایید شما:" + code.ToString()
            };
            string emailaddress = "faridnemati98@gmail.com";
            string password = "hbnuavxaunzychob";
            SmtpClient client=new SmtpClient();
            client.Connect("smtp.gmail.com", 465, true);
            client.Authenticate(emailaddress, password);
            client.Send(message);
            var ver = new validation { id = i++, code = code };
            if (ModelState.IsValid)
            {
                _context.Add<validation>(ver);
                await _context.SaveChangesAsync();
                _context.Add(registercs);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(validation));

            }
            return View(registercs);
        }

        // GET: Registercs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Registercs == null)
            {
                return NotFound();
            }

            var registercs = await _context.Registercs.FindAsync(id);
            if (registercs == null)
            {
                return NotFound();
            }
            return View(registercs);
        }

        // POST: Registercs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,email")] Registercs registercs)
        {
            if (id != registercs.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(registercs);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RegistercsExists(registercs.id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(registercs);
        }

        // GET: Registercs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Registercs == null)
            {
                return NotFound();
            }

            var registercs = await _context.Registercs
                .FirstOrDefaultAsync(m => m.id == id);
            if (registercs == null)
            {
                return NotFound();
            }

            return View(registercs);
        }

        // POST: Registercs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Registercs == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Registercs'  is null.");
            }
            var registercs = await _context.Registercs.FindAsync(id);
            if (registercs != null)
            {
                _context.Registercs.Remove(registercs);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RegistercsExists(int id)
        {
          return (_context.Registercs?.Any(e => e.id == id)).GetValueOrDefault();
        }
    }
}
