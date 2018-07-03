namespace NexGoal.Controllers
{
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Mvc;
    using NexGoal.Models.UserManagement;

    public class UserManagementController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult GetUsers(string keyword, int? groupId, int? categoryId, bool isDisabledUser)
        {
            var list = new List<UserModel>();

            for (var i = 0; i < 100; i++)
            {
                list.Add(new UserModel
                {
                    UserId = i,
                    Group = "123",
                    JobTitle = "456"
                });
            }

            return Ok(list);
        }
    }
}