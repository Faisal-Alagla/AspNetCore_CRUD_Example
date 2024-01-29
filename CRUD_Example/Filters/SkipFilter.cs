using Microsoft.AspNetCore.Mvc.Filters;

namespace CRUD_Example.Filters
{
    public class SkipFilter : Attribute, IFilterMetadata
    {
    }
}
