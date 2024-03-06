using DocumentService.Core.Entities;
using System.Linq.Expressions;

namespace DocumentService.Infrastructure.Specifications;

public abstract class SpecificationBase<TEntity>
    where TEntity : EntityBase
{
    public SpecificationBase(Expression<Func<TEntity, bool>>? criteria) =>
         Criteria = criteria;

    public bool IsQueryFiltersIgnored { get; private set; }

    public Expression<Func<TEntity, bool>>? Criteria { get; set; }

    public List<Expression<Func<TEntity, object>>> IncludeExpressions { get; } = new();

    public Expression<Func<TEntity, object>>? OrderByExpression { get; private set; }

    public Expression<Func<TEntity, object>>? OrderByDescendingExpression { get; private set; }

    protected void AddInclude(Expression<Func<TEntity, object>> includeExpressions) =>
        IncludeExpressions.Add(includeExpressions);

    protected void AddOrderBy(
        Expression<Func<TEntity, object>> orderByExpressions) =>
        OrderByExpression = orderByExpressions;

    protected void AddOrderByDescending(
        Expression<Func<TEntity, object>> orderByDescendingExpression) =>
        OrderByDescendingExpression = orderByDescendingExpression;

    protected void IgnoreQueryFilters() =>
        IsQueryFiltersIgnored = true;
}
