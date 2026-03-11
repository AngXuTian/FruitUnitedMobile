using System;
using System.Web.UI;

namespace FruitUnitedMobile.Component
{
    /// <summary>
    /// Toast Notification User Control
    /// Provides methods to display success, error, warning, and info toast messages
    /// </summary>
    public partial class Toast : System.Web.UI.UserControl
    {
        /// <summary>
        /// Page Load Event - No initialization needed
        /// </summary>
        protected void Page_Load(object sender, EventArgs e)
        {
            // User control is ready to use
        }

        /// <summary>
        /// Show a toast notification
        /// </summary>
        /// <param name="message">The message to display</param>
        /// <param name="type">Type of toast: 'success', 'error', 'warning', 'info'</param>
        /// <param name="title">Optional custom title (null for default)</param>
        /// <param name="duration">Duration in milliseconds (0 = no auto-hide, default = 5000)</param>
        public void Show(string message, string type = "info", string title = null, int duration = 5000)
        {
            if (string.IsNullOrEmpty(message))
            {
                return; // Don't show empty toast
            }

            // First escape special characters
            message = EscapeJavaScript(message);

            // Then convert newlines to <br> tags (after escaping to avoid issues)
            message = message.Replace("\\n", "<br>");

            // Handle title parameter
            string titleParam = string.IsNullOrEmpty(title)
                ? "null"
                : $"'{EscapeJavaScript(title)}'";

            // Build JavaScript function call
            string script = $"showToast('{message}', '{type}', {titleParam}, {duration});";

            // Register script to execute on page load
            ScriptManager.RegisterStartupScript(
                this.Page,
                this.Page.GetType(),
                "toast_" + Guid.NewGuid().ToString(),
                script,
                true
            );
        }

        /// <summary>
        /// Show a success toast notification
        /// </summary>
        /// <param name="message">Success message to display</param>
        /// <param name="title">Optional custom title (default: "Success")</param>
        /// <param name="duration">Duration in milliseconds (default: 5000)</param>
        public void ShowSuccess(string message, string title = null, int duration = 8000)
        {
            Show(message, "success", title, duration);
        }

        /// <summary>
        /// Show an error toast notification
        /// </summary>
        /// <param name="message">Error message to display</param>
        /// <param name="title">Optional custom title (default: "Error")</param>
        /// <param name="duration">Duration in milliseconds (default: 5000)</param>
        public void ShowError(string message, string title = null, int duration = 8000)
        {
            Show(message, "error", title, duration);
        }

        /// <summary>
        /// Show a warning toast notification
        /// </summary>
        /// <param name="message">Warning message to display</param>
        /// <param name="title">Optional custom title (default: "Warning")</param>
        /// <param name="duration">Duration in milliseconds (default: 5000)</param>
        public void ShowWarning(string message, string title = null, int duration = 8000)
        {
            Show(message, "warning", title, duration);
        }

        /// <summary>
        /// Show an info toast notification
        /// </summary>
        /// <param name="message">Info message to display</param>
        /// <param name="title">Optional custom title (default: "Information")</param>
        /// <param name="duration">Duration in milliseconds (default: 5000)</param>
        public void ShowInfo(string message, string title = null, int duration = 8000)
        {
            Show(message, "info", title, duration);
        }

        /// <summary>
        /// Convert newlines to HTML break tags
        /// </summary>
        /// <param name="input">String with newlines</param>
        /// <returns>String with <br> tags</returns>
        private string ConvertNewlinesToHtml(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }

            // Not used anymore - handled in Show() method
            return input;
        }

        /// <summary>
        /// Escape special characters for JavaScript string
        /// </summary>
        /// <param name="input">String to escape</param>
        /// <returns>Escaped string safe for JavaScript</returns>
        private string EscapeJavaScript(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }

            return input
                .Replace("\\", "\\\\")  // Backslash
                .Replace("'", "\\'")     // Single quote
                .Replace("\"", "\\\"")   // Double quote
                .Replace("\r", "")       // Carriage return (remove)
                .Replace("\n", "\\n")    // New line (will be converted to <br> later)
                .Replace("\t", "\\t")    // Tab
                .Replace("<", "\\x3C")   // Less than (prevent XSS)
                .Replace(">", "\\x3E");  // Greater than (prevent XSS)
        }
    }
}