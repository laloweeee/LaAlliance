namespace ASI.Basecode.WebApp.Models
{
    public class ToastNotificationViewModel
    {
        public string Message { get; set; }
        public ToastType Type { get; set; }

        public string BackgroundClass => Type switch
        {
            ToastType.Success => "bg-green-50",
            ToastType.Error => "bg-red-50",
            ToastType.Warning => "bg-yellow-50",
            ToastType.Info => "bg-blue-50",
            _ => "bg-gray-50"
        };

        public string BorderClass => Type switch
        {
            ToastType.Success => "border-green-200",
            ToastType.Error => "border-red-200",
            ToastType.Warning => "border-yellow-200",
            ToastType.Info => "border-blue-200",
            _ => "border-gray-200"
        };

        public string TextClass => Type switch
        {
            ToastType.Success => "text-green-800",
            ToastType.Error => "text-red-800",
            ToastType.Warning => "text-yellow-800",
            ToastType.Info => "text-blue-800",
            _ => "text-gray-800"
        };

        public string IconBackgroundClass => Type switch
        {
            ToastType.Success => "bg-green-500",
            ToastType.Error => "bg-red-500",
            ToastType.Warning => "bg-yellow-500",
            ToastType.Info => "bg-blue-500",
            _ => "bg-gray-500"
        };

        public string IconClass => Type switch
        {
            ToastType.Success => "fas fa-check",
            ToastType.Error => "fas fa-trash",
            ToastType.Warning => "fas fa-exclamation",
            ToastType.Info => "fas fa-info",
            _ => "fas fa-bell"
        };
    }

    public enum ToastType
    {
        Success,
        Error,
        Warning,
        Info
    }
}