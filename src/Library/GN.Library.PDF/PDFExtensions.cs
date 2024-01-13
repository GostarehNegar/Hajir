using Syncfusion.HtmlConverter;
using Syncfusion.Pdf;
using Syncfusion.Licensing;
using System;
using System.IO;
using System.Reflection;
using System.Linq;
using GN.Parnian.Library.EasyHook;

namespace GN.Library.PDF
{
    class HHH
    {
        public string CheckLicense()
        {
            return "";
        }
    }
    public static partial class PDFExtensions
    {
        public static bool Hacked;
        static LocalHook hook;
        public static MemoryStream Test(string url)
        {
            //how_to_use_LocalHook();
            Func<Platform, string> getOne = (Platform p) =>
            {
                return null;
            };
            if (hook == null)
            {
                var type = typeof(FusionLicenseProvider);
                var methods = type.GetMethods(BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                    .Where(x => x.Name == "GetLicenseType")
                    .ToList();

                var method1 = type.GetMethods(BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                    .Skip(1)
                    .ToList()
                    .FirstOrDefault(x => x.Name == "GetLicenseType");
                method1 = methods[1];

                var method2 = typeof(HHH).GetMethods(BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                    .FirstOrDefault(x => x.Name == "CheckLicense");
                method2 = getOne.Method;



                System.Runtime.CompilerServices.RuntimeHelpers.PrepareMethod(method1.MethodHandle);
                System.Runtime.CompilerServices.RuntimeHelpers.PrepareMethod(method2.MethodHandle);

                hook = LocalHook
                    .CreateUnmanaged(method1.MethodHandle.GetFunctionPointer(), method2.MethodHandle.GetFunctionPointer(), IntPtr.Zero);
                hook.ThreadACL.SetExclusiveACL(new int[] { -1 });
            }
            BlinkConverterSettings settings = new BlinkConverterSettings()
            {
                Orientation = PdfPageOrientation.Portrait
            };

            HtmlToPdfConverter htmlConverter = new HtmlToPdfConverter()
            {
                ConverterSettings = settings
            };
            PdfDocument document = htmlConverter.Convert(url);
            document.PageSettings.Orientation = PdfPageOrientation.Portrait;
            MemoryStream stream = new MemoryStream();
            document.Save(stream);

            return stream;
            
            //Syncfusion.HtmlConverter.HtmlToPdfConverter.
        }
    }
}
