using Domain.Shared;

namespace Application.Helpers;

public static class MessagesHelper
{
    private static readonly Dictionary<MessageType, (string Ar, string En)> _map = new()
    {
        // --- General ---
        { MessageType.Success, ("تمت العملية بنجاح", "Operation completed successfully") },
        { MessageType.RetrieveSuccessfully, ("تم جلب البيانات بنجاح", "Data retrieved successfully") },
        { MessageType.Failed, ("فشلت العملية", "Operation failed") },
        { MessageType.Unauthorized, ("غير مصرح لك بالقيام بهذا الإجراء", "Unauthorized action") },
        { MessageType.Forbidden, ("لا تملك الصلاحيات اللازمة", "You do not have the necessary permissions") },
        { MessageType.InvalidInput, ("البيانات المدخلة غير صحيحة", "Invalid input data") },
        { MessageType.SystemError, ("حدث خطأ في النظام", "System error occurred") },

        // --- Authentication ---
        { MessageType.RegisterSuccess, ("تم إنشاء الحساب بنجاح", "Account created successfully") },
        { MessageType.LoginSuccess, ("تم تسجيل الدخول بنجاح", "Logged in successfully") },
        { MessageType.InvalidCredentials, ("اسم المستخدم أو كلمة المرور غير صحيحة", "Invalid username or password") },
        { MessageType.InvalidPassword, ("كلمة المرور الحالية غير صحيحة", "The current password is incorrect.") },
        { MessageType.UserNotFound, ("المستخدم غير موجود", "User not found") },
        { MessageType.EmailAlreadyExists, ("البريد الإلكتروني مستخدم بالفعل", "Email is already in use") },
        { MessageType.EmailNotExists, ("البريد الإلكتروني غير مستخدم", "Email not used") },
        { MessageType.AccountDisabled, ("الحساب مغلق", "Account is disabled") },
        { MessageType.AccountLocked, ("الحساب مغلق", "Account locked") },
        { MessageType.PasswordResetSuccess, ("تم إعادة تعيين كلمة المرور بنجاح", "Password has been reset successfully") },
        { MessageType.UserCreated, ("تم إنشاء المستخدم بنجاح", "User created successfully") },
        { MessageType.WelcomeEmailSubject, ("مرحباً بك في نظام الجمعية", "Welcome to Rosca System") },
        { MessageType.PasswordResetSubject, ("إعادة تعيين كلمة المرور - نظام الجمعية", "Reset Password - Rosca System") },
        { MessageType.PasswordResetRequestReceived, ("إذا كان الحساب موجوداً، فقد تم إرسال رابط إعادة التعيين", "If an account exists, a reset email has been sent") }
    };

    private const string FallbackEn = "Message not found";
    private const string FallbackAr = "الرسالة غير موجودة";

    public static string GetMessage(MessageType type, Languages lang)
    {
        if (!_map.TryGetValue(type, out var val))
        {
            return lang == Languages.En ? FallbackEn : FallbackAr;
        }

        return lang == Languages.En ? val.En : val.Ar;
    }
}