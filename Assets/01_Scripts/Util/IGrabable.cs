namespace Util
{
    interface IGrabable
    {
        public bool IsGrabed { get; }
        public void Grab();
    }
}
