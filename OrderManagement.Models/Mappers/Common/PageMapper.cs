using OrderManagement.Domain.Common;
using OrderManagement.Models.Responses.Common;

namespace OrderManagement.Models.Mappers.Common;

public static class PageMapper
{
    public static PageModel<TModel> Map<TModel, TEntity>(Page<TEntity> page, Func<TEntity, TModel> convertFunc)
    {
        return new PageModel<TModel>(page.PageNumber, page.PageSize, page.Total, page.Items.Select(i => convertFunc(i)).ToList());
    }
}
