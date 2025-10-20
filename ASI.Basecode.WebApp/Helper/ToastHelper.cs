using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using ASI.Basecode.WebApp.Models;
using System.Text.Json;

namespace ASI.Basecode.WebApp.Helpers
{
    /// <summary>
    /// Helper class for managing toast notifications across the application
    /// </summary>
    public static class ToastHelper
    {
        private const string ToastKey = "Toast";

        /// <summary>
        /// Shows a success toast (uses TempData for redirects)
        /// </summary>
        public static void ShowSuccessToast(this Controller controller, string message)
        {
            SetToast(controller.TempData, message, ToastType.Success);
        }

        /// <summary>
        /// Shows an error toast (uses TempData for redirects)
        /// </summary>
        public static void ShowErrorToast(this Controller controller, string message)
        {
            SetToast(controller.TempData, message, ToastType.Error);
        }

        /// <summary>
        /// Shows a warning toast (uses TempData for redirects)
        /// </summary>
        public static void ShowWarningToast(this Controller controller, string message)
        {
            SetToast(controller.TempData, message, ToastType.Warning);
        }

        /// <summary>
        /// Shows an info toast (uses TempData for redirects)
        /// </summary>
        public static void ShowInfoToast(this Controller controller, string message)
        {
            SetToast(controller.TempData, message, ToastType.Info);
        }

        /// <summary>
        /// Shows a success toast on the same page (uses ViewBag, no redirect)
        /// </summary>
        public static void ShowSuccessToastNow(this Controller controller, string message)
        {
            controller.ViewBag.Toast = new ToastNotificationViewModel
            {
                Message = message,
                Type = ToastType.Success
            };
        }

        /// <summary>
        /// Shows an error toast on the same page (uses ViewBag, no redirect)
        /// </summary>
        public static void ShowErrorToastNow(this Controller controller, string message)
        {
            controller.ViewBag.Toast = new ToastNotificationViewModel
            {
                Message = message,
                Type = ToastType.Error
            };
        }

        /// <summary>
        /// Shows a warning toast on the same page (uses ViewBag, no redirect)
        /// </summary>
        public static void ShowWarningToastNow(this Controller controller, string message)
        {
            controller.ViewBag.Toast = new ToastNotificationViewModel
            {
                Message = message,
                Type = ToastType.Warning
            };
        }

        /// <summary>
        /// Shows an info toast on the same page (uses ViewBag, no redirect)
        /// </summary>
        public static void ShowInfoToastNow(this Controller controller, string message)
        {
            controller.ViewBag.Toast = new ToastNotificationViewModel
            {
                Message = message,
                Type = ToastType.Info
            };
        }

        /// <summary>
        /// Private helper to serialize and store toast in TempData
        /// </summary>
        private static void SetToast(ITempDataDictionary tempData, string message, ToastType type)
        {
            var toast = new ToastNotificationViewModel
            {
                Message = message,
                Type = type
            };
            tempData[ToastKey] = JsonSerializer.Serialize(toast);
        }

        /// <summary>
        /// Gets and removes toast from TempData (for views)
        /// </summary>
        public static ToastNotificationViewModel GetToast(ITempDataDictionary tempData)
        {
            if (tempData[ToastKey] != null)
            {
                var toastJson = tempData[ToastKey].ToString();
                return JsonSerializer.Deserialize<ToastNotificationViewModel>(toastJson);
            }
            return null;
        }
    }
}