using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SamyraGlobal.Utilities
{
    public static class Message
    {
        public static void Show(this Controller controller, string title, string message, MessageType type)
        {
            controller.ViewBag.ShowPopup = true;
            controller.ViewBag.MessageTitle = title;
            controller.ViewBag.Message = message;
            controller.ViewBag.MessageType = type;
        }

    }
}