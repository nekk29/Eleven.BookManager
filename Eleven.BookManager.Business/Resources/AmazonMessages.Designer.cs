﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Eleven.BookManager.Business.Resources {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class AmazonMessages {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal AmazonMessages() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Eleven.BookManager.Business.Resources.AmazonMessages", typeof(AmazonMessages).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Amazon account email not found in settings.
        /// </summary>
        internal static string AccountEmailNotFound {
            get {
                return ResourceManager.GetString("AccountEmailNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Author not found.
        /// </summary>
        internal static string AuthorNotFound {
            get {
                return ResourceManager.GetString("AuthorNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Book file is empty.
        /// </summary>
        internal static string BookFileEmpty {
            get {
                return ResourceManager.GetString("BookFileEmpty", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Book file &quot;{0}&quot; not found.
        /// </summary>
        internal static string BookFileNotFound {
            get {
                return ResourceManager.GetString("BookFileNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Book not found.
        /// </summary>
        internal static string BookNotFound {
            get {
                return ResourceManager.GetString("BookNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Book couldn&apos;t be deleted: {0}.
        /// </summary>
        internal static string DeleteBookError {
            get {
                return ResourceManager.GetString("DeleteBookError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Book couldn&apos;t be deleted because it was already sent.
        /// </summary>
        internal static string DeleteBookErrorAlreadySent {
            get {
                return ResourceManager.GetString("DeleteBookErrorAlreadySent", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Book was delete successfully.
        /// </summary>
        internal static string DeleteBookSuccess {
            get {
                return ResourceManager.GetString("DeleteBookSuccess", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Books from author couldn&apos;t be marked: {0}.
        /// </summary>
        internal static string MarkAuthorError {
            get {
                return ResourceManager.GetString("MarkAuthorError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Books from author were marked successfully.
        /// </summary>
        internal static string MarkAuthorSuccess {
            get {
                return ResourceManager.GetString("MarkAuthorSuccess", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Book couldn&apos;t be marked: {0}.
        /// </summary>
        internal static string MarkBookError {
            get {
                return ResourceManager.GetString("MarkBookError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Book was marked successfully.
        /// </summary>
        internal static string MarkBookSuccess {
            get {
                return ResourceManager.GetString("MarkBookSuccess", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Books from author &quot;{0}&quot; couldn&apos;t be sent: {1}.
        /// </summary>
        internal static string SendAuthorError {
            get {
                return ResourceManager.GetString("SendAuthorError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Books from author were sent successfully.
        /// </summary>
        internal static string SendAuthorSuccess {
            get {
                return ResourceManager.GetString("SendAuthorSuccess", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Book already sent.
        /// </summary>
        internal static string SendBookAlreadySent {
            get {
                return ResourceManager.GetString("SendBookAlreadySent", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Book couldn&apos;t be sent.
        /// </summary>
        internal static string SendBookError {
            get {
                return ResourceManager.GetString("SendBookError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Book was sent successfully.
        /// </summary>
        internal static string SendBookSuccess {
            get {
                return ResourceManager.GetString("SendBookSuccess", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Book &quot;{0}&quot; not sent because it is marked as pending.
        /// </summary>
        internal static string SendBookWarningPending {
            get {
                return ResourceManager.GetString("SendBookWarningPending", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Error sending email for book &quot;{0}&quot;: {1}.
        /// </summary>
        internal static string SendEmailError {
            get {
                return ResourceManager.GetString("SendEmailError", resourceCulture);
            }
        }
    }
}
