namespace DocumentService.Core.Entities
{
    public class EntityBase { }

    public class EntityBase<TKey> : EntityBase where TKey : IEquatable<TKey> 
    {
        public TKey Id { get; private set; } = default!;
    }
}
