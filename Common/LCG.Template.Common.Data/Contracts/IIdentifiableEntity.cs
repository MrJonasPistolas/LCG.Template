namespace LCG.Template.Common.Data.Contracts
{
    public interface IIdentifiableEntity
    {
    }

    public interface IIdentifiableEntity<T> : IIdentifiableEntity
    {
        T Id { get; set; }
    }
}
