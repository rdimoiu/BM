using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BuildingManagement.DAL;
using BuildingManagement.Models;
using BuildingManagement.Utils;

namespace BuildingManagement.Controllers
{
    public class UserManagementController : Controller
    {
        private readonly UnitOfWork _unitOfWork = new UnitOfWork();

        // GET: UserManagement
        public ActionResult Index()
        {
            var users = _unitOfWork.UserRepository.Get();
            return View(users);
        }

        public ActionResult Edit(int id)
        {
            if (id == 0)
                return View(new User());
            var user = _unitOfWork.UserRepository.GetById(id);
            if (user == null)
                return HttpNotFound();

            user.RoleType = user.UserRoles.First().UserRoleType;

            return View(user);
        }

        [HttpPost, ActionName("Edit")]
        public ActionResult EditPost(User user)
        {
            if (user.Id == 0)
            {
                _unitOfWork.UserRepository.Insert(user);
                _unitOfWork.Save();
                return RedirectToAction("Index");
            }

            var userToUpdate = _unitOfWork.UserRepository.GetById(user.Id);
            if (userToUpdate == null)
            {
                return HttpNotFound();
            }
            try
            {
                userToUpdate.FirstName = user.FirstName;
                userToUpdate.LastName = user.LastName;
                userToUpdate.Email = user.Email;
                userToUpdate.UserRoles.First().UserRoleType = user.RoleType;

                _unitOfWork.Save();
                return RedirectToAction("Index");
            }
            catch (DataException)
            {
                ModelState.AddModelError("",
                    "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }
            return View(userToUpdate);
        }

        public ActionResult Create()
        {
            return View("Edit", new User());
        }

        [HttpPost, ActionName("Create")]
        public ActionResult CreatePost(User user)
        {
            user.Password = Cryptography.SimpleAes.Encrypt("bm123");
            _unitOfWork.UserRepository.Insert(user);
            var userRole = new UserRole
            {
                UserId = user.Id,
                UserRoleType = user.RoleType
            };
            _unitOfWork.UserRoleRepository.Insert(userRole);
            _unitOfWork.Save();
            return RedirectToAction("Index");
        }

        // GET: Level/Delete/5
        public ActionResult Delete(int? id, bool? saveChangesError = false)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (saveChangesError.GetValueOrDefault())
            {
                ViewBag.ErrorMessage = "Delete failed. Try again, and if the problem persists see your system administrator.";
            }
            var user = _unitOfWork.UserRepository.Get().Single(l => l.Id == id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Level/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                foreach (var role in _unitOfWork.UserRoleRepository.Get(item => item.UserId == id))
                    _unitOfWork.UserRoleRepository.Delete(role.Id);
                _unitOfWork.UserRepository.Delete(id);
                _unitOfWork.Save();
            }
            catch (DataException)
            {
                return RedirectToAction("Delete", new { id, saveChangesError = true });
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult GetAvailableRoles(int? roleSelected)
        {
            var list = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Value = ((int) Constants.RoleTypes.User).ToString(),
                    Text = Enum.GetName(typeof(Constants.RoleTypes), (int) Constants.RoleTypes.User),
                    Selected = roleSelected == (int) Constants.RoleTypes.User
                },
                new SelectListItem
                {
                    Value = ((int) Constants.RoleTypes.Admin).ToString(),
                    Text = Enum.GetName(typeof(Constants.RoleTypes), (int) Constants.RoleTypes.Admin),
                    Selected = roleSelected == (int) Constants.RoleTypes.Admin
                },
                new SelectListItem
                {
                    Value = ((int) Constants.RoleTypes.Sysadmin).ToString(),
                    Text = Enum.GetName(typeof(Constants.RoleTypes), (int) Constants.RoleTypes.Sysadmin),
                    Selected = roleSelected == (int) Constants.RoleTypes.Sysadmin
                }
            };

            return Json(list);
        }
    }
}