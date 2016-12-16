using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace NexGoal.Services.Interfaces.ExportFile
{
    public interface IRazorRenderService
    {
        string RenderViewToString(Controller controller, string viewPath, object model);
    }
}
