using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ClassWithList.Models;

namespace ClassWithList.Controllers
{
    public class ApartmentsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Apartments
        public async Task<ActionResult> Index()
        {
            return View(await db.Apartments.ToListAsync());
        }

        // GET: Apartments/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Apartment apartment = await db.Apartments
                .Include(a => a.Rooms)
                .Where(a => a.Id == id)
                .FirstOrDefaultAsync();
            if (apartment == null)
            {
                return HttpNotFound();
            }
            return View(apartment);
        }

        // GET: Apartments/Create
        public ActionResult Create()
        {
            var Model = new Apartment() { };
            Model.Rooms = new List<Room>();
            return View(Model);
        }

        // POST: Apartments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MultipleButton(Name = "action", Argument = "Create")]
        public async Task<ActionResult> Create([Bind(Include = "Id,Number,Rooms")] Apartment Model)
        {
            if (ModelState.IsValid)
            {
                db.Apartments.Add(Model);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(Model);
        }

        [HttpPost]
        [MultipleButton(Name = "action", Argument = "CreateAddRoom")]
        public async Task<ActionResult> CreateAddRoom([Bind(Include = "Id,Number,Rooms")] Apartment Model)
        {
            if(Model.Rooms == null)
            {
                Model.Rooms = new List<Room>();
            }
            Model.Rooms.Add(new Room()
            {
                Type = "",
                Area = 5
            });
            ModelState.Clear();
            return View("Create", Model);
        }

        [HttpPost]
        [MultipleButton(Name = "action", Argument = "CreateRemoveRoom")]
        public async Task<ActionResult> CreateRemoveRoom([Bind(Include = "Id,Number,Rooms")] Apartment Model)
        {
            string removeRoomPos = Request.Form["RemoveRoom"];
            removeRoomPos = removeRoomPos.Replace(",","");
            Model.Rooms.RemoveAt(Convert.ToInt32(removeRoomPos));
            ModelState.Clear();
            return View("Create", Model);
        }

        // GET: Apartments/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Apartment Model = await db.Apartments
                .Include(a => a.Rooms)
                .Where(a => a.Id == id)
                .FirstOrDefaultAsync();
            if (Model == null)
            {
                return HttpNotFound();
            }
            return View(Model);
        }

        // POST: Apartments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MultipleButton(Name = "action", Argument = "Edit")]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Number,Rooms")] Apartment Model)
        {
            if (ModelState.IsValid)
            {
                // Add, edit rooms
                if(Model.Rooms != null)
                {
                    foreach(Room room in Model.Rooms)
                    {
                        room.ApartmentId = Model.Id;
                        if(room.Id == 0)
                        {
                            db.Rooms.Add(room);
                        }
                        else
                        {
                            db.Entry(room).State = EntityState.Modified;
                        }
                    }
                }

                // Delete rooms
                IList<Room> rooms = db.Rooms.Where(r => r.ApartmentId == Model.Id).ToList();
                foreach(Room roomDB in rooms)
                {
                    bool delete = true;
                    if(Model.Rooms != null)
                    {
                        foreach(Room roomM in Model.Rooms)
                        {
                            if(roomM.Id == roomDB.Id)
                            {
                                delete = false;
                                break;
                            }
                        }
                    }
                    if(delete)
                    {
                        db.Rooms.Remove(roomDB);
                    }
                }

                db.Entry(Model).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(Model);
        }

        [HttpPost]
        [MultipleButton(Name = "action", Argument = "EditAddRoom")]
        public async Task<ActionResult> EditAddRoom([Bind(Include = "Id,Number,Rooms")] Apartment Model)
        {
            if (Model.Rooms == null)
            {
                Model.Rooms = new List<Room>();
            }
            Model.Rooms.Add(new Room()
            {
                Type = "",
                Area = 5
            });
            ModelState.Clear();
            return View("Edit", Model);
        }

        [HttpPost]
        [MultipleButton(Name = "action", Argument = "EditRemoveRoom")]
        public async Task<ActionResult> EditRemoveRoom([Bind(Include = "Id,Number,Rooms")] Apartment Model)
        {
            string removeRoomPos = Request.Form["RemoveRoom"];
            removeRoomPos = removeRoomPos.Replace(",", "");
            Model.Rooms.RemoveAt(Convert.ToInt32(removeRoomPos));
            ModelState.Clear();
            return View("Edit", Model);
        }

        // GET: Apartments/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Apartment apartment = await db.Apartments.FindAsync(id);
            if (apartment == null)
            {
                return HttpNotFound();
            }
            return View(apartment);
        }

        // POST: Apartments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Apartment apartment = await db.Apartments.FindAsync(id);
            db.Apartments.Remove(apartment);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
