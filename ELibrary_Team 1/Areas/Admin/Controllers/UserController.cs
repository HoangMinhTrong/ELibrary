﻿using ELibrary.Data;
using ELibrary_Team_1.Models;
using ELibrary_Team1.DataAccess.Data.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
namespace ELibrary_Team_1.Areas.Admin.Controllers
{
    [Area("Admin")]
    
    [Route("Admin/[controller]/[action]")]
    
    public class UserController : Controller
    {
        private readonly ELibraryDbContext _db;
        public UserController(ELibraryDbContext db)
        {
            _db = db;
        }


        public IActionResult Index()
        {
            return View();
        }


        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            var userList = _db.AppUsers.ToList();
            var userRole = _db.UserRoles.ToList();
            var role = _db.Roles.ToList();
            foreach ( var user in userList)
            {
                var roleId = userRole.FirstOrDefault(u => u.UserId == user.Id).RoleId;
                user.Role = role.FirstOrDefault(u => u.Id == roleId).Name;
            }
            return Json(new { data = userList });
        }

        [HttpPost]
        public IActionResult LockUnlock([FromBody] string id)
        {
            var objFromDb = _db.AppUsers.FirstOrDefault(u => u.Id == id);
            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Error while Locking/Unlocking" });
            }
            if (objFromDb.LockoutEnd != null && objFromDb.LockoutEnd > DateTime.Now)
            {
                //user is currently locked, we will unlock them
                objFromDb.LockoutEnd = DateTime.Now;
            }
            else
            {
                objFromDb.LockoutEnd = DateTime.Now.AddYears(1000);
            }
            _db.SaveChanges();
            return Json(new { success = true, message = "Operation Successful." });
        }

        #endregion
    }
}
