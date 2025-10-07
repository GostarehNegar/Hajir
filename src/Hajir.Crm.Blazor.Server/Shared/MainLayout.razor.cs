using Hajir.Crm.Blazor.XrmFrames;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Hajir.Crm.Blazor.Server.Shared
{
    public partial class MainLayout
    {
        [Inject]
        public NavigationManager Nav { get; set; }
        public string URL { get; set; }
        protected override void OnParametersSet()
        {
            URL = Nav.Uri;
            base.OnParametersSet();
        }
        public bool IsNotXrmFrame => !URL.Contains("/quote/") && !XrmFrameBaseEx.IsXrmPage(URL) && !URL.Contains("/print/");
        
        private static readonly string[] Fonts = new[] { "Sahel", "Poppins", "Helvetica", "Arial", "sans-serif" };

        public MudTheme MyCustomTheme = new()
        {
           

            Typography = new Typography()
            {
                Default = new Default()
                {
                    FontFamily = Fonts,
                    LineHeight = 1.5
                },
                Body1 = new Body1()
                {
                    FontFamily = Fonts,
                    LineHeight = 1.5
                },
                Body2 = new Body2()
                {
                    FontFamily = Fonts,
                    LineHeight = 1.5

                },
                Button = new Button()
                {
                    FontFamily = Fonts,
                    LineHeight = 1.5
                },
                Caption = new Caption()
                {
                    FontFamily = Fonts,
                    LineHeight = 1.5
                },
                H1 = new H1()
                {
                    FontFamily = Fonts,
                    LineHeight = 1.5
                },
                H2 = new H2()
                {
                    FontFamily = Fonts,
                    LineHeight = 1.5
                },
                H3 = new H3()
                {
                    FontFamily = Fonts,
                    LineHeight = 1.5
                },
                H4 = new H4()
                {
                    FontFamily = Fonts,
                    LineHeight = 1.5
                },
                H5 = new H5()
                {
                    FontFamily = Fonts,
                    LineHeight = 1.5
                },
                H6 = new H6()
                {
                    FontFamily = Fonts,
                    LineHeight = 1.5
                },
                Overline = new Overline()
                {
                    FontFamily = Fonts,
                    LineHeight = 1.5
                },
                Subtitle1 = new Subtitle1()
                {
                    FontFamily = Fonts,
                    LineHeight = 1.5
                },
                Subtitle2 = new Subtitle2()
                {
                    FontFamily = Fonts,
                    LineHeight = 1.5
                }

            },

        };
    }
}
