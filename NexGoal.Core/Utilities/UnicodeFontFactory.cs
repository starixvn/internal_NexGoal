using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace NexGoal.Core.Utilities
{
    public class UnicodeFontFactory : FontFactoryImp
    {
        private static readonly string FontPath = HttpContext.Current.Request.PhysicalApplicationPath + "Content\\Fonts\\Roboto\\Roboto-Regular.ttf";

        private readonly BaseFont _baseFont;

        public UnicodeFontFactory()
        {
            _baseFont = BaseFont.CreateFont(FontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED, true);
        }

        public override Font GetFont(string fontname, string encoding, bool embedded, float size, int style, BaseColor color, bool cached)
        {
            return new Font(_baseFont, size, style, color);
        }
    }
}
