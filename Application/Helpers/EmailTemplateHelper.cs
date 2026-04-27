using Domain.Shared;

namespace Application.Helpers;

public static class EmailTemplateHelper
{
    // --- Configuration Constants ---
    private const string HeaderColor = "#00897B"; // Teal/Green Theme
    private const string BodyBackgroundColor = "#f4f4f4";
    private const string ContainerColor = "#ffffff";
    private const string FontFamily = "'Segoe UI', Tahoma, Geneva, Verdana, sans-serif";

    /// <summary>
    /// Generates the full HTML email by wrapping specific content in the standard layout.
    /// </summary>
    private static string GenerateEmail(string title, string content, Languages language)
    {
        bool isRtl = language == Languages.Ar;
        string dir = isRtl ? "rtl" : "ltr";
        string align = isRtl ? "right" : "left";
        string langCode = isRtl ? "ar" : "en";

        // Footer Text
        string footerText = isRtl
            ? $"© {DateTime.Now.Year} Rosca System. جميع الحقوق محفوظة.<br>إذا كان لديك أي استفسار، لا تتردد في الاتصال بفريق الدعم."
            : $"© {DateTime.Now.Year} Rosca System. All rights reserved.<br>If you have any questions, feel free to contact our support team.";

        return $@"
        <!DOCTYPE html>
        <html lang='{langCode}' dir='{dir}'>
        <head>
            <meta charset='UTF-8'>
            <meta name='viewport' content='width=device-width, initial-scale=1.0'>
            <style>
                body {{ margin: 0; padding: 0; font-family: {FontFamily}; background-color: {BodyBackgroundColor}; }}
                .container {{ max-width: 600px; margin: 20px auto; background-color: {ContainerColor}; border-radius: 8px; overflow: hidden; box-shadow: 0 4px 6px rgba(0,0,0,0.1); }}
                .header {{ background-color: {HeaderColor}; color: white; padding: 30px 20px; text-align: center; }}
                .header h1 {{ margin: 0; font-size: 24px; }}
                .content {{ padding: 30px 20px; color: #333333; line-height: 1.6; text-align: {align}; direction: {dir}; }}
                .btn {{ display: inline-block; background-color: {HeaderColor}; color: white; padding: 12px 25px; text-decoration: none; border-radius: 5px; font-weight: bold; margin-top: 20px; }}
                .footer {{ background-color: #eeeeee; padding: 20px; text-align: center; font-size: 12px; color: #777777; }}
                
                /* List Styles */
                ul {{ padding: 0 {(isRtl ? "25px 0 0" : "0 0 25px")}; margin: 10px 0; list-style-position: inside; }}
                li {{ margin-bottom: 10px; }}
                
                a {{ color: {HeaderColor}; text-decoration: none; }}
            </style>
        </head>
        <body>
            <div class='container'>
                <div class='header'>
                    <h1>{title}</h1>
                </div>
                <div class='content'>
                    {content}
                </div>
                <div class='footer'>
                    <p>{footerText}</p>
                </div>
            </div>
        </body>
        </html>";
    }

    // --- Public Methods ---

    public static string GenerateWelcomeEmail(string userFullName, string dashboardLink, Languages language)
    {
        string title = language == Languages.Ar ? "مرحباً بك في نظام الجمعية" : "Welcome to Rosca System";
        string btnText = language == Languages.Ar ? "الذهاب إلى لوحة التحكم" : "Go to Dashboard";

        string bodyContent;
        if (language == Languages.Ar)
        {
            bodyContent = $@"
                <h2>أهلاً {userFullName}،</h2>
                <p>شكراً لانضمامك إلينا! لقد تم إنشاء حسابك بنجاح.</p>
                <p>نحن هنا لمساعدتك في إدارة مدخراتك والمشاركة في الجمعيات المالية بكل سهولة وأمان.</p>
                <p>يمكنك الآن:</p>
                <ul>
                    <li>إنشاء مجموعات ادخار جديدة.</li>
                    <li>الانضمام إلى المجموعات الموجودة.</li>
                    <li>متابعة مدفوعاتك واستلام مستحقاتك.</li>
                </ul>
                <div style='text-align: center; margin-top: 30px;'>
                    <a href='{dashboardLink}' class='btn'>{btnText}</a>
                </div>";
        }
        else
        {
            bodyContent = $@"
                <h2>Hi {userFullName},</h2>
                <p>Thanks for joining us! Your account has been created successfully.</p>
                <p>We are here to help you manage your savings and participate in financial circles securely and easily.</p>
                <p>You can now:</p>
                <ul>
                    <li>Create new saving groups.</li>
                    <li>Join existing circles.</li>
                    <li>Track your contributions and payouts.</li>
                </ul>
                <div style='text-align: center; margin-top: 30px;'>
                    <a href='{dashboardLink}' class='btn'>{btnText}</a>
                </div>";
        }

        return GenerateEmail(title, bodyContent, language);
    }

    public static string GenerateForgotPasswordBody(string userName, string resetLink, Languages language)
    {
        string title = language == Languages.Ar ? "إعادة تعيين كلمة المرور" : "Reset Password Request";
        string btnText = language == Languages.Ar ? "إعادة تعيين كلمة المرور" : "Reset Password";

        string bodyContent;
        if (language == Languages.Ar)
        {
            bodyContent = $@"
                <h2>مرحباً {userName}،</h2>
                <p>لقد طلبت إعادة تعيين كلمة المرور الخاصة بك في نظام الجمعية (Rosca System).</p>
                <p>يرجى النقر على الرابط أدناه لتعيين كلمة مرور جديدة:</p>
                <div style='text-align: center; margin: 30px 0;'>
                    <a href='{resetLink}' class='btn'>{btnText}</a>
                </div>
                <p>إذا لم تقم بهذا الطلب، يرجى تجاهل هذا البريد الإلكتروني.</p>
                <p style='font-size: 12px; color: #777; margin-top: 20px;'>هذا الرابط صالح لمدة ساعة واحدة.</p>";
        }
        else
        {
            bodyContent = $@"
                <h2>Hi {userName},</h2>
                <p>You requested to reset your password on Rosca System.</p>
                <p>Please click the link below to set a new password:</p>
                <div style='text-align: center; margin: 30px 0;'>
                    <a href='{resetLink}' class='btn'>{btnText}</a>
                </div>
                <p>If you didn't make this request, please ignore this email.</p>
                <p style='font-size: 12px; color: #777; margin-top: 20px;'>This link is valid for 1 hour.</p>";
        }

        return GenerateEmail(title, bodyContent, language);
    }
}