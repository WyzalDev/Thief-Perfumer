namespace Game.Scripts.Utility
{
    public interface IBlockable
    {
        public bool IsBlocked { get; }

        public void Block();
        public void UnBlock();
    }
}