using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using MainSite.Data;
using MainSite.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MainSite.Controllers
{
    
    public class TransactionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ApplicationUser CurrentUser
        {
            get
            {
                if (!User.Identity.IsAuthenticated)
                {
                    return null;
                }
                var username = User.Identity.Name;
                return _context.Users.FirstOrDefault(u => u.UserName == username);
            }
        }

        public TransactionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Authorize]
        public IActionResult Index(string iban = null)
        {
            ViewData["SearchIban"] = iban;

            //OWASP #1 Sql Injection ( ' OR 1=1 -- )
            using (var cmd = ((SqlConnection)_context.Database.GetDbConnection()).CreateCommand())
            {
                var query = string.Format(
                    @"SELECT * 
                        FROM 
                           Transactions INNER JOIN AspNetUsers 
                        ON UserId = AspNetUsers.Id 
                        WHERE UserId='{0}' ", CurrentUser.Id);
                if (!string.IsNullOrWhiteSpace(iban)) { query += string.Format(" AND IBAN = '{0}'", iban); }

                cmd.CommandText = query;
                var datatable = new DataTable();
                var adapter = new SqlDataAdapter(cmd);
                adapter.Fill(datatable);
                return View("IndexInjection", datatable);
            }


            using (var cmd = ((SqlConnection)_context.Database.GetDbConnection()).CreateCommand())
            {
                var query =
                    @"SELECT * 
                         FROM 
                            Transactions INNER JOIN AspNetUsers 
                         ON UserId = AspNetUsers.Id 
                        WHERE UserId= @UserId ";
                cmd.Parameters.Add(new SqlParameter("@UserId", SqlDbType.VarChar) { Value = CurrentUser.Id });
                if (!string.IsNullOrWhiteSpace(iban))
                {
                    query += " AND IBAN = @IBAN";
                    cmd.Parameters.Add(new SqlParameter("@IBAN", SqlDbType.VarChar) { Value = iban });
                }
                cmd.CommandText = query;

                var datatable = new DataTable();
                var adapter = new SqlDataAdapter(cmd);
                adapter.Fill(datatable);
                return View("IndexInjection", datatable);
            }


            var transactions = _context.Transactions.Where(t => t.UserId == CurrentUser.Id);
            if (!string.IsNullOrWhiteSpace(iban))
            {
                transactions = transactions.Where(t => t.Iban == iban);
            }
            return View(transactions.ToList());
        }

        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //OWASP #4 Insecure direct object references
            var transaction = await _context.Transactions
                .Include(t => t.User)
                .SingleOrDefaultAsync(m => m.TransactionId == id);
            //.SingleOrDefaultAsync(m => m.TransactionId == id && m.UserId == CurrentUser.Id) 

            if (transaction == null)
            {
                return NotFound();
            }

            return View(transaction);
        }

        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize]
        [HttpPost]

        //OWASP #8 Cross site request forgery (CSRF)
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TransactionId,Amount,Iban")] Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                transaction.Created = DateTime.Now;
                transaction.UserId = CurrentUser.Id;
                _context.Add(transaction);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(transaction);
        }
     
    }
}
